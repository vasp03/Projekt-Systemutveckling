using System;
using Godot;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Class that controls the flow of the menus in the game.
/// </summary>
public class MenuManager : GameManagerBase {
    private readonly Node MenuControllerNode;

    private Control currentMenu;
    private Control previousMenu;

    public MenuManager() {
        MenuControllerNode = new Node();
        GameController.AddChild(MenuControllerNode);

        OpenMainMenu();
    }

    private readonly static Control guideMenu =
        GD.Load<PackedScene>("res://Scenes/MenuScenes/GuideMenu.tscn").Instantiate<Control>();

    private readonly static Control mainMenu =
        GD.Load<PackedScene>("res://Scenes/MenuScenes/MainMenu.tscn").Instantiate<Control>();

    private readonly static Control optionsMenu =
        GD.Load<PackedScene>("res://Scenes/MenuScenes/OptionsMenu.tscn").Instantiate<Control>();

    private readonly static Control pauseMenu =
        GD.Load<PackedScene>("res://Scenes/MenuScenes/GamePausedMenu.tscn").Instantiate<Control>();

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
            previousMenu = currentMenu;
            currentMenu = newMenu;

            if (!currentMenu.IsInsideTree()) MenuControllerNode.AddChild(currentMenu);
            
            if (previousMenu is not null) MenuControllerNode.RemoveChild(previousMenu);
            currentMenu.Visible = true;
            

            currentMenu.GlobalPosition = new Vector2(GameController.GetWindow().Size[0]/2, GameController.GetWindow().Size[1]/2);
            
            (GameController.GetTree().CurrentScene as Node2D).Visible = false;
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
        } else {
            // Menu is null; So it is likely the game we need to refocus
            (GameController.GetTree().CurrentScene as Node2D).Visible = true;
        }
    }

    /// <summary>
    ///     Closes all the menus and resumes the game.
    /// </summary>
    public void CloseMenus() {
        currentMenu.Visible = false;
        
        foreach (Node menu in MenuControllerNode.GetChildren())
            if (menu is Control controlMenu)
                controlMenu.Visible = false;

        CurrentScene.GetTree().Paused = false;
        (CurrentScene as Node2D).Visible = true;
        GameController.GetManager<DayTimeManager>().SetPaused(false);
        GameController.GetManager<SoundManager>().MusicMuted = true;
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

    public void ClearPreviousMenu() {
        previousMenu = null;
    }
}