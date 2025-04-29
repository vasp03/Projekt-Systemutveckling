using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

/// <summary>
///     Class representing the main menu of the game.
/// </summary>
public partial class MainMenu : Control {
	private readonly bool canContinue = false;
	private Button continueButton;
	private Button exitButton;
	private Button guideButton;
	private Button optionsButton;
	private Button playButton;

    public GameController GameController => IGameManager.GameControllerSingleton;
    private SoundController soundController => GameController.GetManager<SoundController>();
    private MenuController menuController => GameController.GetManager<MenuController>();

	public override void _Ready() {
		soundController.PlayMenuMusic();

		continueButton = GetNode<Button>("Node/ButtonContainer/ContinueButton");
		continueButton.Pressed += OnContinueButtonPressed;

		playButton = GetNode<Button>("Node/ButtonContainer/PlayButton");
		playButton.Pressed += OnPlayButtonPressed;

		optionsButton = GetNode<Button>("Node/ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;

		guideButton = GetNode<Button>("Node/ButtonContainer/GuideButton");
		guideButton.Pressed += OnGuideButtonPressed;

		exitButton = GetNode<Button>("Node/ButtonContainer/ExitButton");
		exitButton.Pressed += OnExitButtonPressed;

		if (!canContinue)
			continueButton.Disabled = true;
		else
			continueButton.Disabled = false;
	}

	/// <summary>
	///     Handles the button press event for the continue button.
	///     Continues the saved game if available.
	/// </summary>
	private void OnContinueButtonPressed() {
		//continue saved game
	}

	/// <summary>
	///     Handles the button press event for the play button.
	///     Starts new game.
	/// </summary>
	private void OnPlayButtonPressed() {
        GetTree().Paused = false;
        soundController.StopMusic();
        this.QueueFree();
		GetTree().ChangeSceneToFile("res://Scenes/mainScene.tscn");
	}

	/// <summary>
	///     Handles the button press event for the options button.
	///     Opens the options menu.
	/// </summary>
	private void OnOptionsButtonPressed() {
		menuController.OpenOptionsMenu();
	}

	/// <summary>
	///     Handles the button press event for the guide button.
	///     Opens the guide menu.
	/// </summary>
	private void OnGuideButtonPressed() {
		menuController.OpenGuideMenu();
	}

	/// <summary>
	///     Handles the button press event for the exit button.
	///     Closes the game.
	/// </summary>
	private void OnExitButtonPressed() {
        GetTree().CurrentScene.QueueFree();
        GetTree().Quit();
    }
}
