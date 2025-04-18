using Godot;

public partial class MainMenu : Control {
    private Button exitButton;

    private Goodot15.Scripts.Game.Controller.MenuController menuController;
    private Button optionsButton;
    private Button playButton;

    public override void _Ready() {
        menuController = GetNode<Goodot15.Scripts.Game.Controller.MenuController>("/root/MenuController");
        menuController.configureWithNewMainMenuInstance(this);

        playButton = GetNode<Button>("ButtonContainer/PlayButton");
        playButton.Pressed += OnPlayButtonPressed;

        optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
        optionsButton.Pressed += OnOptionsButtonPressed;

        exitButton = GetNode<Button>("ButtonContainer/ExitButton");
        exitButton.Pressed += OnExitButtonPressed;
    }

    private void OnPlayButtonPressed() {
        GetTree().ChangeSceneToFile("res://Scenes/mainScene.tscn");
    }

    private void OnOptionsButtonPressed() {
        menuController.OpenOptionsMenu();
    }

    private void OnExitButtonPressed() {
        GetTree().Quit();
    }
}