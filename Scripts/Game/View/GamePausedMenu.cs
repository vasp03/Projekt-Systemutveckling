using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class GamePausedMenu : Control {
	private MenuController MenuController => GetNode<MenuController>("/root/MenuController");

	public override void _Ready() {
		VBoxContainer buttonContainer = GetNode<VBoxContainer>("ButtonContainer");
		buttonContainer.Show();

		Button resumeButton = GetNode<Button>("ButtonContainer/ResumeButton");
		resumeButton.Pressed += OnResumeButtonPressed;

		Button optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;

		Button exitButton = GetNode<Button>("ButtonContainer/ExitToMainMenuButton");
		exitButton.Pressed += OnExitButtonPressed;
	}

	private void OnResumeButtonPressed() {
		MenuController.CloseMenus();
	}

	private void OnOptionsButtonPressed() {
		MenuController.OpenOptionsMenu();
	}

	private void OnExitButtonPressed() {
		GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/MainMenu.tscn");
		Visible = false;
	}
}