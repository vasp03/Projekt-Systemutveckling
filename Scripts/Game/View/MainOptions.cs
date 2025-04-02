using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class MainOptions : Control {
	private MenuController menuController;
	private ResolutionManager resolutionManager;
	
	private OptionButton resolutionButton => GetNode<OptionButton>("ButtonContainer/ResolutionButton");
	private OptionButton displayModeButton => GetNode<OptionButton>("ButtonContainer/DisplayModeButton");
	private Button goBackButton => GetNode<Button>("GoBackButton");
	
	private readonly Vector2I[] resolutions = {
		new Vector2I(1920, 1080),
		new Vector2I(1600, 900),
		new Vector2I(1280, 720)
	};

	private readonly string[] displayModes = {
		"WINDOWED",
		"FULLSCREEN",
		"BORDERLESS WINDOWED"
	};

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		resolutionManager = GetNode<ResolutionManager>("/root/ResolutionManager");
		
		resolutionButton.Connect("item_selected", new Callable(this, nameof(OnResolutionSelected)));
		displayModeButton.Connect("item_selected", new Callable(this, nameof(OnDisplayModeSelected)));
		goBackButton.Pressed += OnBackButtonPressed;
		
		PopulateResolutionOptions();
		PopulateDisplayModeOptions();
	}
	
	private void PopulateResolutionOptions() {
		resolutionButton.Clear();
		
		foreach (Vector2I res in resolutions) {
			resolutionButton.AddItem($"         {res.X}x{res.Y}");
		}
	}

	private void PopulateDisplayModeOptions() {
		displayModeButton.Clear();

		foreach (string displayMode in displayModes) {
			displayModeButton.AddItem(displayMode);
		}
	}

	private void OnResolutionSelected(int index) {
		resolutionManager.ChangeResolution(resolutions[index]);
	}
	
	private void OnDisplayModeSelected(int index) {
		resolutionManager.ChangeDisplayMode(index);
	}

	private void OnBackButtonPressed() {
		menuController.GoBackToPreviousMenu();
	}
}