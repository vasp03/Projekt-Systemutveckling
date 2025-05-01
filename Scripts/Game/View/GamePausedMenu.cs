using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

/// <summary>
///     Class representing the pause menu.
/// </summary>
public partial class GamePausedMenu : Control {
    private MenuManager MenuManager => IGameManager.GameControllerSingleton.GetManager<MenuManager>();

    public override void _Ready() {
        VBoxContainer buttonContainer = GetNode<VBoxContainer>("Node/ButtonContainer");
        buttonContainer.Show();

        Button resumeButton = GetNode<Button>("Node/ButtonContainer/ResumeButton");
        resumeButton.Pressed += OnResumeButtonPressed;

        Button guideButton = GetNode<Button>("Node/ButtonContainer/GuideButton");
        guideButton.Pressed += OnGuideButtonPressed;

        Button optionsButton = GetNode<Button>("Node/ButtonContainer/OptionsButton");
        optionsButton.Pressed += OnOptionsButtonPressed;

        Button exitButton = GetNode<Button>("Node/ButtonContainer/ExitToMainMenuButton");
        exitButton.Pressed += OnExitButtonPressed;
    }

    /// <summary>
    ///     Handles the button press event for the resume button.
    ///     Closes all the menus and resumes the game.
    /// </summary>
    private void OnResumeButtonPressed() {
        MenuManager.CloseMenus();
    }

    /// <summary>
    ///     Handles the button press event for the guide button.
    ///     Opens the guide menu.
    /// </summary>
    private void OnGuideButtonPressed() {
        MenuManager.OpenGuideMenu();
    }

    /// <summary>
    ///     Handles the button press event for the options button.
    ///     Opens the options menu.
    /// </summary>
    private void OnOptionsButtonPressed() {
        MenuManager.OpenOptionsMenu();
    }

    /// <summary>
    ///     Handles the button press event for the exit button.
    ///     Exits the game and returns to the main menu.
    /// </summary>
    private void OnExitButtonPressed() {
        // Await is required to synchronize scene change
        // await ToSignal(CurrentScene.GetTree(), SceneTree.SignalName.ProcessFrame);
        ChangeSceneDeferred();
        // CallDeferred(nameof(ChangeSceneDeferred));
        // menuController.OpenMainMenu();
    }

    /// <summary>
    ///     Changes the scene through deferred action.
    /// </summary>
    private void ChangeSceneDeferred() {
        GetTree().CurrentScene.Free();
        Error e = GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/MainMenu.tscn");
        Visible = false;
    }
}