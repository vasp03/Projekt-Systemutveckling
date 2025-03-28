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
			cardCreationHelper.GetCreatedInstanceOfCard(cardCreationHelper.GetRandomCardType()), this);
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
			SetTopCard(selectedCard);
			selectedCard.SetIsBeingDragged(true);

			if(selectedCard.CardType is IStackable stackable) {
				print("Selected card: " + stackable.TextureType);
				if(stackable.NeighbourAbove != null) {
					stackable.NeighbourAbove.NeighbourBelow = null;
					stackable.NeighbourAbove = null;
				}

				if(stackable.NeighbourBelow != null) {
					stackable.NeighbourBelow.NeighbourAbove = null;
					stackable.NeighbourBelow = null;
				}

				stackable.SetNeighbourAbove(null);
				stackable.SetNeighbourBelow(null);
			}
		}
	}

	public void LeftMouseButtonReleased() {
		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(false);
			CardNode underCard = GetCardUnderMovedCard();
			selectedCard.SetOverLappedCardToStack(underCard);
			if (selectedCard is IStackable stackable) stackable.NeighbourAbove = null;

			selectedCard = null;
		}
	}

	public void PrintCardsNeighbours() {
		// Print the all cards and their neighbours
		foreach (CardNode card in AllCards) {
			if (card.CardType is IStackable stackable) {
				GD.Print("This: " + card.CardType.TextureType + " - " +
					" Above: " + (stackable.NeighbourAbove != null ? stackable.NeighbourAbove.TextureType : "None") +
					" Below: " + (stackable.NeighbourBelow != null ? stackable.NeighbourBelow.TextureType : "None"));
			}
		}

		GD.Print("------------------");
	}
}