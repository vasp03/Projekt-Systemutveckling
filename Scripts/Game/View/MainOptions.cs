using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class MainOptions : Control {
	private MenuController menuController;
	
	private OptionButton resolutionButton => GetNode<OptionButton>("ButtonContainer/ResolutionButton");
	private Button goBackButton => GetNode<Button>("GoBackButton");
	
	private readonly Vector2I[] resolutions = {
		new Vector2I(1920, 1080),
		new Vector2I(1600, 900),
		new Vector2I(1280, 720)
	};

	public override void _Ready() {
		menuController = GetNode<MenuController>("/root/MenuController");
		resolutionButton.Connect("item_selected", new Callable(this, nameof(OnResolutionSelected)));
		goBackButton.Pressed += OnBackButtonPressed;
		
		PopulateResolutionOptions();
	}
	
	private void PopulateResolutionOptions() {
		resolutionButton.Clear();
		
		foreach (Vector2I res in resolutions) {
			resolutionButton.AddItem($"        {res.X}x{res.Y}");
		}
	}

	private void OnResolutionSelected(int index) {
		
		GD.Print("option selected: " + index);
		Vector2I selectedResolution = resolutions[index];
		DisplayServer.WindowSetSize(selectedResolution);
	}

	private void OnBackButtonPressed() {
		menuController.GoBackToPreviousMenu();
	}
}