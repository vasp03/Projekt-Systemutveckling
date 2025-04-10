using Godot;
using Goodot15.Scripts.Game.Controller;

public partial class MainMenu : Control {
	
	private SoundController soundController;
	private MenuController menuController;
	private Button optionsButton;
	private Button playButton;
	private Button guideButton;
	private Button exitButton;
	
	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		menuController.configureWithNewMainMenuInstance(this);
		soundController = GetNode<SoundController>("/root/SoundController");
		soundController.PlayMenuMusic();
		

		playButton = GetNode<Button>("ButtonContainer/PlayButton");
		playButton.Pressed += OnPlayButtonPressed;

		optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;
		
		guideButton = GetNode<Button>("ButtonContainer/GuideButton");
		guideButton.Pressed += OnGuideButtonPressed;

		exitButton = GetNode<Button>("ButtonContainer/ExitButton");
		exitButton.Pressed += OnExitButtonPressed;
	}

	private void OnPlayButtonPressed() {
		GetTree().ChangeSceneToFile("res://Scenes/mainScene.tscn");
		soundController.StopMusic();
	}

	private void OnOptionsButtonPressed() {
		menuController.OpenOptionsMenu();
	}
	
	private void OnGuideButtonPressed() {
		menuController.OpenGuideMenu();
	}

	private void OnExitButtonPressed() {
		GetTree().Quit();
	}
}