using System;
using System.Collections.Generic;

public partial class CardMaterial(String textureAddress, bool movable, int cost) : Card(textureAddress, movable, cost), IStackable
{
    private Card neighbourAbove;
    private Card neighbourBelow;

    public List<string> GetStackableItems()
    {
        return new List<string> { "CardMaterial" };
    }

    public Card GetNeighbourAbove()
    {
        return neighbourAbove;
    }

    public Card GetNeighbourBelow()
    {
        return neighbourBelow;
    }

    public void SetNeighbourAbove(Card card)
    {
        neighbourAbove = card;
    }

    public void SetNeighbourBelow(Card card)
    {
        neighbourBelow = card;
    }

    public bool CanStackWith(Card card)
    {
        return card is CardMaterial;
    }
}
