using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public class CardController {
	public const string CARD_GROUP_NAME = "CARDS";
	private readonly CardCreationHelper CardCreationHelper;

	private readonly List<CardNode> hoveredCards = [];

	private readonly NodeController nodeController;

	private CardNode selectedCard;

	private readonly CraftingController CraftingController;


	// Constructor
	public CardController(NodeController nodeController) {
		this.nodeController = nodeController;
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

	/// <summary>
	/// Creates a new card and adds it to the scene.
	/// </summary>
	/// <returns>
	/// The created card instance.
	/// </returns>
	public void CreateCard(string type) {
		CardCreationHelper.CreateCard(type);
	}

	/// <summary>
	/// Checks if the card is the top card on the scene.
	/// </summary>
	private bool CardIsTopCard(Node2D cardNode) {
		foreach (CardNode node in hoveredCards)
			if (node.ZIndex > cardNode.ZIndex)
				return false;
		return true;
	}

	/// <summary>
	/// Generates a new UUID (Universally Unique Identifier) string.
	/// </summary>
	public static string GenerateUUID() {
		return Guid.NewGuid().ToString();
	}

	/// <summary>
	/// Adds the card to the hovered cards list and sets its highlighted state to true.
	/// </summary>
	public void AddCardToHoveredCards(CardNode cardNode) {
		hoveredCards.Add(cardNode);
		CheckForHighLight();
	}

	/// <summary>
	/// Removes the card from the hovered cards list and sets its highlighted state to false.
	/// </summary>
	public void RemoveCardFromHoveredCards(CardNode cardNode) {
		hoveredCards.Remove(cardNode);
		CheckForHighLight();
		cardNode.SetHighlighted(false);
	}

	/// <summary>
	/// Checks if the card is the top card on the scene which the mouse is hovering over and sets the highlighted state.
	/// </summary>
	public void CheckForHighLight() {
		foreach (CardNode card in hoveredCards)
			if (CardIsTopCard(card))
				card.SetHighlighted(true);
			else
				card.SetHighlighted(false);
	}

	/// <summary>
	/// Gets the top card at the mouse position.
	/// </summary>
	/// <returns>
	/// The top card at the mouse position or null if no card is hovered. 
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
	/// Gets the card under the moved card.
	/// </summary>
	/// <returns>
	/// The card under the moved card or null if no card is found.
	/// </returns>
	private CardNode GetCardUnderMovedCard() {
		IReadOnlyCollection<CardNode> hoveredCardsSorted = selectedCard.HoveredCardsSorted;

		CardNode topUnderCard = null;

		foreach (CardNode card in hoveredCardsSorted) {
			if (card.ZIndex < selectedCard.ZIndex && (topUnderCard == null || card.ZIndex > topUnderCard.ZIndex)) {
				topUnderCard = card;
			}
		}

		return topUnderCard;
	}

	/// <summary>
	/// Called when the left mouse button is pressed.
	/// </summary>
	public void LeftMouseButtonPressed() {
		selectedCard = GetTopCardAtMousePosition();

		if (selectedCard != null) SetZIndexForAllCards(selectedCard);

		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(true);

			if (selectedCard.HasNeighbourAbove()) {
				selectedCard.IsMovingOtherCards = true;
			}
			else {
				// Set the card that is being dragged to the top
				selectedCard.ZIndex = CardCount + 1;
			}

			// Set the neighbour below to null if the card is moved to make the moved card able to get new neighbours
			// And sets the card below if it exists to not have a neighbour above
			if (selectedCard.HasNeighbourBelow()) {
				if (selectedCard.CardType is IStackable stackable) {
					if (stackable.NeighbourBelow != null) {
						stackable.NeighbourBelow.NeighbourAbove = null;
					}
					stackable.NeighbourBelow = null;
				}
			}
		}
	}

	/// <summary>
	/// Sets the ZIndex of all cards based on the selected card.
	/// </summary>
	/// <param name="cardNode">The card node to set the ZIndex from and its neighbours above.</param>
	public void SetZIndexForAllCards(CardNode cardNode) {
		List<IStackable> stackAbove = null;

		if (cardNode.CardType is IStackable stackable) {
			stackAbove = stackable.StackAbove;
		}

		int counterForStackedCards = CardCount - (stackAbove != null ? stackAbove.Count : 0);
		int counterForOtherCards = 1;

		foreach (CardNode card in AllCardsSorted) {
			if ((stackAbove != null && card is IStackable stackableCard && stackAbove.Contains(stackableCard)) || card == cardNode) {
				card.ZIndex = counterForStackedCards;
				counterForStackedCards++;
			}
			else {
				card.ZIndex = counterForOtherCards;
				counterForOtherCards++;
			}
		}
	}

	/// <summary>
	/// Called when the left mouse button is released.
	/// </summary>
	public void LeftMouseButtonReleased() {
		if (selectedCard != null) {
			selectedCard.SetIsBeingDragged(false);

			// Checks for a card under the moved card and sets if it exists as a neighbour below. 
			CardNode underCard = GetCardUnderMovedCard();

			if (underCard != null && !selectedCard.HasNeighbourBelow() && !underCard.HasNeighbourAbove()) {
				selectedCard.SetOverLappedCardToStack(underCard);
			}

			selectedCard = null;
		}

		CheckForCrafting();
	}

	/// <summary>
	/// Used to print the cards and their neighbours for debugging purposes.
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

	public void CheckForCrafting() {
		List<List<CardNode>> allStacks = GetAllCardStacks();

		foreach (List<CardNode> stack in allStacks) {
			if(stack.Count <= 1) continue;

			List<Card> cards = [];

			foreach (CardNode card in stack) {
				if (card.CardType is Card cardType) {
					cards.Add(cardType);
				}
			}

			List<string> craftedCard = CraftingController.CheckForCrafting(cards);

			if (craftedCard != null) {
				foreach (string cardName in craftedCard) {
					GD.Print("Crafted: " + cardName);

					foreach (CardNode card in stack) {
						GD.Print("Removing card: " + card.CardType.TextureType);
						hoveredCards.Remove(card);
						card.QueueFree();
					}

					CardCreationHelper.CreateCard(cardName);
				}
			}else{
				GD.Print("No crafting possible");
			}
		}
	}

	public List<List<CardNode>> GetAllCardStacks() {
		List<List<CardNode>> cardStacks = [];

		foreach (CardNode card in AllCardsSorted) {
			if (card.CardType is IStackable stackable && stackable.NeighbourBelow == null) {
				List<CardNode> stack = [];

				stack.Add(card);

				List<IStackable> stackAbove = stackable.StackAbove;

				foreach (IStackable stackableCard in stackAbove) {
					if (stackableCard is Card stackCard) {
						stack.Add(stackCard.CardNode);
					}
				}

				cardStacks.Add(stack);
			}
		}

		return cardStacks;
	}
}