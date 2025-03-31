using Godot;

public partial class NodeController : Node2D {
	private CardController cardController;

	public override void _Ready() {
		cardController = new CardController(this);
	}

	public override void _Input(InputEvent @event) {
		// Detect mouse movement
		if (@event is InputEventMouseMotion mouseMotion) {
		}
		else if (@event is InputEventKey eventKey) {
			if (eventKey.Pressed && eventKey.Keycode == Key.Space)
				cardController.CreateCard();
			else if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
				// Exit the game
				GetTree().Quit();
			// else if (eventKey.Pressed && eventKey.Keycode == Key.A) cardController.PrintCardsNeighbours();
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