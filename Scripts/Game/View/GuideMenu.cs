using System.Collections.Generic;
using Godot;

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
    private Button foodButton;
    private Button[] foodCardButtons;
    private VBoxContainer foodList;
    private Button goBackButton;

    private MenuController menuController;
    private Button natureButton;
    private Button[] natureCardButtons;
    private VBoxContainer natureList;

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
        cardImage = GetNode<TextureRect>("TabContainer/Card Types/CTBoxContainer/ListPanel/CardImage");
        descriptionLabel = GetNode<Label>("TabContainer/Card Types/CTBoxContainer/ListPanel/DescriptionLabel");
        cardInfoLabel = GetNode<Label>("TabContainer/Card Types/CTBoxContainer/ListPanel/InfoLabel");

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
    /// <param name="CardName">The name of the card to load the image for</param>
    /// <returns>The card image or a default image if no image is found</returns>
    private Texture2D LoadCardTexture(string CardName) {
        string cardImagePath = "res://Assets/Cards/Ready To Use/" + CardName + ".png";
        string defaultPath = "res://Assets/Cards/Ready To Use/Error.png";
        if (!FileAccess.FileExists(cardImagePath)) {
            GD.PrintErr($"Card image not found: {cardImagePath}");
            return GD.Load<Texture2D>(defaultPath);
        }

        Texture2D cardTexture = GD.Load<Texture2D>(cardImagePath);

        return cardTexture;
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
        toolsButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ToolsButton");
        foodButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/FoodButton");
        buildingsButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/BuildingsButton");
        natureButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/NatureButton");
        villagersButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/VillagersButton");
        resourcesButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ResourcesButton");

        toolsButton.Pressed += () => OnMainButtonPressed(toolsButton);
        foodButton.Pressed += () => OnMainButtonPressed(foodButton);
        buildingsButton.Pressed += () => OnMainButtonPressed(buildingsButton);
        natureButton.Pressed += () => OnMainButtonPressed(natureButton);
        villagersButton.Pressed += () => OnMainButtonPressed(villagersButton);
        resourcesButton.Pressed += () => OnMainButtonPressed(resourcesButton);

        goBackButton = GetNode<Button>("GoBackButton");
        goBackButton.Pressed += OnGoBackButtonPressed;
    }

    /// <summary>
    ///     Retrieves and assigns the list containers for each card category and maps each button to its corresponding list
    ///     container.
    /// </summary>
    private void InitializeLists() {
        toolList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/ToolList");
        foodList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/FoodList");
        buildingList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/BuildingList");
        natureList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/NatureList");
        villagerList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/VillagerList");
        resourceList = GetNode<VBoxContainer>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList");

        buttons[toolsButton] = toolList;
        buttons[foodButton] = foodList;
        buttons[buildingsButton] = buildingList;
        buttons[natureButton] = natureList;
        buttons[villagersButton] = villagerList;
        buttons[resourcesButton] = resourceList;

        toolList.Visible = false;
        foodList.Visible = false;
        buildingList.Visible = false;
        natureList.Visible = false;
        villagerList.Visible = false;
        resourceList.Visible = false;
    }

    /// <summary>
    ///     Initializes all individual card buttons under the resources category and assigns them event handlers that trigger
    ///     OnSubButtonPressed when clicked.
    /// </summary>
    private void InitializeResourceCardButtons() {
        resourceCardButtons = new[] {
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/WoodButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/PlanksButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/StickButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/StoneButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/WaterButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/BrickButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/SandButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/GlassButton"),
            GetNode<Button>(
                "TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/LeafButton"), //no leaf card yet 
            GetNode<Button>(
                "TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/ClayButton") //no clay card yet 
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
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ToolList/SwordButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ToolList/FishingPoleButton"),
            GetNode<Button>(
                "TabContainer/Card Types/CTBoxContainer/ListPanel/ToolList/ShovelButton"), //no shovel card yet 
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ToolList/AxeButton")
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
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/BuildingList/GreenhouseButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/BuildingList/HouseButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/BuildingList/TentButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/BuildingList/FieldButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/BuildingList/CampfireButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/BuildingList/CookingPotButton")
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
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/FoodList/AppleButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/FoodList/BerryButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/FoodList/JamButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/FoodList/MeatButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/FoodList/CookedMeatButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/FoodList/FishButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/FoodList/CookedFishButton")
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
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/NatureList/TreeButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/NatureList/MineButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/NatureList/BushButton")
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
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/VillagerList/VillagerButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/VillagerList/HunterButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/VillagerList/FarmerButton"),
            GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/VillagerList/BlacksmithButton")
        };

        foreach (Button button in villagerCardButtons) {
            string buttonName = button.Name;
            button.Pressed += () => OnSubButtonPressed(buttonName);
        }
    }

    /// <summary>
    ///     Sets up the texture and description for each card button in all categories
    /// </summary>
    private void InitializeDescriptions() {
        //resource cards
        cardData["WoodButton"] = (LoadCardTexture("Wood"), "Wood is a basic resource used for crafting and building.");
        cardData["PlanksButton"] = (LoadCardTexture("Planks"),
            "Planks are crafted from wood and are used for crafting and building.");
        cardData["StickButton"] = (LoadCardTexture("Stick"),
            "Sticks are small pieces of wood used for crafting and building.");
        cardData["StoneButton"] =
            (LoadCardTexture("Stone"), "Stone is a basic resource used for crafting and building.");
        cardData["WaterButton"] =
            (LoadCardTexture("Water"), "Water is a basic resource used for crafting and building.");
        cardData["BrickButton"] = (LoadCardTexture("Brick"),
            "Bricks are crafted from clay and are used for crafting and building.");
        cardData["SandButton"] = (LoadCardTexture("Sand"), "Sand is a basic resource used for crafting and building.");
        cardData["GlassButton"] = (LoadCardTexture("Glass"),
            "Glass is crafted from sand and is used for crafting and building.");
        cardData["LeafButton"] =
            (LoadCardTexture("Leaves"),
                "Leaves are a basic resource used for crafting and building."); //no leaf card yet 
        cardData["ClayButton"] =
            (LoadCardTexture("Clay"), "Clay is a basic resource used for crafting and building."); //no clay card yet 

        //tool cards
        cardData["SwordButton"] = (LoadCardTexture("SwordMK1"), "A basic sword used for combat.");
        cardData["FishingPoleButton"] = (LoadCardTexture("FishingPole"), "A basic fishing pole used for fishing.");
        cardData["ShovelButton"] =
            (LoadCardTexture("Shovel"), "A basic shovel used for digging."); //no shovel card yet 
        cardData["AxeButton"] = (LoadCardTexture("Axe"), "A basic axe used for chopping wood.");

        //building cards
        cardData["GreenhouseButton"] = (LoadCardTexture("Greenhouse"), "A greenhouse used for growing plants.");
        cardData["HouseButton"] = (LoadCardTexture("House"), "A house used for better sheltering.");
        cardData["TentButton"] = (LoadCardTexture("Tent"), "A tent used for shelter.");
        cardData["FieldButton"] = (LoadCardTexture("Field"), "A field used for growing crops.");
        cardData["CampfireButton"] = (LoadCardTexture("Campfire"), "A campfire used for cooking and warmth.");
        cardData["CookingPotButton"] = (LoadCardTexture("CookingPot"), "A cooking pot used for cooking food.");

        //food cards
        cardData["AppleButton"] = (LoadCardTexture("Apple"), "An apple used for food.");
        cardData["BerryButton"] = (LoadCardTexture("Berry"), "A berry used for food.");
        cardData["JamButton"] = (LoadCardTexture("Jam"), "Jam used for food.");
        cardData["MeatButton"] = (LoadCardTexture("Meat"), "Meat used for food.");
        cardData["CookedMeatButton"] = (LoadCardTexture("CookedMeat"), "Cooked meat used for food.");
        cardData["FishButton"] = (LoadCardTexture("Fish"), "Fish used for food.");
        cardData["CookedFishButton"] = (LoadCardTexture("CookedFish"), "Cooked fish used for food.");

        //nature cards
        cardData["TreeButton"] = (LoadCardTexture("Tree"), "A tree used for wood.");
        cardData["MineButton"] = (LoadCardTexture("Mine"), "A mine used for stone.");
        cardData["BushButton"] = (LoadCardTexture("Bush"), "A bush used to find berries.");

        //villager cards
        cardData["VillagerButton"] = (LoadCardTexture("Villager"), "A villager used for crafting and building.");
        cardData["HunterButton"] = (LoadCardTexture("Hunter"), "A hunter used for hunting.");
        cardData["FarmerButton"] = (LoadCardTexture("Farmer"), "A farmer used for farming.");
        cardData["BlacksmithButton"] = (LoadCardTexture("Blacksmith"), "A blacksmith used for crafting.");
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
            GD.Print("Button pressed: " + buttonName);
        } else {
            GD.Print($"No data found for button: {buttonName}");
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
                choice.Key.Text = GetBaseButtonText(choice.Key) + (isNowVisible ? " v" : " >");
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