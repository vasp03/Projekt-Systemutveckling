using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public partial class CardController {

	// Constructor
	public CardController(NodeController nodeController) {
		this.nodeController = nodeController;
	}

	private readonly NodeController nodeController;

	public const string CARD_GROUP_NAME = "CARDS";
	private readonly CardCreationHelper cardCreationHelper = new();

	private readonly List<CardNode> hoveredCards = new();

	private CardNode selectedCard;

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
			cardInstance.ZIndex = CardCount;
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

		foreach (CardNode overlappedCard in selectedCard.HoveredCards) {
			if (topCard == null)
				topCard = overlappedCard;
			else if (overlappedCard.GetZIndex() > topCard.GetZIndex()) topCard = overlappedCard;
		}

		return topCard;
	}

	public void LeftMouseButtonPressed() {
		selectedCard = GetTopCardAtMousePosition();
		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(true);

			if (selectedCard.HasNeighbourAbove()) {
				selectedCard.IsMovingOtherCards = true;
			}
			else {
				SetTopCard(selectedCard);
			}

			if (selectedCard.HasNeighbourBelow()) {
				((IStackable)selectedCard.CardType)?.NeighbourBelow.SetNeighbourAbove(null);
				((IStackable)selectedCard.CardType)?.SetNeighbourBelow(null);
			}
		}
	}

	public void SetZIndexForSpecifiedCard(CardNode cardNode, int zIndex) {
		cardNode.ZIndex = zIndex;
	}

	public void SetTopCardWithFollowingCards(CardNode cardNode) {
		GD.Print("SetTopCardWithFollowingCards");
		int cardCount = AllCards.Count;
		int counter = 0;
		int counterBackwards = AllCards.Count;

		IReadOnlyCollection<CardNode> cardNodes = AllCardsSorted;

		List<CardNode> stackedCards = [cardNode];
		CardNode currentCard = cardNode;

		while (currentCard != null && currentCard.HasNeighbourBelow()) {
			currentCard = ((currentCard.CardType as IStackable)?.NeighbourBelow as Card)?.CardNode;
			if (currentCard != null) {
				GD.Print("Current card: " + currentCard.CardType.TextureType);
				stackedCards.Add(currentCard);
				currentCard.ZIndex = counterBackwards--;
			}
			else {
				GD.Print("Current card is null");
				break;
			}
		}

		foreach (CardNode card in cardNodes) {
			if (counter == cardCount) {
				break;
			}

			if (!stackedCards.Contains(card)) {
				card.ZIndex = counter++;
			}
		}
	}

	public void LeftMouseButtonReleased() {
		Global.AntiInfinity = 0;

		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(false);
			if (!selectedCard.IsMovingOtherCards) {
				CardNode underCard = GetCardUnderMovedCard();
				if (underCard != null && !underCard.HasNeighbourAbove()) {
					selectedCard.SetOverLappedCardToStack(underCard);
				}
			}
			selectedCard.IsMovingOtherCards = false;
			selectedCard = null;
		}


	}

	public void PrintCardsNeighbours() {
		// Print the all cards and their neighbours
		foreach (CardNode card in AllCards) {
			if (card.CardType is IStackable stackable) {
				GD.Print("This: " + card.CardType.TextureType + " - " + card.IsBeingDragged +
					" | Above: " + (stackable.NeighbourAbove != null ? stackable.NeighbourAbove.TextureType + " - " + ((Card)stackable.NeighbourAbove).CardNode.IsBeingDragged : "None") +
					" | Below: " + (stackable.NeighbourBelow != null ? stackable.NeighbourBelow.TextureType + " - " + ((Card)stackable.NeighbourBelow).CardNode.IsBeingDragged : "None"));
			}
		}

		GD.Print("------------------");
	}
}