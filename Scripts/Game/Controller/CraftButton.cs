using Godot;
using System;

public partial class CraftButton : Node {
    CardNode _cardNode;
    CardController _cardController;
    GameController _gameController;
    MenuController _menuController;

    public CraftButton(CardNode cardNode, CardController cardController, GameController gameController, MenuController menuController) {
        _cardNode = cardNode;
        _cardController = cardController;
        _gameController = gameController;
        _menuController = menuController;
    }

    public void _on_button_button_down() {
        GD.Print("Craft button pressed.");
    }

    public void _on_button_button_up() {
        // Handle the button release event
        GD.Print("Craft button released.");
    }
}
