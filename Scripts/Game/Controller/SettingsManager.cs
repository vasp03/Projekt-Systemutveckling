using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class SettingsManager : Node{
	
	private const string ConfigFilePath = "user://settings.cfg";	
	
	private SoundController soundController;
	
	public int DisplayMode { get; private set; } = 0;
	public float MusicVolume { get; private set; } = 1.0f;
	public float SfxVolume { get; private set; } = 1.0f;
	
	public override void _Ready() {
		soundController = GetNode<SoundController>("/root/SoundController");
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
	
	public void SetMusicVolume(float volume) {
		soundController.SetMusicVolume(volume);
		MusicVolume = volume;
		SaveConfig();
	}
	
	public void SetSfxVolume(float volume) {
		soundController.SetSfxVolume(volume);
		SfxVolume = volume;
		SaveConfig();
	}

	private void LoadConfig() {
		ConfigFile config = new ConfigFile();
		if (FileAccess.FileExists(ConfigFilePath)) {
			Error error = config.Load(ConfigFilePath);
			if (error == Error.Ok) {
				DisplayMode = (int)config.GetValue("Display", "Mode", 0);
				MusicVolume = (float)config.GetValue("Audio", "MusicVolume", 1.0f);
				SfxVolume = (float)config.GetValue("Audio", "SfxVolume", 1.0f);
			}
		}
	}

	private void SaveConfig()
	{
		ConfigFile config = new ConfigFile();
		config.SetValue("Display", "Mode", DisplayMode);
		config.SetValue("Audio", "MusicVolume", MusicVolume);
		config.SetValue("Audio", "SfxVolume", SfxVolume);
		Error error = config.Save(ConfigFilePath);
		if (error != Error.Ok)
		{
			GD.Print("Failed to save settings.");
		}
	}
}