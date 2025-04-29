using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

/// <summary>
///     Class representing the options menu.
/// </summary>
public partial class MainOptions : Control {
    private readonly string[] displayModes = {
        "WINDOWED",
        "FULLSCREEN",
        "BORDERLESS WINDOWED"
    };

    private MenuController menuController;
    private Label musicVolumePercentageLabel;

    private HSlider musicVolumeSlider;
    private SettingsManager settingsManager;
    private Label sfxVolumePercentageLabel;
    private HSlider sfxVolumeSlider;
    private SoundController soundController;
    private OptionButton DisplayModeButton => GetNode<OptionButton>("Node/ButtonContainer/DisplayModeButton");
    private Button GoBackButton => GetNode<Button>("Node/GoBackButton");

    public override void _Ready() {
        GameController gameController = IGameManager.GameControllerSingleton;
        menuController = gameController.GetManager<MenuController>();
        settingsManager = gameController.GetManager<SettingsManager>();
        soundController = gameController.GetManager<SoundController>();

        musicVolumeSlider = GetNode<HSlider>("Node/ButtonContainer/MusicVolumeSlider");
        sfxVolumeSlider = GetNode<HSlider>("Node/ButtonContainer/SFXVolumeSlider");

        musicVolumePercentageLabel = GetNode<Label>("Node/MusicPercentageLabel");
        sfxVolumePercentageLabel = GetNode<Label>("Node/SFXPercentageLabel");

        musicVolumePercentageLabel.Text = $"{musicVolumeSlider.Value * 100:F0}%";
        sfxVolumePercentageLabel.Text = $"{sfxVolumeSlider.Value * 100:F0}%";

        musicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
        sfxVolumeSlider.ValueChanged += OnSfxVolumeChanged;
        DisplayModeButton.Connect("item_selected", new Callable(this, nameof(OnDisplayModeSelected)));
        GoBackButton.Pressed += OnBackButtonPressed;

        musicVolumeSlider.Value = soundController.MusicVolume;
        sfxVolumeSlider.Value = soundController.SfxVolume;

        PopulateDisplayModeOptions();
        SetDisplayModeButton();
        soundController.MusicVolume = settingsManager.MusicVolume;
        soundController.SfxVolume = settingsManager.SfxVolume;
    }

    /// <summary>
    ///     Handles the event for when the music volume slider value changes.
    ///     Sets the music volume
    /// </summary>
    private void OnMusicVolumeChanged(double value) {
        settingsManager.SetMusicVolume((float)value);
        GD.Print("Music volume changed to: " + soundController.MusicVolume);

        musicVolumePercentageLabel.Text = $"{value * 100:F0}%";
    }

    /// <summary>
    ///     Handles the event for when the SFX volume slider value changes.
    ///     Sets the SFX volume
    /// </summary>
    private void OnSfxVolumeChanged(double value) {
        settingsManager.SetSfxVolume((float)value);
        GD.Print("SFX volume changed to: " + soundController.SfxVolume);

        sfxVolumePercentageLabel.Text = $"{value * 100:F0}%";
    }

    /// <summary>
    ///     Sets the display mode to the saved setting on the game startup.
    /// </summary>
    private void SetDisplayModeButton() {
        int currentMode = settingsManager.DisplayMode;
        DisplayModeButton.Select(currentMode);
    }

    /// <summary>
    ///     Populates the display mode settings drop down menu with options.
    /// </summary>
    private void PopulateDisplayModeOptions() {
        DisplayModeButton.Clear();

        foreach (string displayMode in displayModes) DisplayModeButton.AddItem(displayMode);
    }

    /// <summary>
    ///     Handles the event for when a display mode is selected from the drop down menu.
    ///     Sets the display mode selected.
    /// </summary>
    private void OnDisplayModeSelected(int index) {
        settingsManager.ChangeDisplayMode(index);
    }

    /// <summary>
    ///     Handles the event for when the go back button is pressed.
    ///     Goes back to the previous menu.
    /// </summary>
    private void OnBackButtonPressed() {
        menuController.GoBackToPreviousMenu();
    }
}