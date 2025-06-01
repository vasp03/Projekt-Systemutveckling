using Godot;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Responsible for saving and loading the game settings.
/// </summary>
public partial class SettingsManager : Node {
    /// <summary>
    ///     Path to config file
    /// </summary>
    private const string CONFIG_FILE = "user://settings.cfg";

    public static SettingsManager Singleton =>
        (Engine.GetMainLoop() as SceneTree).CurrentScene.GetNode<SettingsManager>("/root/SettingsManager");

    public SoundController SoundController { get; private set; }
    public int DisplayMode { get; private set; }
    public float MusicVolume { get; private set; } = 1.0f;
    public float SfxVolume { get; private set; } = 1.0f;
    public bool CheatMode { get; private set; }

    public override void _Ready() {
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
        SoundController.Singleton.MusicVolume = volume;
        MusicVolume = volume;
        SaveConfig();
    }

    /// <summary>
    ///     sets the SFX volume based on the selected option and saves the setting.
    /// </summary>
    /// <param name="volume">The volume selected</param>
    public void SetSfxVolume(float volume) {
        SoundController.Singleton.SfxVolume = volume;
        SfxVolume = volume;
        SaveConfig();
    }

    public void SetCheatMode(bool enabled) {
        CheatMode = enabled;
        GD.Print("Cheat mode is now " + CheatMode);
        SaveConfig();
    }

    /// <summary>
    ///     Loads the saved settings from a config file.
    /// </summary>
    private void LoadConfig() {
        ConfigFile config = new();
        if (!FileAccess.FileExists(CONFIG_FILE)) return;
        Error error = config.Load(CONFIG_FILE);
        if (error != Error.Ok) return;
        DisplayMode = (int)config.GetValue("Display", "Mode", 0);
        MusicVolume = (float)config.GetValue("Audio", "MusicVolume", 1.0f);
        SfxVolume = (float)config.GetValue("Audio", "SfxVolume", 1.0f);
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
        config.SetValue("Cheat", "Enabled", CheatMode);
        Error error = config.Save(CONFIG_FILE);
        if (error != Error.Ok) GD.Print("Failed to save settings.");
    }
}