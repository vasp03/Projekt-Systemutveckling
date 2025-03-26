using System.Collections.Generic;
using Godot;

public partial class CardNode : Node2D
{
    private Card card;

    private bool hasBeenCreated = false;

    Sprite2D sprite;

    private const float HighLightFactor = 1.3f;

    private CardController cardController;

    private bool mouseIsHovering = false;

    private bool isBeingDragged = false;

    private Vector2 oldMousePosition;

    private bool oldIsHighlighted;

    private List<CardNode> OverlappingCards = new List<CardNode>();

    public bool CreateNode(Card card, Vector2 position, CardController cardController)
    {
        this.cardController = cardController;

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

    public void SetIsBeingDragged(bool isBeingDragged)
    {
        oldMousePosition = GetGlobalMousePosition();
        this.isBeingDragged = isBeingDragged;
    }

    public bool GetIsBeingDragged()
    {
        return isBeingDragged;
    }

    public bool CreateNode(Card card, CardController cardController)
    {
        return CreateNode(card, new Vector2(100, 100), cardController);
    }

    public Card GetCard()
    {
        return card;
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

    public void SetHighlighted(bool isHighlighted)
    {
        if (isHighlighted && !oldIsHighlighted)
        {
            sprite.SetModulate(sprite.Modulate * HighLightFactor);
            oldIsHighlighted = true;
        }
        else if (!isHighlighted && oldIsHighlighted)
        {
            oldIsHighlighted = false;
            sprite.SetModulate(sprite.Modulate / HighLightFactor);
        }
    }

    public static CardNode GetCardNodeFromArea2D(Area2D area2D)
    {
        return (CardNode)area2D.GetParent();
    }

    public List<CardNode> GetOverlappingCards()
    {
        return OverlappingCards;
    }

    public void _on_area_2d_mouse_entered()
    {
        mouseIsHovering = true;
        cardController.AddCardToHoveredCards(this);
    }

    public void _on_area_2d_mouse_exited()
    {
        mouseIsHovering = false;
        cardController.RemoveCardFromHoveredCards(this);
    }

    public void _on_area_2d_area_entered(Area2D area)
    {
        GD.Print("Area entered");
        OverlappingCards.Add(GetCardNodeFromArea2D(area));
    }

    public void _on_area_2d_area_exited(Area2D area)
    {
        GD.Print("Area exited");
        OverlappingCards.Remove(GetCardNodeFromArea2D(area));
    }

    public override void _Process(double delta)
    {
        if (isBeingDragged)
        {
            Vector2 mousePosition = GetGlobalMousePosition();

            Position += mousePosition - oldMousePosition;

            oldMousePosition = mousePosition;
        }
    }
}
