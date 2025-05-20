using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

/// <summary>
///     Class representing the guide menu in the game.
/// </summary>
public partial class GuideMenu : Control {
    private readonly Dictionary<Button, VBoxContainer> buttons = new();
    private readonly Dictionary<string, (Texture2D, string)> cardData = new();
    private Button[] buildingCardButtons;
    private VBoxContainer buildingList;
    private Button buildingsButton;

    private TextureRect cardImage;
    private Label cardInfoLabel;
    private Label descriptionLabel;
    private Button[] eventCardButtons;
    private VBoxContainer eventsList;
    private Button foodButton;
    private Button[] foodCardButtons;
    private VBoxContainer foodList;
    private Button goBackButton;

    private MenuController menuController;
    private Button natureButton;
    private Button[] natureCardButtons;
    private VBoxContainer natureList;
    private Button packsButton;
    private Button[] packsButtons;
    private VBoxContainer packsList;
    private Button randomEventsButton;

    private Button[] resourceCardButtons;

    private VBoxContainer resourceList;

    private Button resourcesButton;
    private Button[] toolCardButtons;
    private VBoxContainer toolList;
    private Button toolsButton;
    private Button[] villagerCardButtons;
    private VBoxContainer villagerList;
    private Button villagersButton;

    public override void _Ready() {
        menuController = GetNode<MenuController>("/root/MenuController");
        cardImage = GetNode<TextureRect>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/CardImage");
        descriptionLabel =
            GetNode<Label>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/DescriptionLabel");
        cardInfoLabel = GetNode<Label>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/InfoLabel");

        descriptionLabel.Visible = false;
        cardInfoLabel.Visible = false;
        cardImage.Visible = false;

        InitializeMainButtons();
        InitializeLists();
        InitializeDescriptions();
        InitializeResourceCardButtons();
        InitializeToolCardButtons();
        InitializeBuildingCardButtons();
        InitializeFoodCardButtons();
        InitializeNatureCardButtons();
        InitializeVillagerCardButtons();
        InitializeRandomEventButtons();
        InitializePacksButtons();
    }

    /// <summary>
    ///     Extracts the base text from a button's text, removing any arrow indicators.
    /// </summary>
    /// <param name="button">The button to extract the base text from</param>
    /// <returns>The base text of the button with the arrow indicator removed</returns>
    private string GetBaseButtonText(Button button) {
        string text = button.Text;
        int arrowIndex = text.IndexOf(">");
        if (arrowIndex == -1) arrowIndex = text.LastIndexOf('v');
        if (arrowIndex != -1) return text.Substring(0, arrowIndex).Trim();
        return text;
    }

    /// <summary>
    ///     Loads the card image from the specified path.
    /// </summary>
    /// <param name="cardName">The name of the card to load the image for</param>
    /// <returns>The card image or a default image if no image is found</returns>
    private Texture2D LoadCardTexture(string cardName) {
        string cardImagePath = "res://Assets/Cards/Ready To Use/" + cardName + ".png";
        string defaultPath = "res://Assets/Cards/Ready To Use/Error.png";
        if (!FileAccess.FileExists(cardImagePath)) {
            GD.PrintErr($"Card image not found: {cardImagePath}");
            return GD.Load<Texture2D>(defaultPath);
        }

        Texture2D cardTexture = GD.Load<Texture2D>(cardImagePath);

        return cardTexture;
    }

    private Texture2D LoadPackTexture(string cardName) {
        string packImagePath = "res://Assets/Packs/" + cardName + ".png";
        string defaultPath = "res://Assets/Packs/Error.png";
        if (!FileAccess.FileExists(packImagePath)) {
            GD.PrintErr($"Pack image not found: {packImagePath}");
            return GD.Load<Texture2D>(defaultPath);
        }

        Texture2D packTexture = GD.Load<Texture2D>(packImagePath);
        return packTexture;
    }

