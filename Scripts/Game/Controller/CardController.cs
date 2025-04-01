using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public class CardController
{
	public const string CARD_GROUP_NAME = "CARDS";
	private readonly CardCreationHelper cardCreationHelper = new();

	private readonly List<CardNode> hoveredCards = new();

	private readonly NodeController nodeController;

	private CardNode selectedCard;

	// Constructor
	public CardController(NodeController nodeController)
	{
		this.nodeController = nodeController;
	}

	public int CardCount => AllCards.Count;

	public IReadOnlyCollection<CardNode> AllCards =>
		nodeController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

	public IReadOnlyCollection<CardNode> AllCardsSorted =>
		AllCards.OrderBy(x => x.ZIndex).ToArray();

	/// <summary>
	/// Creates a new card and adds it to the scene.
	/// It loads the card scene from the resource path and instantiates it.
	/// It then creates a new card by copying the card from Card scene and adding an instance of CardMaterial to it.
	/// It sets the ZIndex of the new card to be one higher than the current card count.
	/// It also sets the position of the new card to (100, 100).
	/// </summary>
	/// <returns>
	/// The created card instance.
	/// </returns>
	public void CreateCard()
	{
		// Create a new card by copying the card from Card scene and adding a instance of CardMaterial to it
		PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
		CardNode cardInstance = cardScene.Instantiate<CardNode>();

		bool ret = cardInstance.CreateNode(
			cardCreationHelper.GetCreatedInstanceOfCard(cardCreationHelper.GetRandomCardType(), cardInstance), this);
		if (ret)
		{
			cardInstance.ZIndex = CardCount + 1;
			nodeController.AddChild(cardInstance);
			cardInstance.SetPosition(new Vector2(100, 100));
		}
	}

	/// <summary>
	/// Checks if the card is the top card on the scene.
	/// It checks all the hovered cards and returns true if there are no cards with a higher ZIndex.
	/// If there are cards with a higher ZIndex, it returns false.
	/// </summary>
	private bool CardIsTopCard(Node2D cardNode)
	{
		foreach (CardNode node in hoveredCards)
			if (node.ZIndex > cardNode.ZIndex)
				return false;
		return true;
	}

	/// <summary>
	/// Generates a new UUID (Universally Unique Identifier) string.
	/// </summary>
	public static string GenerateUUID()
	{
		return Guid.NewGuid().ToString();
	}

	/// <summary>
	/// Adds the card to the hovered cards list and sets its highlighted state to true.
	/// </summary>
	public void AddCardToHoveredCards(CardNode cardNode)
	{
		hoveredCards.Add(cardNode);
		CheckForHighLight();
	}

	/// <summary>
	/// Removes the card from the hovered cards list and sets its highlighted state to false.
	/// </summary>
	public void RemoveCardFromHoveredCards(CardNode cardNode)
	{
		hoveredCards.Remove(cardNode);
		CheckForHighLight();
		cardNode.SetHighlighted(false);
	}

	/// <summary>
	/// Checks if the card is the top card on the scene which the mouse is hovering over and sets the highlighted state.
	/// </summary>
	public void CheckForHighLight()
	{
		foreach (CardNode card in hoveredCards)
			if (CardIsTopCard(card))
				card.SetHighlighted(true);
			else
				card.SetHighlighted(false);
	}

	/// <summary>
	/// Gets the top card at the mouse position.
	/// It checks all the hovered cards and returns the one with the highest ZIndex.
	/// If no card is hovered, it returns null.
	/// </summary>
	/// <returns>
	/// The top card at the mouse position or null if no card is hovered. 
	/// </returns>
	public CardNode GetTopCardAtMousePosition()
	{
		CardNode topCard = null;

		foreach (CardNode card in hoveredCards)
			if (topCard == null)
				topCard = card;
			else if (card.GetZIndex() > topCard.GetZIndex()) topCard = card;

		return topCard;
	}

	/// <summary>
	/// Gets the card under the moved card.
	/// It checks all the hovered cards and returns the one with the highest ZIndex that is below the moved card.
	/// If no card is found, it returns null.
	/// </summary>
	/// <returns>
	/// The card under the moved card or null if no card is found.
	/// </returns>
	private CardNode GetCardUnderMovedCard()
	{
		IReadOnlyCollection<CardNode> hoveredCardsSorted = selectedCard.HoveredCardsSorted;

		CardNode topUnderCard = null;

		foreach (CardNode card in hoveredCardsSorted)
		{
			if (card.ZIndex < selectedCard.ZIndex && (topUnderCard == null || card.ZIndex > topUnderCard.ZIndex))
			{
				topUnderCard = card;
			}
		}

		return topUnderCard;
	}

	/// <summary>
	/// Called when the left mouse button is pressed.
	/// It gets the top card at the mouse position and sets it to be dragged.
	/// It also sets the ZIndex of all cards to make sure the dragged card is on top.
	/// If the card has a neighbour above, it sets the IsMovingOtherCards property to true.
	/// If the card has a neighbour below, it sets the neighbour below to null.
	/// </summary>
	public void LeftMouseButtonPressed()
	{
		selectedCard = GetTopCardAtMousePosition();

		if (selectedCard != null) SetZIndexForAllCards(selectedCard);

		if (selectedCard != null)
		{
			selectedCard.SetIsBeingDragged(true);

			if (selectedCard.HasNeighbourAbove())
			{
				selectedCard.IsMovingOtherCards = true;
			}
			else
			{
				// Set the card that is being dragged to the top
				selectedCard.ZIndex = CardCount + 1;
			}

			// Set the neighbour below to null if the card is moved to make the moved card able to get new neighbours
			// And sets the card below if it exists to not have a neighbour above
			if (selectedCard.HasNeighbourBelow())
			{
				((IStackable)selectedCard.CardType)?.NeighbourBelow.SetNeighbourAbove(null);
				((IStackable)selectedCard.CardType)?.SetNeighbourBelow(null);
			}
		}
	}

	/// <summary>
	/// Sets the ZIndex of all cards based on the selected card.
	/// It checks the stack above the selected card and sets the ZIndex of all cards accordingly.
	/// The selected card is set to the top of the stack.
	/// </summary>
	/// <param name="cardNode">The card node to set the ZIndex from and its neighbours above.</param>
	public void SetZIndexForAllCards(CardNode cardNode)
	{
		List<IStackable> stackAbove = ((IStackable)cardNode.CardType)?.StackAbove;

		if (stackAbove == null) return;

		int counterForStackedCards = CardCount - stackAbove.Count;
		int counterForOtherCards = 1;

		foreach (CardNode card in AllCardsSorted)
		{
			if (stackAbove.Contains((IStackable)card.CardType ?? null) || card == cardNode)
			{
				card.ZIndex = counterForStackedCards;
				counterForStackedCards++;
			}
			else
			{
				card.ZIndex = counterForOtherCards;
				counterForOtherCards++;
			}
		}
	}

	/// <summary>
	/// Called when the left mouse button is released.
	/// It sets the selected card to not be dragged anymore.
	/// It checks for a card under the moved card and sets it as a neighbour below if it exists.
	/// </summary>
	public void LeftMouseButtonReleased()
	{
		if (selectedCard != null)
		{
			selectedCard.SetIsBeingDragged(false);

			// Checks for a card under the moved card and sets if it exists as a neighbour below. 
			CardNode underCard = GetCardUnderMovedCard();

			if (underCard != null && !selectedCard.HasNeighbourBelow() && !underCard.HasNeighbourAbove())
			{
				selectedCard.SetOverLappedCardToStack(underCard);
			}

			selectedCard = null;
		}
	}

	/// <summary>
	/// Used to print the cards and their neighbours for debugging purposes.
	/// It prints the card type, ZIndex, and whether the card is being dragged.
	/// It also prints the neighbours above and below the card, if they exist.
	/// </summary>
	public void PrintCardsNeighbours()
	{
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