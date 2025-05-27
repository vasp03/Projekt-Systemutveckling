using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Class that controls the flow of the menus in the game.
/// </summary>
public partial class MenuController : Node {
    //  private const string CLICK_SFX = "General Sounds/Buttons/sfx_sounds_button11.wav";

    private GameController gameController;

    public static MenuController Singleton =>
        (Engine.GetMainLoop() as SceneTree).CurrentScene.GetNode<MenuController>("/root/MenuController");

    public override void _Ready() {
        // mainMenu = GetParent().GetNode<Control>("MainMenu");
        currentMenu = mainMenu;
        pauseMenu = null;
        optionsMenu = null;
        guideMenu = null;
        // this.previousMenu = mainMenu;
        // GetTree().NodeAdded += OnNodeAdded;
    }

    // private void OnNodeAdded(Node node) {
    //     if (node is Button addedButton) {
    //         // GD.Print($"New button: [{addedButton.GetPath()}]");
    //         addedButton.Pressed += AddedButtonOnPressed;
    //     }
    // }

    // private void AddedButtonOnPressed() {
    //     SoundController.Singleton.PlaySound(CLICK_SFX);
    // }

    /// <summary>
    ///     sets the GameController to a variable for the MenuController.
    /// </summary>
    /// <param name="gameController">the GameController to be set</param>
    public void SetGameController(GameController gameController) {
        this.gameController = gameController;
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
    private Control gameOverMenu;
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
        gameController.CallPausedCallbacks(true);

        if (pauseMenu is null) {
            PackedScene packedPauseMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/GamePausedMenu.tscn");
            pauseMenu = packedPauseMenu.Instantiate() as Control;
            AddChild(pauseMenu);
        }

        SwitchMenu(pauseMenu);
    }

    public void QuickOpenGuideMenu() {
        if (GetTree().Paused) return;
        GetTree().Paused = true;
        gameController.CallPausedCallbacks(true);

        previousMenu = null;
        if (guideMenu is null) {
            PackedScene packedGuideMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/GuideMenu.tscn");
            guideMenu = packedGuideMenu.Instantiate() as Control;
            AddChild(guideMenu);
        }

        guideMenu.Visible = true;
        guideMenu.ZIndex = 3000;

        SwitchMenu(guideMenu);
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

    public void OpenGameOverMenu() {
        previousMenu = currentMenu;
        if (gameOverMenu is null) {
            PackedScene packedGameOverMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/GameOverMenu.tscn");
            gameOverMenu = packedGameOverMenu.Instantiate() as Control;
            AddChild(gameOverMenu);
        }

        if (GameController.Singleton.GameEventManager.EventInstance<DayTimeEvent>() is IPausable pausable2)
            pausable2.SetPaused(true);

        GameController.Singleton.SoundController.MusicMuted = true;
        GameController.Singleton.Visible = false;
        HUD hud = GameController.Singleton.GetNodeOrNull<HUD>("HUD");
        if (hud is not null) hud.Visible = false;

        SwitchMenu(gameOverMenu);
    }

    /// <summary>
    ///     Switches the current menu to the new menu and hides the previous menu.
    /// </summary>
    /// <param name="newMenu">The new menu that should be shown</param>
    private void SwitchMenu(Control newMenu) {
        if (newMenu is not null && newMenu.IsInsideTree()) {
            currentMenu = newMenu;
            newMenu.Visible = true;
            if (previousMenu is not null && IsInstanceValid(previousMenu)) previousMenu.Visible = false;
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
        } else {
            guideMenu.Visible = false;
            GetTree().Paused = false;
            gameController.CallPausedCallbacks(false);
            if (GameController.Singleton.GameEventManager.EventInstance<DayTimeEvent>() is IPausable pausable)
                pausable.SetPaused(false);

            SoundController.Singleton.MusicMuted = false;
            GameController.Singleton.ShowHUD();

            CanvasLayer canvasLayer = GameController.Singleton.GetNode<CanvasLayer>("HUD");
            TextureButton quickGuideButton = canvasLayer.GetNode<TextureButton>("QuickGuideButton");

            quickGuideButton.Visible = true;
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
        gameController.CallPausedCallbacks(false);

        gameController.Visible = true;

        gameController.SoundController.MusicMuted = true;
    }

    #endregion Menu opening methods
}