    /// <summary>
    ///     Resets the guide menu to its initial state, hiding all submenus and resetting button texts.
    /// </summary>
    private void ResetGuideMenu() {
        foreach (KeyValuePair<Button, VBoxContainer> choice in buttons) {
            choice.Value.Visible = false;
            choice.Key.Text = GetBaseButtonText(choice.Key) + " >";
        }

        cardImage.Texture = null;
        cardInfoLabel.Text = "";

        descriptionLabel.Visible = false;
        cardInfoLabel.Visible = false;
        cardImage.Visible = false;
    }

    #region Initialization of submenus

    /// <summary>
    ///     Initializes the main category buttons and assigns them event handlers that trigger OnMainButtonPressed When
    ///     clicked.
    ///     Also initializes the go back button which exits the menu
    /// </summary>
    private void InitializeMainButtons() {
        toolsButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ButtonContainer/ToolsButton");
        foodButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ButtonContainer/FoodButton");
        buildingsButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ButtonContainer/BuildingsButton");
        natureButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ButtonContainer/NatureButton");
        villagersButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ButtonContainer/VillagersButton");
        resourcesButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer//ButtonContainer/ResourcesButton");
        randomEventsButton =
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ButtonContainer/RandomEventsButton");
        packsButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ButtonContainer/PacksButton");

        toolsButton.Pressed += () => OnMainButtonPressed(toolsButton);
        foodButton.Pressed += () => OnMainButtonPressed(foodButton);
        buildingsButton.Pressed += () => OnMainButtonPressed(buildingsButton);
        natureButton.Pressed += () => OnMainButtonPressed(natureButton);
        villagersButton.Pressed += () => OnMainButtonPressed(villagersButton);
        resourcesButton.Pressed += () => OnMainButtonPressed(resourcesButton);
        randomEventsButton.Pressed += () => OnMainButtonPressed(randomEventsButton);
        packsButton.Pressed += () => OnMainButtonPressed(packsButton);

        goBackButton = GetNode<Button>("GoBackButton");
        goBackButton.Pressed += OnGoBackButtonPressed;
    }

    /// <summary>
    ///     Retrieves and assigns the list containers for each card category and maps each button to its corresponding list
    ///     container.
    /// </summary>
    private void InitializeLists() {
        toolList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ToolList");
        foodList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/FoodList");
        buildingList =
            GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/BuildingList");
        natureList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/NatureList");
        villagerList =
            GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/VillagerList");
        resourceList =
            GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList");
        eventsList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/EventsList");
        packsList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/PacksList");


        buttons[toolsButton] = toolList;
        buttons[foodButton] = foodList;
        buttons[buildingsButton] = buildingList;
        buttons[natureButton] = natureList;
        buttons[villagersButton] = villagerList;
        buttons[resourcesButton] = resourceList;
        buttons[randomEventsButton] = eventsList;
        buttons[packsButton] = packsList;


        toolList.Visible = false;
        foodList.Visible = false;
        buildingList.Visible = false;
        natureList.Visible = false;
        villagerList.Visible = false;
        resourceList.Visible = false;
        eventsList.Visible = false;
        packsList.Visible = false;
    }

    /// <summary>
    ///     Initializes all individual card buttons under the resources category and assigns them event handlers that trigger
    ///     OnSubButtonPressed when clicked.
    /// </summary>
    private void InitializeResourceCardButtons() {
        resourceCardButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/WoodButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/PlanksButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/StickButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/StoneButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/WaterButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/BrickButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/SandButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/GlassButton"),
            GetNode<Button>(
                "TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/LeafButton"), //no leaf card yet 
            GetNode<Button>(
                "TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ResourceList/ClayButton") //no clay card yet 
        };

