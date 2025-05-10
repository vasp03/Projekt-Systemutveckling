using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

/// <summary>
///     Class representing the pause menu.
/// </summary>
public partial class GamePausedMenu : Control {
	private MenuController menuController;
	private SoundController soundController;
    private GameController gameController;

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		soundController = GetNode<SoundController>("/root/SoundController");
        gameController = GetNode<GameController>("/root/GameController");

		VBoxContainer buttonContainer = GetNode<VBoxContainer>("ButtonContainer");
		buttonContainer.Show();

		Button resumeButton = GetNode<Button>("ButtonContainer/ResumeButton");
		resumeButton.Pressed += OnResumeButtonPressed;

		Button guideButton = GetNode<Button>("ButtonContainer/GuideButton");
		guideButton.Pressed += OnGuideButtonPressed;

		Button optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;

		Button exitButton = GetNode<Button>("ButtonContainer/ExitToMainMenuButton");
		exitButton.Pressed += OnExitButtonPressed;
	}

	/// <summary>
	///     Handles the button press event for the resume button.
	///     Closes all the menus and resumes the game.
	/// </summary>
	private void OnResumeButtonPressed() {
		menuController.CloseMenus();
		soundController.ToggleMusicMuted();
		gameController.ShowHUD();
	}

	/// <summary>
	///     Handles the button press event for the guide button.
	///     Opens the guide menu.
	/// </summary>
	private void OnGuideButtonPressed() {
		menuController.OpenGuideMenu();
	}

	/// <summary>
	///     Handles the button press event for the options button.
	///     Opens the options menu.
	/// </summary>
	private void OnOptionsButtonPressed() {
		menuController.OpenOptionsMenu();
	}

	/// <summary>
	///     Handles the button press event for the exit button.
	///     Exits the game and returns to the main menu.
	/// </summary>
	private void OnExitButtonPressed() {
		// Await is required to synchronize scene change
		// await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		CallDeferred(nameof(ChangeSceneDeferred));
		soundController.ToggleMusicMuted();
		soundController.PlayMenuMusic();
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
