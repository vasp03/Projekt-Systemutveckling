using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class GamePausedMenu : Control
{
	public override void _Ready()
	{
		VBoxContainer buttonContainer = GetNode<VBoxContainer>("ButtonContainer");
		buttonContainer.Show();
		
		Button resumeButton = GetNode<Button>("ButtonContainer/ResumeButton");
		resumeButton.Pressed += OnResumeButtonPressed;

		Button optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;

		Button exitButton = GetNode<Button>("ButtonContainer/ExitButton");
		exitButton.Pressed += OnExitButtonPressed;
	}

	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("Pause")) {
			GetTree().Paused = true;
		}
	}

	private void OnResumeButtonPressed()
	{
		GetTree().Paused = false;
		Visible = false;
	}

	private void OnOptionsButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/OptionsMenu.tscn");
	}

	private void OnExitButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/MainMenu.tscn");
		
	}
}