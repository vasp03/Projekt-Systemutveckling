using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public class CardController {
	public const string CARD_GROUP_NAME = "CARDS";
	private readonly CardCreationHelper cardCreationHelper = new();

	private readonly List<CardNode> hoveredCards = new();

	private readonly NodeController nodeController;

	private CardNode selectedCard;

	// Constructor
	public CardController(NodeController nodeController) {
		this.nodeController = nodeController;
	}

	public int CardCount => AllCards.Count;

	public IReadOnlyCollection<CardNode> AllCards =>
		nodeController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

	public IReadOnlyCollection<CardNode> AllCardsSorted =>
		AllCards.OrderBy(x => x.ZIndex).ToArray();

	public void CreateCard() {
		// Create a new card by copying the card from Card scene and adding a instance of CardMaterial to it
		PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
		CardNode cardInstance = cardScene.Instantiate<CardNode>();

		bool ret = cardInstance.CreateNode(
			cardCreationHelper.GetCreatedInstanceOfCard(cardCreationHelper.GetRandomCardType(), cardInstance), this);
		if (ret) {
			cardInstance.ZIndex = CardCount + 1;
			nodeController.AddChild(cardInstance);
			cardInstance.SetPosition(new Vector2(100, 100));
		}
	}

	private void SetTopCard(Node2D cardNode) {
		IReadOnlyCollection<CardNode> cardNodes = AllCardsSorted;

		foreach (CardNode node in cardNodes)
			if (node.ZIndex > cardNode.ZIndex)
				node.ZIndex -= 1;

		// Set the card that is being dragged to the top
		cardNode.ZIndex = cardNodes.Count;
	}

	private bool CardIsTopCard(Node2D cardNode) {
		// Get all the card nodes
		IReadOnlyCollection<CardNode> cardNodes = AllCardsSorted;

		foreach (CardNode node in cardNodes)
			if (node.ZIndex > cardNode.ZIndex)
				if (hoveredCards.Contains(node))
					return false;

		return true;
	}

	public void print(string message) {
		GD.Print(message);
	}

	public static string GenerateUUID() {
		return Guid.NewGuid().ToString();
	}

	public void AddCardToHoveredCards(CardNode cardNode) {
		hoveredCards.Add(cardNode);
		CheckForHighLight();
	}

	public void RemoveCardFromHoveredCards(CardNode cardNode) {
		hoveredCards.Remove(cardNode);
		CheckForHighLight();
		cardNode.SetHighlighted(false);
	}

	public void CheckForHighLight() {
		foreach (CardNode card in hoveredCards)
			if (CardIsTopCard(card))
				card.SetHighlighted(true);
			else
				card.SetHighlighted(false);
	}

	// Get the top card at the mouse position
	public CardNode GetTopCardAtMousePosition() {
		CardNode topCard = null;

		foreach (CardNode card in hoveredCards)
			if (topCard == null)
				topCard = card;
			else if (card.GetZIndex() > topCard.GetZIndex()) topCard = card;

		return topCard;
	}

	private CardNode GetCardUnderMovedCard() {
		// Get the card under the moved card
		CardNode topCard = null;

		foreach (CardNode overlappedCard in selectedCard.HoveredCards)
			if (topCard == null)
				topCard = overlappedCard;
			else if (overlappedCard.GetZIndex() > topCard.GetZIndex()) topCard = overlappedCard;

		return topCard;
	}

	public void LeftMouseButtonPressed() {
		selectedCard = GetTopCardAtMousePosition();

		if (selectedCard != null) SetZIndexForAllCards(selectedCard);

		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(true);

			if (selectedCard.HasNeighbourAbove())
				selectedCard.IsMovingOtherCards = true;
			else
				SetTopCard(selectedCard);

			if (selectedCard.HasNeighbourBelow()) {
				((IStackable)selectedCard.CardType)?.NeighbourBelow.SetNeighbourAbove(null);
				((IStackable)selectedCard.CardType)?.SetNeighbourBelow(null);
			}
		}
	}

	public void SetZIndexForAllCards(CardNode cardNode) {
		List<IStackable> stackAbove = ((IStackable)cardNode.CardType)?.StackAbove;

		if (stackAbove == null) return;

		int counterForStackedCards = CardCount - stackAbove.Count;
		int counterForOtherCards = 1;

		foreach (CardNode card in AllCardsSorted) {
			if (stackAbove.Contains((IStackable)card.CardType ?? null) || card == cardNode) {
				card.ZIndex = counterForStackedCards;
				counterForStackedCards++;
			}
			else {
				card.ZIndex = counterForOtherCards;
				counterForOtherCards++;
			}
		}
	}

	public void LeftMouseButtonReleased() {
		Global.AntiInfinity = 0;

		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(false);
			if (!selectedCard.IsMovingOtherCards) {
				CardNode underCard = GetCardUnderMovedCard();
				if (underCard != null && !underCard.HasNeighbourAbove())
					selectedCard.SetOverLappedCardToStack(underCard);
			}

			selectedCard.IsMovingOtherCards = false;
			selectedCard = null;
		}
	}

	public void PrintCardsNeighbours() {
		// Print the all cards and their neighbours
		foreach (CardNode card in AllCardsSorted)
			if (card.CardType is IStackable stackable)
				GD.Print("This: " + card.CardType.TextureType + ":" + card.ZIndex + " - " + card.IsBeingDragged +
						 " | Above: " +
						 (stackable.NeighbourAbove != null
							 ? stackable.NeighbourAbove.TextureType + " - " +
							   ((Card)stackable.NeighbourAbove).CardNode.IsBeingDragged
							 : "None") +
						 " | Below: " +
						 (stackable.NeighbourBelow != null
							 ? stackable.NeighbourBelow.TextureType + " - " +
							   ((Card)stackable.NeighbourBelow).CardNode.IsBeingDragged
							 : "None"));

		GD.Print("------------------");
	}
}