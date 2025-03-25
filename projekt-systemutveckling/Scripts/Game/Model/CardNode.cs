using Godot;
using System;
using System.Collections.Generic;

public partial class CardNode : Node2D
{
    private Card card;

    private bool hasBeenCreated = false;

    public bool CreateNode(Card card, Vector2 position)
    {
        if (hasBeenCreated)
        {
            return false;
        }

        this.card = card;
        this.hasBeenCreated = true;

        ApplyTexture();

        // Set the name of the card to the name of the card
        Name = card.GetName();

        // Add the card to the group "Cards"
        AddToGroup("Cards");

        return true;
    }

    public bool CreateNode(Card card)
    {
        return CreateNode(card, new Vector2(100, 100));
    }

    public Card GetCard()
    {
        return card;
    }

    public new void SetPosition(Vector2 position)
    {
        Position = position;
    }

    private void ApplyTexture()
    {
        Texture2D texture;
        // try to load the texture from the address
        try
        {
            texture = GD.Load<Texture2D>(card.GetTextureAddress());
        }
        catch (System.Exception)
        {
            texture = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Error.png");
            GD.PrintErr("Texture not found for card: " + card.GetTextureAddress());
        }

        Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
        sprite.Texture = texture;
    }
}
