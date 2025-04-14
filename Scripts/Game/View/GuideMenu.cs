using System.Collections.Generic;
using Godot;

public partial class GuideMenu : Control {
	private VBoxContainer buildingList;
	private Button buildingsButton;
	private readonly Dictionary<Button, VBoxContainer> buttons = new();
	private Label cardDescription;

	private TextureRect cardImage;
	private Button foodButton;
	private VBoxContainer foodList;

	private Button goBackButton;
	private MenuController menuController;
	private Button natureButton;
	private VBoxContainer natureList;
	private VBoxContainer resourceList;
	private Button resourcesButton;

	private VBoxContainer toolList;

	private Button toolsButton;
	private VBoxContainer villagerList;
	private Button villagersButton;

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");

		toolsButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ToolsButton");
		foodButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/FoodButton");
		buildingsButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/BuildingsButton");
		natureButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/NatureButton");
		villagersButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/VillagersButton");
		resourcesButton = GetNode<Button>("TabContainer/Card Types/CTBoxContainer/ResourcesButton");

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

		toolsButton.Pressed += () => onButtonPressed(toolsButton);
		foodButton.Pressed += () => onButtonPressed(foodButton);
		buildingsButton.Pressed += () => onButtonPressed(buildingsButton);
		natureButton.Pressed += () => onButtonPressed(natureButton);
		villagersButton.Pressed += () => onButtonPressed(villagersButton);
		resourcesButton.Pressed += () => onButtonPressed(resourcesButton);

		goBackButton = GetNode<Button>("GoBackButton");
		goBackButton.Pressed += OnGoBackButtonPressed;
	}


	private void onButtonPressed(Button button) {
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

	private void OnGoBackButtonPressed() {
		menuController.GoBackToPreviousMenu();
	}
}