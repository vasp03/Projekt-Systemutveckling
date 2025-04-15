using System.Collections.Generic;
using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class GuideMenu : Control {
	
	private readonly Dictionary<Button, VBoxContainer> buttons = new();
	private readonly Dictionary<string, (Texture2D, string)> cardData = new();
	
	private MenuController menuController;
	
	private TextureRect cardImage;
	private Label descriptionLabel;
	private Label cardInfoLabel;
	
	private VBoxContainer resourceList;
	private VBoxContainer toolList;
	private VBoxContainer buildingList;
	private VBoxContainer foodList;
	private VBoxContainer natureList;
	private VBoxContainer villagerList;
	
	private Button resourcesButton;
	private Button toolsButton;
	private Button buildingsButton;
	private Button foodButton;
	private Button natureButton;
	private Button villagersButton;
	private Button goBackButton;

	private Button[] resourceCardButtons;
	private Button[] toolCardButtons;
	private Button[] buildingCardButtons;
	private Button[] foodCardButtons;
	private Button[] natureCardButtons;
	private Button[] villagerCardButtons;
	
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
	}

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
	
	private void InitializeResourceCardButtons() {
		resourceCardButtons = new Button[] {
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/WoodButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/PlanksButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/StickButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/StoneButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/WaterButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/BrickButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/SandButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/GlasButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/LeafButton"),
			GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ListPanel/ResourceList/ClayButton")
		};

		foreach (Button button in resourceCardButtons) {
			string buttonName = button.Name;
			button.Pressed += () => OnSubButtonPressed(buttonName);
		}
	}

	private void InitializeDescriptions() {
		cardData["WoodButton"] = (LoadCardTexture("Wood"), "Wood is a basic resource used for crafting and building.");
		cardData["PlanksButton"] = (LoadCardTexture("Planks"), "Planks are crafted from wood and are used for crafting and building.");
		cardData["StickButton"] = (LoadCardTexture("Stick"), "Sticks are small pieces of wood used for crafting and building.");
		cardData["StoneButton"] = (LoadCardTexture("Stone"), "Stone is a basic resource used for crafting and building.");
		
		
	}

	private Texture2D LoadCardTexture(string CardName) {
		return GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/" + CardName + ".png");
	}

	private void OnSubButtonPressed(string buttonName) {
		if (cardData.TryGetValue(buttonName, out (Texture2D, string) data)) {
			cardImage.Texture = data.Item1;
			cardInfoLabel.Text = data.Item2;
		}
		else {
			GD.Print($"No data found for button: {buttonName}");
		}
		
		descriptionLabel.Visible = true;
		cardInfoLabel.Visible = true;
		cardImage.Visible = true;
	}

	private void OnMainButtonPressed(Button button) {
		foreach (KeyValuePair<Button, VBoxContainer> choice in buttons) {
			if (choice.Key == button) {
				bool isNowVisible = !choice.Value.Visible;
				choice.Value.Visible = isNowVisible;
				choice.Key.Text = GetBaseButtonText(choice.Key) + (isNowVisible ? " v" : " >");	
			}
			else {
				choice.Value.Visible = false;
				choice.Key.Text = GetBaseButtonText(choice.Key) + " >";
			}
		}
	}
	
	private string GetBaseButtonText(Button button) {
		string text = button.Text;
		int arrowIndex = text.IndexOf(">");
		if (arrowIndex == -1) {
			arrowIndex = text.LastIndexOf('v');
		}
		
		if (arrowIndex != -1) {
			return text.Substring(0, arrowIndex).Trim();
		}
		
		return text;
		
	}
	
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

	private void OnGoBackButtonPressed() {
		ResetGuideMenu();
		menuController.GoBackToPreviousMenu();
	}
}