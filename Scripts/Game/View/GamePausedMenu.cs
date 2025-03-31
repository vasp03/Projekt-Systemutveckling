using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class GamePausedMenu : Control {
	
	private MenuController menuController;
	
	public override void _Ready()
	{
		menuController = GetNode<MenuController>("/root/MenuController");
		
		VBoxContainer buttonContainer = GetNode<VBoxContainer>("ButtonContainer");
		buttonContainer.Show();
		
		Button resumeButton = GetNode<Button>("ButtonContainer/ResumeButton");
		resumeButton.Pressed += OnResumeButtonPressed;

		Button optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;

		Button exitButton = GetNode<Button>("ButtonContainer/ExitToMainMenuButton");
		exitButton.Pressed += OnExitButtonPressed;
	}
	
	private void OnResumeButtonPressed()
	{
		menuController.CloseMenus();
	}

	private void OnOptionsButtonPressed()
	{
		menuController.OpenOptionsMenu();
	}

	private void OnExitButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/MainMenu.tscn");
		
	}
}