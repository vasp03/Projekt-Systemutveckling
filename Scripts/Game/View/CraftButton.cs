using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class CraftButton : Node2D {
    public CardController CardController { get; set; }

    public string NameOfButton { get; set; }

    public CardNode CardNode { get; set; }

    public void _on_button_button_down() {
    }

    public void _on_button_button_up() {
        // Handle the button release event
        GD.Print("Craft button released.");
        CardController.CraftCardFromSpecifiedCardNode(CardNode);
    }
}