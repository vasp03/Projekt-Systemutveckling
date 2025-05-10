using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class HUD : CanvasLayer {
	public GameController GameController { get; set; }

	public override void _Ready() {
		GD.Print("HUD _Ready called");

		var sellModeButton = GetNode<SellModeButton>("HUDRoot/SellModeButton");
		GD.Print("SellModeButton found");

		sellModeButton.GameController = GameController;
	}
}
