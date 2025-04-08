using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class MainMenu : Control {
	private Button ExitButton => GetNode<Button>("ButtonContainer/ExitButton");

	private MenuController MenuController => GetNode<MenuController>("/root/MenuController");
	private Button OptionsButton => GetNode<Button>("ButtonContainer/OptionsButton");
	private Button PlayButton => GetNode<Button>("ButtonContainer/PlayButton");

	public override void _Ready() {
		MenuController.configureWithNewMainMenuInstance(this);
		
		PlayButton.Pressed += OnPlayButtonPressed;

		OptionsButton.Pressed += OnOptionsButtonPressed;
		
		ExitButton.Pressed += OnExitButtonPressed;
	}

	private void OnPlayButtonPressed() {
		GetTree().ChangeSceneToFile("res://Scenes/mainScene.tscn");
	}

	private void OnOptionsButtonPressed() {
		MenuController.OpenOptionsMenu();
	}

	private void OnExitButtonPressed() {
		GetTree().Quit();
	}
}