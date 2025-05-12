using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Class that controls the flow of the menus in the game.
/// </summary>
public partial class MenuController : Node {
    private GameController GameController;

    public override void _Ready() {
        // mainMenu = GetParent().GetNode<Control>("MainMenu");
        currentMenu = mainMenu;
        pauseMenu = null;
        optionsMenu = null;
        guideMenu = null;
        // this.previousMenu = mainMenu;
    }

    /// <summary>
    ///     sets the GameController to a variable for the MenuController.
    /// </summary>
    /// <param name="gameController">the GameController to be set</param>
    public void SetGameController(GameController gameController) {
        GameController = gameController;
    }

    /// <summary>
    ///     Configures the MenuController with a new instance of the MainMenu.
    /// </summary>
    /// <param name="menu">The new main menu instance to configure with</param>
    public void ConfigureWithNewMainMenuInstance(MainMenu menu) {
        mainMenu = menu;
        currentMenu = menu;

        GetTree().Paused = false;
        // CallCallbacks(false);
    }

    /// <summary>
    ///     Cleans up resources and frees the MenuController when it is removed from the scene tree.
    /// </summary>
    public override void _ExitTree() {
        QueueFree();
    }

    #region Control UI fields

    private Control previousMenu;
    private Control currentMenu;

    private Control guideMenu;
    private Control mainMenu;
    private Control optionsMenu;

    private Control pauseMenu;

    #endregion Control UI fields

    #region Menu opening methods

    /// <summary>
    ///     Loads and opens the main menu.
    /// </summary>
    public void OpenMainMenu() {
        if (mainMenu is null) {
            PackedScene packedMainMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/MainMenu.tscn");
            mainMenu = packedMainMenu.Instantiate() as Control;
            AddChild(mainMenu);
        }

        GetTree().ChangeSceneToFile("res://Scenes/MenuScenes/MainMenu.tscn");
        // SwitchMenu(mainMenu);
    }

    /// <summary>
    ///     Loads and opens the pause menu.
    /// </summary>
    public void OpenPauseMenu() {
        if (GetTree().Paused) return;
        GetTree().Paused = true;
        CallPausedCallbacks(true);

        if (pauseMenu is null) {
            PackedScene packedPauseMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/GamePausedMenu.tscn");
            pauseMenu = packedPauseMenu.Instantiate() as Control;
            AddChild(pauseMenu);
        }

        SwitchMenu(pauseMenu);
    }

    /// <summary>
    ///     Loads and opens the options menu.
    /// </summary>
    public void OpenOptionsMenu() {
        previousMenu = currentMenu;
        if (optionsMenu is null) {
            PackedScene packedOptionsMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/OptionsMenu.tscn");
            optionsMenu = packedOptionsMenu.Instantiate() as Control;
            AddChild(optionsMenu);
        }

        SwitchMenu(optionsMenu);
    }

    /// <summary>
    ///     Loads and opens the guide menu.
    /// </summary>
    public void OpenGuideMenu() {
        previousMenu = currentMenu;
        if (guideMenu is null) {
            PackedScene packedGuideMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/GuideMenu.tscn");
            guideMenu = packedGuideMenu.Instantiate() as Control;
            AddChild(guideMenu);
        }

        SwitchMenu(guideMenu);
    }

    /// <summary>
    ///     Switches the current menu to the new menu and hides the previous menu.
    /// </summary>
    /// <param name="newMenu">The new menu that should be shown</param>
    private void SwitchMenu(Control newMenu) {
        if (newMenu is not null && newMenu.IsInsideTree()) {
            currentMenu = newMenu;
            newMenu.Visible = true;
            if (previousMenu is not null) previousMenu.Visible = false;
        }
    }

    /// <summary>
    ///     Goes back to the previous menu.
    /// </summary>
    public void GoBackToPreviousMenu() {
        if (previousMenu is not null && previousMenu.IsInsideTree()) {
            Control menuToSwitchTo = previousMenu;
            previousMenu = currentMenu;
            SwitchMenu(menuToSwitchTo);
        }
    }

    /// <summary>
    ///     Closes all the menus and resumes the game.
    /// </summary>
    public void CloseMenus() {
        foreach (Node menu in GetChildren())
            if (menu is Control controlMenu && controlMenu.IsInsideTree())
                controlMenu.Visible = false;

        GetTree().Paused = false;
        CallPausedCallbacks(false);

        GameController.Visible = true;
        GameController.DayTimeController.SetPaused(false);
        GameController.SoundController.MusicMuted = true;
    }

    #endregion Menu opening methods

    #region Callbacks related

    private readonly IList<IPauseCallback> pausedCallbacks = [];

    private void CallPausedCallbacks(bool isPaused) {
        if (pausedCallbacks is null) return;

        foreach (IPauseCallback callback in pausedCallbacks) callback.PauseToggle(isPaused);
    }

    public void AddPauseCallback(IPauseCallback callback) {
        pausedCallbacks.Add(callback);
    }

    public void RemovePauseCallback(IPauseCallback callback) {
        if (pausedCallbacks is not null) pausedCallbacks.Remove(callback);
    }

    #endregion Callbacks related
}