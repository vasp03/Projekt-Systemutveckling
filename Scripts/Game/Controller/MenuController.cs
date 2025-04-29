using System;
using Godot;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Class that controls the flow of the menus in the game.
/// </summary>
public class MenuController : GameManagerBase {
	private Node MenuControllerNode;

	private static readonly Control guideMenu =
		GD.Load<PackedScene>("res://Scenes/MenuScenes/GuideMenu.tscn").Instantiate<Control>();
	private static Control mainMenu => GD.Load<PackedScene>("res://Scenes/MenuScenes/MainMenu.tscn").Instantiate<Control>();
	private static Control optionsMenu => GD.Load<PackedScene>("res://Scenes/MenuScenes/OptionsMenu.tscn").Instantiate<Control>();
	private static Control pauseMenu => GD.Load<PackedScene>("res://Scenes/MenuScenes/GamePausedMenu.tscn").Instantiate<Control>();
	
	private Control currentMenu;
	private Control previousMenu;


	public MenuController() {
		this.MenuControllerNode = new Node();
		GameController.AddChild(MenuControllerNode);
		
		OpenMainMenu();
	}

	/// <summary>
	///     Loads and opens the main menu.
	/// </summary>
	public void OpenMainMenu() {
		SwitchMenu(mainMenu);
		// if (mainMenu == null) {
		//     PackedScene packedMainMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/MainMenu.tscn");
		//     mainMenu = packedMainMenu.Instantiate() as Control;
		//     MenuControllerNode.AddChild(mainMenu);
		// }
// 
		// CurrentScene.GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/MainMenu.tscn");
		// // SwitchMenu(mainMenu);
	}

	/// <summary>
	///     Loads and opens the pause menu.
	/// </summary>
	public void OpenPauseMenu() {
		if (CurrentScene.GetTree().Paused) return;
		CurrentScene.GetTree().Paused = true;

		// if (pauseMenu == null) {
		//     PackedScene packedPauseMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/GamePausedMenu.tscn");
		//     pauseMenu = packedPauseMenu.Instantiate() as Control;
		//     MenuControllerNode.AddChild(pauseMenu);
		// }

		SwitchMenu(pauseMenu);
	}

	/// <summary>
	///     Loads and opens the options menu.
	/// </summary>
	public void OpenOptionsMenu() {
		// previousMenu = currentMenu;
		// if (optionsMenu == null) {
		//     PackedScene packedOptionsMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/OptionsMenu.tscn");
		//     optionsMenu = packedOptionsMenu.Instantiate() as Control;
		//     MenuControllerNode.AddChild(optionsMenu);
		// }

		SwitchMenu(optionsMenu);
	}

	/// <summary>
	///     Loads and opens the guide menu.
	/// </summary>
	public void OpenGuideMenu() {
		// previousMenu = currentMenu;
		// if (guideMenu == null) {
		//     PackedScene packedGuideMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/GuideMenu.tscn");
		//     guideMenu = packedGuideMenu.Instantiate() as Control;
		//     MenuControllerNode.AddChild(guideMenu);
		// }

		SwitchMenu(guideMenu);
	}

	/// <summary>
	///     Switches the current menu to the new menu and hides the previous menu.
	/// </summary>
	/// <param name="newMenu">The new menu that should be shown</param>
	private void SwitchMenu(Control newMenu) {
		try {
			this.previousMenu = currentMenu;
			this.currentMenu = newMenu;

			if (this.previousMenu is not null) MenuControllerNode.RemoveChild(previousMenu);
			if (!this.currentMenu.IsInsideTree()) {
                MenuControllerNode.AddChild(currentMenu);
			}

			currentMenu.GlobalPosition = new Vector2(1280 / 2, 720 / 2);
		}
		catch (Exception e) {
			GD.Print("stop with disposal of random objects godot");
			GD.Print(e);
		}
	}

	/// <summary>
	///     Goes back to the previous menu.
	/// </summary>
	public void GoBackToPreviousMenu() {
		if (previousMenu is not null) {
			Control menuToSwitchTo = previousMenu;
			SwitchMenu(menuToSwitchTo);
		}
	}

	/// <summary>
	///     Closes all the menus and resumes the game.
	/// </summary>
	public void CloseMenus() {
		foreach (Node menu in MenuControllerNode.GetChildren())
			if (menu is Control controlMenu && controlMenu.IsInsideTree())
				controlMenu.Visible = false;

		CurrentScene.GetTree().Paused = false;
		GameController.Visible = true;
		GameController.GetManager<DayTimeController>().SetPaused(false);
		GameController.GetManager<SoundController>().MusicMuted = true;
	}

	/// <summary>
	///     sets the GameController to a variable for the MenuController.
	/// </summary>
	/// <param name="gameController">the GameController to be set</param>
	public void SetNodeController(GameController gameController) {
		// GameController = gameController;
	}

	/// <summary>
	///     Configures the MenuController with a new instance of the MainMenu.
	/// </summary>
	/// <param name="menu">The new main menu instance to configure with</param>
	public void ConfigureWithNewMainMenuInstance(MainMenu menu) {
		// mainMenu = menu;
		// currentMenu = menu;

		CurrentScene.GetTree().Paused = false;
	}

    /// <summary>
    ///     Cleans up resources and frees the MenuController when it is removed from the scene tree.
    /// </summary>
    public override void OnUnload() {
	    MenuControllerNode.QueueFree();
	}

	public bool IsPaused() {
		return CurrentScene.GetTree().Paused;
	}
}
