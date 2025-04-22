using Godot;
using Goodot15.Scripts;
using System;

public partial class CraftButton : Node2D {
    public CardController CardController { get; set; }

    public CardNode CardNode { get; set; }

    private string cardType;

    public string CardType {
        get => cardType;
        set {
            cardType = value;
            UpdateIcon();
        }
    }



    private void UpdateIcon() {
        string texturePath = Global.GetTexturePath(cardType);

        Texture2D texture = ResourceLoader.Load<Texture2D>(texturePath);

        Button button = GetNode<Button>("Button");

        button.Icon = texture;

        button.Scale = new Vector2(0.4f, 0.4f);
    }

    public void _on_button_button_down() {
    }

    public void _on_button_button_up() {
        // Handle the button release event
        GD.Print("Craft button released.");
        CardController.CraftCardFromSpecifiedCardNode(CardNode);
    }
}
