using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class MainOptions : Control {
	private MenuController MenuController => GetNode<MenuController>("/root/MenuController");

	private Button GoBackButton => GetNode<Button>("ButtonContainer/GoBackButton");

	public override void _Ready() {
		GoBackButton.ButtonDown += OnBackButtonPressed;
	}

	private void OnBackButtonPressed() {
		MenuController.GoBackToPreviousMenu();
	}
}