using Godot;
using Goodot15.Scripts.Game.Controller;

public partial class GameOverMenu : Control {
    private Sprite2D background;
    private Button backToMenuButton;
    private Button exitGameButton;
    private MenuController menuController;
    private SoundController soundController;

    public override void _Ready() {
        background = GetNode<Sprite2D>("Background");
        exitGameButton = GetNode<Button>("ExitGame");
        backToMenuButton = GetNode<Button>("BackToMenu");
        menuController = GetNode<MenuController>("/root/MenuController");
        soundController = GetNode<SoundController>("/root/SoundController");

        exitGameButton.Pressed += OnExitGameButtonPressed;
        backToMenuButton.Pressed += OnBackToMenuButtonPressed;
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