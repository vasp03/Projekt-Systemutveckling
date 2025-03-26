using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

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

	public bool cardsStackable(Card card1, Card card2)
	{
		if (card1 is IStackable card1Stackable && card2 is IStackable card2Stackable)
			return card1Stackable.CanStackWith(card2) == card2Stackable.CanStackWith(card1);

		return false;
	}
}