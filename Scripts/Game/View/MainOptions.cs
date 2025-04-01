using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class MainOptions : Control {
	private MenuController menuController;

	private Button goBackButton => GetNode<Button>("ButtonContainer/GoBackButton");

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");

		goBackButton.ButtonDown += OnBackButtonPressed;
	}

	private void OnBackButtonPressed() {
		menuController.GoBackToPreviousMenu();
	}
}