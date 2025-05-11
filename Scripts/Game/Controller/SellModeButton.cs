using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class SellModeButton : TextureButton {
	private Texture2D _iconOn;
	private Texture2D _iconOff;
	public GameController GameController { get; set; }

	public override void _Ready() {
		Pressed += OnButtonPressed;
		_iconOn = GD.Load<Texture2D>("res://Assets/UI/Sell/sell_on.png");
		_iconOff = GD.Load<Texture2D>("res://Assets/UI/Sell/sell_off.png");

		if (GameController != null)
			UpdateIcon();
	}

	private void OnButtonPressed() {
		if (GameController != null) {
			GameController.ToggleSellMode();
			UpdateIcon();
		} else {
			GD.PrintErr("GameController is null when pressing SellModeButton.");
		}
	}

	public void UpdateIcon() {
		TextureNormal = GameController.SellModeActive ? _iconOn : _iconOff;
	}
}
