using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class SettingsManager : Node {
    private const string ConfigFilePath = "user://settings.cfg";

    public int DisplayMode { get; private set; }

    public override void _Ready() {
        LoadConfig();
        ApplyDisplayMode();
    }

    public void ChangeDisplayMode(int mode) {
        DisplayMode = mode;
        ApplyDisplayMode();
        SaveConfig();
    }

    public void ApplyDisplayMode() {
        switch (DisplayMode) {
            case 0:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
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

    private void LoadConfig() {
        ConfigFile config = new();
        if (FileAccess.FileExists(ConfigFilePath)) {
            config.SetValue("Display", "Mode", DisplayMode);
            Error error = config.Load(ConfigFilePath);
            if (error == Error.Ok) DisplayMode = (int)config.GetValue("Display", "Mode", 0);
        }
    }

    private void SaveConfig() {
        ConfigFile config = new();
        config.SetValue("Display", "Mode", DisplayMode);
        Error error = config.Save(ConfigFilePath);
        if (error != Error.Ok) GD.Print("Failed to save settings.");
    }
}