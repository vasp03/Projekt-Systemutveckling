using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class MainMenu : Control {
	public override void _Ready()
	{
		Button playButton = GetNode<Button>("ButtonContainer/PlayButton");
		playButton.Pressed += OnPlayButtonPressed;

		Button optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;

		Button exitButton = GetNode<Button>("ButtonContainer/ExitButton");
		exitButton.Pressed += OnExitButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/mainScene.tscn");
	}

	private void OnOptionsButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/OptionsMenu.tscn");
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}