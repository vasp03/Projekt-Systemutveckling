using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class SellModeButton : Button {
	public GameController GameController { get; set; }

	public override void _Ready() {
		Pressed += OnButtonPressed;
		UpdateText();
	}

	private void OnButtonPressed() {
		GameController.ToggleSellMode();
		UpdateText();
	}

	private void UpdateText() {
		Text = GameController.SellModeActive ? "Sell Mode: ON" : "Sell Mode: OFF";
	}
}
