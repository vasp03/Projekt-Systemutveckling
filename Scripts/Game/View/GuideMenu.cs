using Godot;
using System;

public partial class GuideMenu : Control {
	private MenuController menuController;
	
	private Button goBackButton;

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		goBackButton = GetNode<Button>("GoBackButton");
		goBackButton.Pressed += OnGoBackButtonPressed;
	}
	
	private void OnGoBackButtonPressed() {
		menuController.GoBackToPreviousMenu();
	}
}
