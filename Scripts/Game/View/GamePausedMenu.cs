using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

/// <summary>
///     Class representing the pause menu.
/// </summary>
public partial class GamePausedMenu : Control {
	private VBoxContainer buttonContainer;

	private CanvasLayer exitConfirmationBox;
	private MenuController menuController;
	private SoundController soundController;

	private Button Button(NodePath path) {
		return GetNode<Button>(path);
	}

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		soundController = GetNode<SoundController>("/root/SoundController");

		buttonContainer = GetNode<VBoxContainer>("ButtonContainer");
		buttonContainer.Show();

		Button resumeButton = Button("ButtonContainer/ResumeButton");
		resumeButton.Pressed += OnResumeButtonPressed;

		Button guideButton = Button("ButtonContainer/GuideButton");
		guideButton.Pressed += OnGuideButtonPressed;

		Button optionsButton = Button("ButtonContainer/OptionsButton");
		optionsButton.Pressed += OnOptionsButtonPressed;

		Button exitButton = Button("ButtonContainer/ExitToMainMenuButton");
		exitButton.Pressed += OnExitButtonPressed;

		exitConfirmationBox = GetNode<CanvasLayer>("ExitConfirmation");

		Button yesButton = exitConfirmationBox.GetNode<Button>("YesButton");
		yesButton.Pressed += () => OnConfirmationButtonPressed(1);

		Button noButton = exitConfirmationBox.GetNode<Button>("NoButton");
		noButton.Pressed += () => OnConfirmationButtonPressed(0);
	}

	/// <summary>
	///     Handles the button press event for the resume button.
	///     Closes all the menus and resumes the game.
	/// </summary>
	private void OnResumeButtonPressed() {
		menuController.CloseMenus();
		soundController.ToggleMusicMuted();
		GameController.Singleton.ShowHUD();
		GameController.Singleton.Visible = true;
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
	///     Opens the exit confirmation box.
	/// </summary>
	private void OnExitButtonPressed() {
		buttonContainer.Visible = false;
		exitConfirmationBox.Visible = true;
	}

	/// <summary>
	///     handles the button press event for the confirmation buttons. Either exits to main menu or closes the confirmation
	///     box.
	/// </summary>
	/// <param name="choice">0 = No, cancel the exit. 1 = Yes, exit to main menu</param>
	private void OnConfirmationButtonPressed(int choice) {
		switch (choice) {
			case 0:
				exitConfirmationBox.Visible = false;
				buttonContainer.Visible = true;
				break;
			case 1:
				exitConfirmationBox.Visible = false;
				buttonContainer.Visible = true;
				ChangeToMainMenu();
				soundController.MusicMuted = false;
				soundController.PlayMenuMusic();
				soundController.StopAllAmbiance();
				break;
		}
	}

	/// <summary>
	///     Changes the scene through deferred action.
	/// </summary>
	private void ChangeToMainMenu() {
		GetTree().CurrentScene.Free();
		GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/MainMenu.tscn");
		Visible = false;
	}
}
