using Godot;
using System;
using System.Collections.Generic;

public partial class CardController : Node2D
{
    private List<Card> cards;

    public override void _Ready()
    {
        cards = [];
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }

    public Boolean cardsStackable(Card card1, Card card2)
    {
        if(card1 is IStackable && card2 is IStackable)
        {
            return card1.CanStackWith(card2) == card2.CanStackWith(card1);
        }

        return false;
    }
}
