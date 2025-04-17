using System.Collections.Generic;
using System.Text;
using Godot;
using Vector2 = Godot.Vector2;

public partial class GameController : Node2D {
    private readonly List<int> numberList = new();
    private CardController cardController;
    private MenuController menuController;
    private MouseController mouseController;

    [Export] public Button CraftButton { get; set; }

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
                    cardController.CreateCard("Random", Vector2.One * 100);
                    break;
                case Key.D:
                    cardController.PrintCardsNeighbours();
                    break;
                case Key.Key0:
                case Key.Key1:
                case Key.Key2:
                case Key.Key3:
                case Key.Key4:
                case Key.Key5:
                case Key.Key6:
                case Key.Key7:
                case Key.Key8:
                case Key.Key9:
                    MultipleNumberInput((int)eventKey.Keycode - (int)Key.Key0);
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

    public void MultipleNumberInput(int number) {
        numberList.Add(number);

        if (numberList.Count >= 2) {
            StringBuilder numbers = new();
            for (int i = 0; i < numberList.Count; i++) {
                numbers.Append(numberList[i]);
            }

            // Create a new card with the numbers in the list
            cardController.CreateCard(numbers.ToString(), new Vector2(100, 100));

            numberList.Clear();
        }
    }
}