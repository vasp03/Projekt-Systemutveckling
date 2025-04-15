using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class MainOptions : Control {
	private MenuController menuController;
	private SettingsManager settingsManager;
	private SoundController soundController;
	
	private HSlider musicVolumeSlider;
	private HSlider sfxVolumeSlider;
	private Label musicVolumePercentageLabel;
	private Label sfxVolumePercentageLabel;
	private OptionButton DisplayModeButton => GetNode<OptionButton>("ButtonContainer/DisplayModeButton");
	private Button GoBackButton => GetNode<Button>("GoBackButton");
	

	private readonly string[] displayModes = {
		"WINDOWED",
		"FULLSCREEN",
		"BORDERLESS WINDOWED"
	};

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		soundController = GetNode<SoundController>("/root/SoundController");
		
		musicVolumeSlider = GetNode<HSlider>("ButtonContainer/MusicVolumeSlider");
		sfxVolumeSlider = GetNode<HSlider>("ButtonContainer/SFXVolumeSlider");
		
		musicVolumePercentageLabel = GetNode<Label>("MusicPercentageLabel");
		sfxVolumePercentageLabel = GetNode<Label>("SFXPercentageLabel");
		
		musicVolumePercentageLabel.Text = $"{(musicVolumeSlider.Value * 100):F0}%";
		sfxVolumePercentageLabel.Text = $"{(sfxVolumeSlider.Value * 100):F0}%";
		
		musicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
		sfxVolumeSlider.ValueChanged += OnSfxVolumeChanged;
		DisplayModeButton.Connect("item_selected", new Callable(this, nameof(OnDisplayModeSelected)));
		GoBackButton.Pressed += OnBackButtonPressed;
		
		musicVolumeSlider.Value = soundController.GetMusicVolume();
		sfxVolumeSlider.Value = soundController.GetSfxVolume();
		
		PopulateDisplayModeOptions();
		SetDisplayModeButton();
		soundController.SetMusicVolume(settingsManager.MusicVolume);
		soundController.SetSfxVolume(settingsManager.SfxVolume);
	}
	
	private void OnMusicVolumeChanged(double value) {
		settingsManager.SetMusicVolume((float)value);
		GD.Print("Music volume changed to: " + soundController.GetMusicVolume());
		
		musicVolumePercentageLabel.Text = $"{(value*100):F0}%";
	}
	
	private void OnSfxVolumeChanged(double value) {
		settingsManager.SetSfxVolume((float)value);
		GD.Print("SFX volume changed to: " + soundController.GetSfxVolume());
		
		sfxVolumePercentageLabel.Text = $"{(value * 100):F0}%";
	}
	
	private void SetDisplayModeButton() {
		int currentMode = settingsManager.DisplayMode;
		DisplayModeButton.Select(currentMode);
	}

	private void PopulateDisplayModeOptions() {
		DisplayModeButton.Clear();

		foreach (string displayMode in displayModes) {
			DisplayModeButton.AddItem(displayMode);
		}
	}
	
	private void OnDisplayModeSelected(int index) {
		settingsManager.ChangeDisplayMode(index);
	}

	private void OnBackButtonPressed() {
		menuController.GoBackToPreviousMenu();
	}
}