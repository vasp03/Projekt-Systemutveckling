using Godot;
using Goodot15.Scripts.Game.Model.Living;

public partial class GameController : Node2D {
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
        if (@event is InputEventKey eventKey && eventKey.Pressed) {
            switch (eventKey.Keycode) {
                case Key.Escape:
                    menuController.OpenPauseMenu();
                    Visible = false; // Hide the game scene
                    break;
                case Key.Space:
                    cardController.CreateCard("Random");
                    break;
                case Key.D:
                    cardController.PrintCardsNeighbours();
                    break;
                case Key.Key1:
                    cardController.CreateCard("Apple");
                    break;
                case Key.Key2:
                    cardController.CreateCard("Berry");
                    break;
                case Key.Key3:
                    cardController.CreateCard("Leaves");
                    break;
                case Key.Key4:
                    cardController.CreateCard("Mine");
                    break;
                case Key.Key5:
                    cardController.CreateCard("Plank");
                    break;
                case Key.Key6:
                    cardController.CreateCard("Stick");
                    break;
                case Key.Key7:
                    cardController.CreateCard("Stone");
                    break;
                case Key.Key8:
                    cardController.CreateCard("Water");
                    break;
                case Key.Key9:
                    cardController.CreateCard("Wood");
                    break;
                case Key.Key0:
                    cardController.CreateAllCards();
                    break;
                case Key.A:
                    cardController.CreateCard(new LivingPlayer());
                    break;
            }
        } else if (@event is InputEventMouseButton mouseButton) {
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