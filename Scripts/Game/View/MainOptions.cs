using Godot;
using System;

public partial class MainOptions : Control {
	public override void _Ready() {
		Button goBackButton = GetNode<Button>("ButtonContainer/GoBackButton");
		goBackButton.Pressed += OnBackButtonPressed;
	}

	private void OnBackButtonPressed() {
		GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/MainMenu.tscn");
	}
}
