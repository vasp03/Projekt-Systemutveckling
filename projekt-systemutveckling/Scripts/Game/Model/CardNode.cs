using Godot;
using System;
using System.Collections.Generic;

public partial class CardNode : Node2D
{
    private Card card;

    private bool hasBeenCreated = false;

    Sprite2D sprite;

    private const float HighLightFactor = 1.3f;

    public bool CreateNode(Card card, Vector2 position)
    {
        if (hasBeenCreated)
        {
            return false;
        }

        this.card = card;
        this.hasBeenCreated = true;
        sprite = GetNode<Sprite2D>("Sprite2D");

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
        
        sprite.Texture = texture;
    }

    public bool MouseIsHovering(Vector2 mousePosition)
    {
        Vector2 size = sprite.Texture.GetSize() * sprite.Scale;
        Vector2 halfSize = size / 2;

        // Create a rectangle in global coordinates
        Rect2 globalRect = new Rect2(GlobalPosition - halfSize, size);

        return globalRect.HasPoint(mousePosition);
    }

    public void SetHighlighted(bool isHighlighted)
    {
        if (isHighlighted)
        {
            sprite.SetModulate(sprite.Modulate * HighLightFactor);
        }
        else
        {
            sprite.SetModulate(sprite.Modulate / HighLightFactor);
        }
    }

    public void _on_area_2d_mouse_entered()
    {
        GD.Print("Mouse entered");
        SetHighlighted(true);
    }

    public void _on_area_2d_mouse_exited()
    {
        GD.Print("Mouse exited");
        SetHighlighted(false);
    }
}
