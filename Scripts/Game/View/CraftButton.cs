using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.View;

public partial class CraftButton : Node2D {
    public CardNode CardNode { get; set; }

    public override void _Ready() {
        GetNode<ButtonWithSound>("CraftButton").ButtonUp += OnCraftButtonReleased;
    }

    public void OnCraftButtonReleased() {
        // Handle the button release event
        // GD.Print("Craft button released.");
        GameController.Singleton!.CardController.CraftCardFromSpecifiedCardNode(CardNode);
    }
}