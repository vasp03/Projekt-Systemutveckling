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
	private OptionButton displayModeButton => GetNode<OptionButton>("ButtonContainer/DisplayModeButton");
	private Button goBackButton => GetNode<Button>("GoBackButton");
	

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
		
		musicVolumePercentageLabel.Text = $"{musicVolumeSlider.Value}%";
		sfxVolumePercentageLabel.Text = $"{sfxVolumeSlider.Value}%";
		
		musicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
		sfxVolumeSlider.ValueChanged += OnSfxVolumeChanged;
		displayModeButton.Connect("item_selected", new Callable(this, nameof(OnDisplayModeSelected)));
		goBackButton.Pressed += OnBackButtonPressed;
		
		musicVolumeSlider.Value = soundController.GetMusicVolume();
		sfxVolumeSlider.Value = soundController.GetSfxVolume();
		
		PopulateDisplayModeOptions();
		SetDisplayModeButton();
	}
	
	private void OnMusicVolumeChanged(double value) {
		soundController.SetMusicVolume((float)(value/100.0f));
		GD.Print("Music volume changed to: " + soundController.GetMusicVolume());
		
		musicVolumePercentageLabel.Text = $"{value}%";
	}
	
	private void OnSfxVolumeChanged(double value) {
		soundController.SetSfxVolume((float)(value/100.0f));
		GD.Print("SFX volume changed to: " + soundController.GetSfxVolume());
		
		sfxVolumePercentageLabel.Text = $"{value}%";
	}
	
	private void SetDisplayModeButton() {
		int currentMode = settingsManager.DisplayMode;
		displayModeButton.Select(currentMode);
	}

	private void PopulateDisplayModeOptions() {
		displayModeButton.Clear();

		foreach (string displayMode in displayModes) {
			displayModeButton.AddItem(displayMode);
		}
	}
	
	private void OnDisplayModeSelected(int index) {
		settingsManager.ChangeDisplayMode(index);
	}

	private void OnBackButtonPressed() {
		menuController.GoBackToPreviousMenu();
	}
}