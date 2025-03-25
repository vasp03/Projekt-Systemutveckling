using Godot;
using System;
using System.Collections.Generic;

public partial class CardNode(Card card) : Node2D
{
    private readonly Card card = card;
    private bool isHighlighted;

    public Card GetCard()
    {
        return card;
    }

    public void SetHighlighted(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
    }
}
