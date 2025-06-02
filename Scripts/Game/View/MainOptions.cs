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

    private OptionButton DisplayModeButton => GetNode<OptionButton>("ButtonContainer/DisplayModeButton");

    public override void _Ready() {
        InitializeReferences();
        InitializeEvents();


        musicVolumeSlider.Value = SoundControllerReference.MusicVolume;
        sfxVolumeSlider.Value = SoundControllerReference.SfxVolume;

        PopulateDisplayModeOptions();
        SetDisplayModeButton();
        SoundControllerReference.MusicVolume = SettingsManagerReference.MusicVolume;
        SoundControllerReference.SfxVolume = SettingsManagerReference.SfxVolume;
        cheatModeButton.ButtonPressed = SettingsManagerReference.CheatMode;
    }

    private void InitializeReferences() {
        musicVolumeSlider = Slider("ButtonContainer/MusicVolumeSlider");
        sfxVolumeSlider = Slider("ButtonContainer/SFXVolumeSlider");

        musicVolumePercentageLabel = Label("MusicPercentageLabel");
        sfxVolumePercentageLabel = Label("SFXPercentageLabel");

        musicVolumePercentageLabel.Text = $"{musicVolumeSlider.Value * 100:F0}%";
        sfxVolumePercentageLabel.Text = $"{sfxVolumeSlider.Value * 100:F0}%";

        cheatModeButton = GetNode<CheckButton>("ButtonContainer/CheatModeButton");
    }

    private void InitializeEvents() {
        musicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
        sfxVolumeSlider.ValueChanged += OnSfxVolumeChanged;
        DisplayModeButton.ItemSelected += OnDisplayModeSelected;
        cheatModeButton.Toggled += OnCheatModeToggled;
        GoBackButton.Pressed += OnBackButtonPressed;
    }

    /// <summary>
    ///     Handles the event for when the music volume slider value changes.
    ///     Sets the music volume
    /// </summary>
    private void OnMusicVolumeChanged(double value) {
        SettingsManagerReference.SetMusicVolume((float)value);
        GD.Print("Music volume changed to: " + SoundControllerReference.MusicVolume);

        musicVolumePercentageLabel.Text = $"{value * 100:F0}%";
    }

    /// <summary>
    ///     Handles the event for when the SFX volume slider value changes.
    ///     Sets the SFX volume
    /// </summary>
    private void OnSfxVolumeChanged(double value) {
        SettingsManagerReference.SetSfxVolume((float)value);
        GD.Print("SFX volume changed to: " + SoundControllerReference.SfxVolume);

        sfxVolumePercentageLabel.Text = $"{value * 100:F0}%";
    }

    /// <summary>
    ///     Sets the display mode to the saved setting on the game startup.
    /// </summary>
    private void SetDisplayModeButton() {
        int currentMode = SettingsManagerReference.DisplayMode;
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
    ///     Handles the event for when a display mode is selected from the dropdown menu.
    ///     Sets the display mode selected.
    /// </summary>
    private void OnDisplayModeSelected(long index) {
        SettingsManagerReference.ChangeDisplayMode((int)index);
    }

    /// <summary>
    ///     Handles event when the cheat mode button is toggled.
    /// </summary>
    /// <param name="enabled">if setting is on or off</param>
    private void OnCheatModeToggled(bool enabled) {
        SettingsManagerReference.SetCheatMode(enabled);
    }

    /// <summary>
    ///     Handles the event for when the go back button is pressed.
    ///     Goes back to the previous menu.
    /// </summary>
    private void OnBackButtonPressed() {
        MenuControllerReference.GoBackToPreviousMenu();
    }

    #region UI elements

    private Button Button(NodePath path) {
        return GetNode<Button>(path);
    }

    private Label Label(NodePath path) {
        return GetNode<Label>(path);
    }

    private HSlider Slider(NodePath path) {
        return GetNode<HSlider>(path);
    }

    private CheckButton cheatModeButton;

    private Label musicVolumePercentageLabel;

    private HSlider musicVolumeSlider;

    private Label sfxVolumePercentageLabel;
    private HSlider sfxVolumeSlider;

    private Button GoBackButton => Button("GoBackButton");

    #endregion


    #region Controller references

    private static MenuController MenuControllerReference => MenuController.Singleton;
    private static SettingsManager SettingsManagerReference => SettingsManager.Singleton;
    private static SoundController SoundControllerReference => SoundController.Singleton;

    #endregion
}