        foreach (Button button in resourceCardButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    /// <summary>
    ///     Initializes all individual card buttons under the tools category and assigns them event handlers that trigger
    ///     OnSubButtonPressed when clicked.
    /// </summary>
    private void InitializeToolCardButtons() {
        toolCardButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ToolList/SwordButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ToolList/FishingPoleButton"),
            GetNode<Button>(
                "TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ToolList/ShovelButton"), //no shovel card yet 
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/ToolList/AxeButton")
        };

        foreach (Button button in toolCardButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    /// <summary>
    ///     Initializes all individual card buttons under the buildings category and assigns them event handlers that trigger
    ///     OnSubButtonPressed when clicked.
    /// </summary>
    private void InitializeBuildingCardButtons() {
        buildingCardButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/BuildingList/GreenhouseButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/BuildingList/HouseButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/BuildingList/TentButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/BuildingList/FieldButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/BuildingList/CampfireButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/BuildingList/CookingPotButton")
        };

        foreach (Button button in buildingCardButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    /// <summary>
    ///     Initializes all individual card buttons under the food category and assigns them event handlers that trigger
    ///     OnSubButtonPressed when clicked.
    /// </summary>
    private void InitializeFoodCardButtons() {
        foodCardButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/FoodList/AppleButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/FoodList/BerryButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/FoodList/JamButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/FoodList/MeatButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/FoodList/CookedMeatButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/FoodList/FishButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/FoodList/CookedFishButton")
        };

        foreach (Button button in foodCardButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    /// <summary>
    ///     Initializes all individual card buttons under the nature category and assigns them event handlers that trigger
    ///     OnSubButtonPressed when clicked.
    /// </summary>
    private void InitializeNatureCardButtons() {
        natureCardButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/NatureList/TreeButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/NatureList/MineButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/NatureList/BushButton")
        };

        foreach (Button button in natureCardButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    /// <summary>
    ///     Initializes all individual card buttons under the villagers category and assigns them event handlers that trigger
    ///     OnSubButtonPressed when clicked.
    /// </summary>
    private void InitializeVillagerCardButtons() {
        villagerCardButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/VillagerList/VillagerButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/VillagerList/HunterButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/VillagerList/FarmerButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/VillagerList/BlacksmithButton")
        };

        foreach (Button button in villagerCardButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    private void InitializeRandomEventButtons() {
        eventCardButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/EventsList/MeteoriteButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/EventsList/FireButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/EventsList/ResourceEventButton")
        };

        foreach (Button button in eventCardButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    private void InitializePacksButtons() {
        packsButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/PacksList/MaterialPackButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/PacksList/StarterPackButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/Container/PacksList/FoodPackButton")
        };

        foreach (Button button in packsButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    /// <summary>
    ///     Sets up the texture and description for each card button in all categories
    /// </summary>
    private void InitializeDescriptions() {
        //resource cards
        cardData["WoodButton"] = (LoadCardTexture("Wood"),
            "Wood is a basic resource used for crafting and building. \nRECIPE: \n1x Tree \n1x Axe \n1x Villager");
        cardData["PlanksButton"] = (LoadCardTexture("Planks"),
            "Planks are crafted from wood and are used for crafting and building. \nRECIPE: \n2x Wood");
        cardData["StickButton"] = (LoadCardTexture("Stick"),
            "Sticks are small pieces of wood used for crafting and building. \nRECIPE: \n1x Wood \n1x Axe \n1x Villager");
        cardData["StoneButton"] = (LoadCardTexture("Stone"),
            "Stone is a basic resource used for crafting and building. \nRECIPE: \n1x Mine \n1x Villager \nOR: \n1x Meteorite \n1x Axe \n1x Villager");
        cardData["WaterButton"] = (LoadCardTexture("Water"),
            "Water is a basic resource used for crafting and building. \nRECIPE: \n 2x Water \nRandomly appears during events");
        cardData["BrickButton"] = (LoadCardTexture("Brick"),
            "Bricks are crafted from clay and are used for crafting and building. \nRECIPE: \n1x Clay \n1x Campfire");
        cardData["SandButton"] = (LoadCardTexture("Sand"),
            "Sand is a basic resource used for crafting and building. \nRECIPE: \n1x Stone \n1x Villager");
        cardData["GlassButton"] = (LoadCardTexture("Glass"),
            "Glass is crafted from sand and is used for crafting and building. \nRECIPE: \n1x Sand \n1x Campfire");
        cardData["LeafButton"] = (LoadCardTexture("Leaves"),
            "Leaves are a basic resource used for crafting and building. \nRECIPE: \n1x Tree \n1x Villager");
        cardData["ClayButton"] = (LoadCardTexture("Clay"),
            "Clay is a basic resource used for crafting and building. \nRECIPE: \n1x Sand \n1x Water");

        //tool cards
        cardData["SwordButton"] = (LoadCardTexture("Sword"),
            "A basic sword used for combat. \nRECIPE: \n2x Wood \n1x Stone");
        cardData["FishingPoleButton"] = (LoadCardTexture("FishingPole"),
            "A basic fishing pole used for fishing. \nRECIPE: \n1x Stick \n1x Stone");
        cardData["ShovelButton"] = (LoadCardTexture("Shovel"),
            "A basic shovel used for digging. \nRECIPE: \n2x Stick \n2x Stone");
        cardData["AxeButton"] = (LoadCardTexture("Axe"),
            "A basic axe used for chopping wood. \nRECIPE: \n1x Stone \n2x Stick");

        //building cards
        cardData["GreenhouseButton"] = (LoadCardTexture("Greenhouse"),
            "A greenhouse used for growing plants. \nRECIPE: \n4x Glass \n2x Brick");
        cardData["HouseButton"] = (LoadCardTexture("House"),
            "A house used for better sheltering. \nRECIPE: \n4x Stone \n2x Planks \n4x Brick");
        cardData["TentButton"] = (LoadCardTexture("Tent"), "A tent used for shelter. \nRECIPE: \n4x Leaf \n1x Wood");
        cardData["FieldButton"] = (LoadCardTexture("Field"),
            "A field used for growing crops. \nRECIPE: \n4x Sand \n2x Stone \n1x Water");
        cardData["CampfireButton"] = (LoadCardTexture("Campfire"),
            "A campfire used for cooking and warmth. \nRECIPE: \n 3x Wood \n2x Sticks \n1x Leaf");
        cardData["CookingPotButton"] = (LoadCardTexture("CookingPot"),
            "A cooking pot used for cooking food. \n2x Clay \n1x Stick");

        //food cards
        cardData["AppleButton"] =
            (LoadCardTexture("Apple"), "An apple used for food. \nRECIPE: \n1x Tree \n1x Villager");
        cardData["BerryButton"] =
            (LoadCardTexture("Berry"), "A berry used for food. \nRECIPE: \n1x Bush \n1x Villager");
        cardData["JamButton"] = (LoadCardTexture("Jam"),
            "Jam used for food. \nRECIPE: \n 1x Cooking Pot \n5x Berry \n1x Campfire");
        cardData["MeatButton"] = (LoadCardTexture("Meat"),
            "Meat used for food. \nRECIPE: \n1x Field \n1x Tree \n1x Sword \n1x Villager");
        cardData["CookedMeatButton"] = (LoadCardTexture("CookedMeat"),
            "Cooked meat used for food. \nRECIPE: \n1x Meat \n1x Campfire");
        cardData["FishButton"] = (LoadCardTexture("Fish"),
            "Fish used for food. \nRECIPE: \n1x Water \n1x Fishing Pole \n1x Villager");
        cardData["CookedFishButton"] = (LoadCardTexture("CookedFish"),
            "Cooked fish used for food. \nRECIPE: \n1x Fish \n1x Campfire");

        //nature cards
        cardData["TreeButton"] = (LoadCardTexture("Tree"),
            "A tree used for wood. \nRECIPE: \nAppears in random Events. \nCan be found in packs");
        cardData["MineButton"] = (LoadCardTexture("Mine"), "A mine used for stone. \nRECIPE \n10x Stone");
        cardData["BushButton"] = (LoadCardTexture("Bush"), "A bush used to find berries. \nRECIPE: \n6x Leaf");

        //villager cards
        cardData["VillagerButton"] = (LoadCardTexture("Villager"),
            "A villager used for crafting and building. \nRECIPE: \n2x Villager \n1x Tent or 1x House");
        cardData["HunterButton"] = (LoadCardTexture("Hunter"),
            "A hunter used for hunting. Hunts food faster than other villagers. \nRECIPE: \n1x Villager \n1x Sword");
        cardData["FarmerButton"] = (LoadCardTexture("Farmer"),
            "A farmer used for farming. Farms food faster than other villagers. \nRECIPE: \n1x Villager \n1x Shovel");
        cardData["BlacksmithButton"] = (LoadCardTexture("Blacksmith"),
            "A blacksmith used for crafting. Crafts items faster than other villagers. \nRECIPE: \n1x Villager \n1x Axe");

        //Random Event cards
        cardData["MeteoriteButton"] = (LoadCardTexture("Meteorite"),
            "As days go by there is a chance for a meteorite strike. \n\nThis cannot kill your villagers but can instead be mined with an axe to receive 10 stone.");
        cardData["FireButton"] = (LoadCardTexture("Fire"),
            "As days go by there is a chance for a fire to start. \n\nThis can kill your villagers if they are not put out in time. Stack a water card on the fire to put it out.");
        cardData["ResourceEventButton"] = (LoadCardTexture("ResourceEvent"),
            "As days go by there is a chance for a random resource event to happen. \n\nThis event randomly grants you any kind of resource card.");

        //packs
        cardData["StarterPackButton"] = (LoadPackTexture("Starter_Pack"),
            "A pack you can open up once at the start of a new game. \n\nCONTAINS: \n1x Villager \n1x Tree \n1x Bush \n1x Stone \n2x Stick");
        cardData["MaterialPackButton"] = (LoadPackTexture("Material_Pack"),
            "A pack containing 3-5 random material cards. There is a 10% chance for each of the cards in the pack to be a rare. \n\nCOMMON CARDS: \nWood, Stone, Leaf, Sand, Stick, Water, Brick \nRARE CARDS: \nPlanks, Clay, Glass ");
        cardData["FoodPackButton"] = (LoadPackTexture("Food_Pack"),
            "A pack containing 3-5 random food cards. There is a 10% chance for each of the cards in the pack to be a rare. \n\nCOMMON CARDS: \nBerry, Apple, Fish, Meat \nRARE CARDS: \nJam, Cooked Fish, Cooked Meat");
    }

    #endregion

    #region Button pressed events

    /// <summary>
    ///     handles the event when a subcategory button is pressed
    ///     Updates the cards image and description label
    /// </summary>
    /// <param name="buttonName">the name of the button that was pressed</param>
    private void OnSubButtonPressed(string buttonName) {
        if (cardData.TryGetValue(buttonName, out (Texture2D, string) data)) {
            cardImage.Texture = data.Item1;
            cardInfoLabel.Text = data.Item2;
        }

        descriptionLabel.Visible = true;
        cardInfoLabel.Visible = true;
        cardImage.Visible = true;
    }

    /// <summary>
    ///     Handles the event when a main category button is pressed
    ///     Updates the list of cards that should be shown
    /// </summary>
    /// <param name="button">the button that was pressed</param>
    private void OnMainButtonPressed(Button button) {
        foreach (KeyValuePair<Button, VBoxContainer> choice in buttons)
            if (choice.Key == button) {
                bool isNowVisible = !choice.Value.Visible;
                choice.Value.Visible = isNowVisible;
                choice.Key.Text = GetBaseButtonText(choice.Key) + (isNowVisible
                    ? " v"
                    : " >");
            } else {
                choice.Value.Visible = false;
                choice.Key.Text = GetBaseButtonText(choice.Key) + " >";
            }
    }

    /// <summary>
    ///     Handles the event when the go back button is pressed
    ///     Goes back to the previous menu
    /// </summary>
    private void OnGoBackButtonPressed() {
        ResetGuideMenu();
        menuController.GoBackToPreviousMenu();
    }

    #endregion
}