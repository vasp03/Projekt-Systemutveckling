using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public partial class CardController : Node2D {
	public const string CARD_GROUP_NAME = "CARDS";
	private readonly CardCreationHelper cardCreationHelper = new();

	private readonly List<CardNode> hoveredCards = new();

	private CardNode selectedCard;

	public int CardCount => AllCards.Count;

	public IReadOnlyCollection<CardNode> AllCards =>
		GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

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
			AddChild(cardInstance);
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

	// UUID generator
	public static string GenerateUUID() {
		return Guid.NewGuid().ToString();
	}

	// Get all cards


	// Move card to the mouse position
	public void MoveCardToMousePosition(CardNode cardNode) {
		Vector2 mousePosition = GetGlobalMousePosition();
		cardNode.SetPosition(mousePosition);
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

	private CardNode GetCardUnderMovedCard(){
		CardNode topCard = null;

		foreach (CardNode card in hoveredCards) {
			if ((topCard == null || card.GetZIndex() > topCard.GetZIndex()) && !card.IsBeingDragged) {
				topCard = card;
				break;
			}			
		}

		GD.Print("Top card: " + topCard.CardType.TextureType);

		return topCard;
	}

	public override void _Input(InputEvent @event) {
		// Detect mouse movement
		if (@event is InputEventMouseMotion mouseMotion) {
		}
		else if (@event is InputEventKey eventKey) {
			if (eventKey.Pressed && eventKey.Keycode == Key.Space)
				CreateCard();
			else if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
				// Exit the game
				GetTree().Quit();
			else if (eventKey.Pressed && eventKey.Keycode == Key.A) {
				// Print the all cards and their neighbours
				foreach (CardNode card in AllCards) {
					if (card.CardType is IStackable stackable) {
						GD.Print("This: " + card.CardType.TextureType + " - " +
							" Above: " + (stackable.NeighbourAbove != null ? stackable.NeighbourAbove.TextureType : "None") +
							" Below: " + (stackable.NeighbourBelow != null ? stackable.NeighbourBelow.TextureType : "None"));
					}
				}
			}
		}
		else if (@event is InputEventMouseButton mouseButton) {
			if (mouseButton.Pressed) {
				selectedCard = GetTopCardAtMousePosition();
				if (selectedCard != null) {
					SetTopCard(selectedCard);
					selectedCard.SetIsBeingDragged(true);
				}
			}
			else {
				if (selectedCard != null) {
					selectedCard.SetIsBeingDragged(false);
					CardNode underCard = GetCardUnderMovedCard();
					selectedCard.SetOverLappedCardToStack(underCard);
					selectedCard = null;
				}
			}
		}
	}
}