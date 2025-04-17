using Godot;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Class that manages and saves the game settings.
/// </summary>
public partial class SettingsManager : Node {
    private const string ConfigFilePath = "user://settings.cfg";

    private SoundController soundController;

    public int DisplayMode { get; private set; }
    public float MusicVolume { get; private set; } = 1.0f;
    public float SfxVolume { get; private set; } = 1.0f;

    public override void _Ready() {
        soundController = GetNode<SoundController>("/root/SoundController");
        LoadConfig();
        ApplyDisplayMode();
    }

    /// <summary>
    ///     Changes display mode based on the selected option and saves the setting.
    /// </summary>
    /// <param name="mode">the display mode selected</param>
    public void ChangeDisplayMode(int mode) {
        DisplayMode = mode;
        ApplyDisplayMode();
        SaveConfig();
    }

    /// <summary>
    ///     Applies the display mode settings to the game window.
    /// </summary>
    private void ApplyDisplayMode() {
        switch (DisplayMode) {
            case 0:
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                break;
            case 1:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                break;
            case 2:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                DisplayServer.WindowSetSize(DisplayServer.ScreenGetSize());
                DisplayServer.WindowSetPosition(Vector2I.Zero);
                break;
        }
    }

    /// <summary>
    ///     sets the music volume based on the selected option and saves the setting.
    /// </summary>
    /// <param name="volume">The volume selected</param>
    public void SetMusicVolume(float volume) {
        soundController.MusicVolume = volume;
        MusicVolume = volume;
        SaveConfig();
    }

    /// <summary>
    ///     sets the SFX volume based on the selected option and saves the setting.
    /// </summary>
    /// <param name="volume">The volume selected</param>
    public void SetSfxVolume(float volume) {
        soundController.SfxVolume = volume;
        SfxVolume = volume;
        SaveConfig();
    }

    /// <summary>
    ///     Loads the saved settings from a config file.
    /// </summary>
    private void LoadConfig() {
        ConfigFile config = new();
        if (FileAccess.FileExists(ConfigFilePath)) {
            Error error = config.Load(ConfigFilePath);
            if (error == Error.Ok) {
                DisplayMode = (int)config.GetValue("Display", "Mode", 0);
                MusicVolume = (float)config.GetValue("Audio", "MusicVolume", 1.0f);
                SfxVolume = (float)config.GetValue("Audio", "SfxVolume", 1.0f);
            }
        }
    }

    /// <summary>
    ///     Saves current settings to a config file.
    ///     Creates a new config file if it doesn't exist.
    /// </summary>
    private void SaveConfig() {
        ConfigFile config = new();
        config.SetValue("Display", "Mode", DisplayMode);
        config.SetValue("Audio", "MusicVolume", MusicVolume);
        config.SetValue("Audio", "SfxVolume", SfxVolume);
        Error error = config.Save(ConfigFilePath);
        if (error != Error.Ok) GD.Print("Failed to save settings.");
    }
}