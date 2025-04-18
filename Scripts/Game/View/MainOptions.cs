using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class MainOptions : Control {
    private readonly string[] displayModes = {
        "WINDOWED",
        "FULLSCREEN",
        "BORDERLESS WINDOWED"
    };

    private MenuController menuController;
    private SettingsManager settingsManager;

    private OptionButton displayModeButton => GetNode<OptionButton>("ButtonContainer/DisplayModeButton");
    private Button goBackButton => GetNode<Button>("GoBackButton");

    public override void _Ready() {
        menuController = GetNode<MenuController>("/root/MenuController");
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");

        displayModeButton.Connect("item_selected", new Callable(this, nameof(OnDisplayModeSelected)));
        goBackButton.Pressed += OnBackButtonPressed;
        PopulateDisplayModeOptions();
        SetDisplayModeButton();
    }

    private void SetDisplayModeButton() {
        int currentMode = settingsManager.DisplayMode;
        displayModeButton.Select(currentMode);
    }

    private void PopulateDisplayModeOptions() {
        displayModeButton.Clear();

        foreach (string displayMode in displayModes) displayModeButton.AddItem(displayMode);
    }

    private void OnDisplayModeSelected(int index) {
        settingsManager.ChangeDisplayMode(index);
    }

    private void OnBackButtonPressed() {
        menuController.GoBackToPreviousMenu();
    }
}