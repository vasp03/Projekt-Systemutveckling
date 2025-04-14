using Godot;
using Goodot15.Scripts.Game.Controller;

public partial class MainMenu : Control {
	
	private SoundController soundController;
	private MenuController menuController;
	private Button continueButton;
	private Button optionsButton;
	private Button playButton;
	private Button guideButton;
	private Button exitButton;
	private bool canContinue = false;

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		menuController.configureWithNewMainMenuInstance(this);
		soundController = GetNode<SoundController>("/root/SoundController");
		soundController.PlayMenuMusic();

		continueButton = GetNode<Button>("ButtonContainer/ContinueButton");
		continueButton.Pressed += OnContinueButtonPressed;

		playButton = GetNode<Button>("ButtonContainer/PlayButton");
		playButton.Pressed += OnPlayButtonPressed;

		optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;

		guideButton = GetNode<Button>("ButtonContainer/GuideButton");
		guideButton.Pressed += OnGuideButtonPressed;

		exitButton = GetNode<Button>("ButtonContainer/ExitButton");
		exitButton.Pressed += OnExitButtonPressed;

		if (!canContinue) {
			continueButton.Disabled = true;
		}
		else {
			continueButton.Disabled = false;
		}
	}

	private void OnContinueButtonPressed() {
		//continue saved game
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