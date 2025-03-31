using Godot;
using System;

public partial class MainOptions : Control {
	
	private MenuController menuController;
	
	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		
		Button goBackButton = GetNode<Button>("ButtonContainer/GoBackButton");
		goBackButton.Pressed += OnBackButtonPressed;
	}

	private void OnBackButtonPressed() {
		menuController.GoBackToPreviousMenu();
	}
}
