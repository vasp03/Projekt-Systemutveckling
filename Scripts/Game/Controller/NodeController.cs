using Godot;
using Goodot15.Scripts.Game.Model.Living;

public partial class NodeController : Node2D {
	private CardController cardController;
	private MenuController menuController;
	private MouseController mouseController;

	public override void _Ready() {
		mouseController = new MouseController();
		cardController = new CardController(this, mouseController);

		menuController = GetNode<MenuController>("/root/MenuController");
		menuController.SetNodeController(this);
	}

	public override void _Input(InputEvent @event) {
		// Detect mouse movement
		if (@event is InputEventMouseMotion mouseMotion) {
		}
		else if (@event is InputEventKey eventKey && eventKey.Pressed) {
			if (eventKey.Keycode == Key.Space) {
				cardController.CreateCard("Random");
			}
			else if (eventKey.Keycode == Key.Escape) {
				menuController.OpenPauseMenu();
				Visible = false;
			}
			else if (eventKey.Keycode == Key.A) {
				cardController.CreateCard("Wood");
			}
			else if (eventKey.Keycode == Key.S) {
				cardController.CreateCard().CardType = new LivingPlayer();
			}
		}
		else if (@event is InputEventMouseButton mouseButton) {
			if (mouseButton.Pressed)
				cardController.LeftMouseButtonPressed();
			else
				cardController.LeftMouseButtonReleased();
		}
	}

	public Vector2 GetMousePosition() {
		return GetGlobalMousePosition();
	}
}