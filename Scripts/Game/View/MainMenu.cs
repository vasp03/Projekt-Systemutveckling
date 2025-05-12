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
    private Controller.MenuController menuController;
    private Button optionsButton;
    private Button playButton;

    private SoundController soundController;

    public override void _Ready() {
        menuController = GetNode<Controller.MenuController>("/root/MenuController");
        menuController.ConfigureWithNewMainMenuInstance(this);
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
        GetTree().ChangeSceneToFile("res://Scenes/mainScene.tscn");
        soundController.StopMusic();
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
        GetTree().Quit();
    }
}