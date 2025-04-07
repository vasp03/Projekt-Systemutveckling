using Godot;

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
		else if (@event is InputEventKey eventKey) {
			if (eventKey.Pressed && eventKey.Keycode == Key.Space) {
				cardController.CreateCard("");
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.Escape) {
				menuController.OpenPauseMenu();
				Visible = false;
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.A) {
				cardController.CreateCard("Wood");
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
