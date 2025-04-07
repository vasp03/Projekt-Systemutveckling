using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public class CardController {
	public const string CARD_GROUP_NAME = "CARDS";
	private readonly CardCreationHelper CardCreationHelper;

	private readonly CraftingController CraftingController;

	private readonly List<CardNode> hoveredCards = [];

	private readonly MouseController mouseController;

	private readonly NodeController nodeController;

	private CardNode selectedCard;

	// Constructor
	public CardController(NodeController nodeController, MouseController mouseController) {
		this.nodeController = nodeController;
		this.mouseController = mouseController;
		CardCreationHelper = new CardCreationHelper(nodeController, this);
		CraftingController = new CraftingController(CardCreationHelper);

		// Add crafting recipes
		CraftingController.AddRecipe(new CraftingRecipe("Plank", ["Wood", "Wood"], ["Plank"]));
		CraftingController.AddRecipe(new CraftingRecipe("SwordMK1", ["Wood", "Wood", "Stone"], ["SwordMK1"]));
	}

	public int CardCount => AllCards.Count;

	public IReadOnlyCollection<CardNode> AllCards =>
		nodeController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

	public IReadOnlyCollection<CardNode> AllCardsSorted =>
		AllCards.OrderBy(x => x.ZIndex).ToArray();

	public CardNode CreateCard(Card card, Vector2 position = default) {
		ArgumentNullException.ThrowIfNull(card);

		PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
		CardNode cardInstance = cardScene.Instantiate<CardNode>();

		cardInstance.CardType = card;

		cardInstance.Position = position;
		nodeController.AddChild(cardInstance);

		return cardInstance;
	}

	public CardNode CreateCard(string cardType) {
		CardNode cardNode = CreateCard();
		cardNode.CardType = CardCreationHelper.GetCreatedInstanceOfCard(cardType);
		;

		return cardNode;
	}

	/// <summary>
	///     Creates a new card and adds it to the scene, with a random underlying CardType
	/// </summary>
	/// <returns>
	///     The created card instance.
	/// </returns>
	public CardNode CreateCard() {
		// Create a new card by copying the card from Card scene and adding a instance of CardMaterial to it
		PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
		CardNode cardInstance = cardScene.Instantiate<CardNode>();

		bool ret = cardInstance.CreateNode(
			CardCreationHelper.GetCreatedInstanceOfCard(CardCreationHelper.GetRandomCardType()), this);
		if (ret) {
			cardInstance.ZIndex = CardCount + 1;
			nodeController.AddChild(cardInstance);
			cardInstance.SetPosition(new Vector2(100, 100));
		}

		return cardInstance;
	}

	/// <summary>
	///     Checks if the card is the top card on the scene.
	/// </summary>
	private bool CardIsTopCard(Node2D cardNode) {
		foreach (CardNode node in hoveredCards)
			if (node.ZIndex > cardNode.ZIndex)
				return false;
		return true;
	}

	/// <summary>
	///     Generates a new UUID (Universally Unique Identifier) string.
	/// </summary>
	public static string GenerateUUID() {
		return Guid.NewGuid().ToString();
	}

	/// <summary>
	///     Adds the card to the hovered cards list and sets its highlighted state to true.
	/// </summary>
	public void AddCardToHoveredCards(CardNode cardNode) {
		hoveredCards.Add(cardNode);
		CheckForHighLight();
	}

	/// <summary>
	///     Removes the card from the hovered cards list and sets its highlighted state to false.
	/// </summary>
	public void RemoveCardFromHoveredCards(CardNode cardNode) {
		hoveredCards.Remove(cardNode);
		CheckForHighLight();
		cardNode.SetHighlighted(false);
	}

	/// <summary>
	///     Checks if the card is the top card on the scene which the mouse is hovering over and sets the highlighted state.
	/// </summary>
	public void CheckForHighLight() {
		foreach (CardNode card in hoveredCards)
			if (CardIsTopCard(card))
				card.SetHighlighted(true);
			else
				card.SetHighlighted(false);
	}

	/// <summary>
	///     Gets the top card at the mouse position.
	/// </summary>
	/// <returns>
	///     The top card at the mouse position or null if no card is hovered.
	/// </returns>
	public CardNode GetTopCardAtMousePosition() {
		CardNode topCard = null;

		foreach (CardNode card in hoveredCards)
			if (topCard == null)
				topCard = card;
			else if (card.GetZIndex() > topCard.GetZIndex()) topCard = card;

		return topCard;
	}

	/// <summary>
	///     Gets the card under the moved card.
	/// </summary>
	/// <returns>
	///     The card under the moved card or null if no card is found.
	/// </returns>
	private CardNode GetCardUnderMovedCard() {
		IReadOnlyCollection<CardNode> hoveredCardsSorted = selectedCard.HoveredCardsSorted;

		CardNode topUnderCard = null;

		foreach (CardNode card in hoveredCardsSorted)
			if (card.ZIndex < selectedCard.ZIndex && (topUnderCard == null || card.ZIndex > topUnderCard.ZIndex))
				topUnderCard = card;

		return topUnderCard;
	}

	/// <summary>
	///     Called when the left mouse button is pressed.
	/// </summary>
	public void LeftMouseButtonPressed() {
		selectedCard = GetTopCardAtMousePosition();

		mouseController.SetMouseCursor(MouseController.MouseCursor.hand_close);

		if (selectedCard != null) SetZIndexForAllCards(selectedCard);

		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(true);

			if (selectedCard.HasNeighbourAbove())
				selectedCard.IsMovingOtherCards = true;
			else
				// Set the card that is being dragged to the top
				selectedCard.ZIndex = CardCount + 1;

			// Set the neighbour below to null if the card is moved to make the moved card able to get new neighbours
			// And sets the card below if it exists to not have a neighbour above
			if (selectedCard.HasNeighbourBelow())
				if (selectedCard.CardType is IStackable stackable) {
					if (stackable.NeighbourBelow != null) stackable.NeighbourBelow.NeighbourAbove = null;
					stackable.NeighbourBelow = null;
				}
		}
	}

	/// <summary>
	///     Sets the ZIndex of all cards based on the selected card.
	/// </summary>
	/// <param name="cardNode">The card node to set the ZIndex from and its neighbours above.</param>
	public void SetZIndexForAllCards(CardNode cardNode) {
		List<IStackable> stackAbove = null;

		if (cardNode.CardType is IStackable stackable) stackAbove = stackable.StackAbove;

		int counterForStackedCards = CardCount - (stackAbove != null ? stackAbove.Count : 0);
		int counterForOtherCards = 1;

		foreach (CardNode card in AllCardsSorted)
			if ((stackAbove != null && card is IStackable stackableCard && stackAbove.Contains(stackableCard)) ||
			    card == cardNode) {
				card.ZIndex = counterForStackedCards;
				counterForStackedCards++;
			}
			else {
				card.ZIndex = counterForOtherCards;
				counterForOtherCards++;
			}
	}

	/// <summary>
	///     Called when the left mouse button is released.
	/// </summary>
	public void LeftMouseButtonReleased() {
		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(false);

			// Checks for a card under the moved card and sets if it exists as a neighbour below. 
			CardNode underCard = GetCardUnderMovedCard();

			if (underCard != null && !selectedCard.HasNeighbourBelow() && !underCard.HasNeighbourAbove())
				selectedCard.SetOverLappedCardToStack(underCard);

			selectedCard = null;
		}
	}

	/// <summary>
	///     Used to print the cards and their neighbours for debugging purposes.
	/// </summary>
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