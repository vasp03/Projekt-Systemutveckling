using Godot;
using Goodot15.Scripts.Game.Controller;
using System;

public partial class GameOverMenu : Control {
    private const string LOSE_SOUND = "General Sounds/Negative Sounds/sfx_sounds_error9.wav";
    
    Sprite2D background;
    Button exitGameButton;
    Button backToMenuButton;
    MenuController menuController;
    SoundController soundController;

    public override void _Ready() {
        background = GetNode<Sprite2D>("Background");
        exitGameButton = GetNode<Button>("ExitGame");
        backToMenuButton = GetNode<Button>("BackToMenu");
        menuController = GetNode<MenuController>("/root/MenuController");
        soundController = GetNode<SoundController>("/root/SoundController");

        exitGameButton.Pressed += OnExitGameButtonPressed;
        backToMenuButton.Pressed += OnBackToMenuButtonPressed;
        
        SoundController.Singleton.PlaySound(LOSE_SOUND);
    }

    private void OnExitGameButtonPressed() {
        GetTree().Quit();
    }

    private void OnBackToMenuButtonPressed() {
        menuController.CloseMenus();
        menuController.OpenMainMenu();
        menuController.GetTree().Paused = false;
        soundController.MusicMuted = false;
    }
}
