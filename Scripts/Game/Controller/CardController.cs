using System.Collections.Generic;
using System.Linq;
using Godot;

public class CardController {
	public const string CARD_GROUP_NAME = "CARDS";
	private readonly CardCreationHelper cardCreationHelper = new();

	// private readonly List<CardNode> hoveredCards = [];

	private readonly NodeController nodeController;

	private CardNode selectedCard;

	public CardController(NodeController nodeController) {
		this.nodeController = nodeController;

		this.nodeController.GetViewport().PhysicsObjectPickingSort = true;
		this.nodeController.GetViewport().PhysicsObjectPickingFirstOnly = true;
	}

	public int CardCount => AllCards.Count;

	public IReadOnlyCollection<CardNode> AllCards =>
		nodeController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

	public IReadOnlyCollection<CardNode> AllCardsSorted =>
		AllCards.OrderBy(x => x.ZIndex).ToArray();

	public IReadOnlyCollection<CardNode> HoveredCards => AllCards.Where(e => e.MouseIntersecting).ToArray();

	// public void AddCardToHoveredCards(CardNode cardNode) {
	// 	hoveredCards.Add(cardNode);
	// 	CheckForHighLight();
	// }
// 
	// public void RemoveCardFromHoveredCards(CardNode cardNode) {
	// 	hoveredCards.Remove(cardNode);
	// 	CheckForHighLight();
	// 	cardNode.SetHighlighted(false);
	// }

	// Get the top card at the mouse position
	public CardNode TopHoveredCard => HoveredCards.OrderByDescending(x => x.ZIndex).FirstOrDefault();

	public void CreateCard() {
		// Create a new card by copying the card from Card scene and adding a instance of CardMaterial to it
		PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
		CardNode cardInstance = cardScene.Instantiate<CardNode>();

		bool ret = cardInstance.CreateNode(
			cardCreationHelper.GetCreatedInstanceOfCard(cardCreationHelper.GetRandomCardType(), cardInstance), this
		);
		if (ret) {
			cardInstance.ZIndex = CardCount + 1;
			nodeController.AddChild(cardInstance);
			cardInstance.SetPosition(new Vector2(100, 100));
		}
	}

	public void LeftMouseButtonPressed() {
		selectedCard = TopHoveredCard;

		// if (selectedCard != null) SetZIndexForAllCards(selectedCard);

		if (selectedCard != null) selectedCard.IsBeingDragged = true;
		// if (selectedCard.HasNeighbourAbove())
		// 	selectedCard.IsMovingOtherCards = true;
		// else
		// 	SetTopCard(selectedCard);
		// 
		// if (selectedCard.HasNeighbourBelow()) {
		// 	((IStackable)selectedCard.CardType)?.NeighbourBelow.SetNeighbourAbove(null);
		// 	((IStackable)selectedCard.CardType)?.SetNeighbourBelow(null);
		// }
	}

	/// <summary>
	///     Called when the left mouse button is released.
	/// </summary>
	public void LeftMouseButtonReleased() {
		// Global.RecursiveCheckIteration = 0;
// 
		if (selectedCard != null) selectedCard.IsBeingDragged = false;
		// if (!selectedCard.IsMovingOtherCards) {
		// 	CardNode underCard = GetCardUnderMovedCard();
		// 	if (underCard != null && !underCard.HasNeighbourAbove())
		// 		selectedCard.SetOverLappedCardToStack(underCard);
		// }
		// // 
		// selectedCard.IsMovingOtherCards = false;
		// selectedCard = null;
	}
}