using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Living;
using Goodot15.Scripts.Game.Model.Parents;

public class CardController {
    public const string CARD_GROUP_NAME = "CARDS";
    public readonly Vector2 CraftButtonOffset = new(0, -110);
    private readonly GameController GameController;
    private readonly List<CardNode> HoveredCards = [];
    private readonly MouseController MouseController;
    private CardNode SelectedCard;

    public CardController(GameController gameController, MouseController mouseController) {
        GameController = gameController;
        MouseController = mouseController;
        CardCreationHelper = new CardCreationHelper(gameController);
        CraftingController = new CraftingController(CardCreationHelper);
    }

    public CardCreationHelper CardCreationHelper { get; }
    public CraftingController CraftingController { get; }
    public int CardCount => AllCards.Count;

    public IReadOnlyCollection<CardNode> AllCards =>
        GameController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

    public IReadOnlyCollection<CardNode> AllCardsSorted => AllCards.OrderBy(x => x.ZIndex).ToArray();

    private List<CardNode> Stacks =>
        AllCards.Where(x => x.HasNeighbourAbove && !x.HasNeighbourBelow && x.CardType is IStackable).ToList();

    /// <summary>
    ///     Adds the card to the hovered cards list and sets its highlighted state to true.
    /// </summary>
    public void AddCardToHoveredCards(CardNode cardNode) {
        HoveredCards.Add(cardNode);
        CheckForHighLight();
    }

    /// <summary>
    ///     Sets the ZIndex of all cards based on the selected card.
    /// </summary>
    /// <param name="cardNode">The card node to set the ZIndex from and its neighbours above.</param>
    public void SetZIndexForAllCards(CardNode cardNode) {
        int NumberOfCards = AllCards.Count;
        List<IStackable> stackAboveSelectedCard =
            cardNode.CardType is IStackable stackableCard ? stackableCard.StackAbove : null;
        int NumberOfCardsAbove = stackAboveSelectedCard != null ? stackAboveSelectedCard.Count : 0;
        int CounterForCardsAbove = NumberOfCards - NumberOfCardsAbove;
        int CounterForCardsBelow = 1;

        foreach (CardNode card in AllCardsSorted)
            if (card == cardNode) {
                if (card.CardType is IStackable stackable) {
                    if (stackable.NeighbourAbove == null)
                        card.ZIndex = NumberOfCards;
                    else
                        card.ZIndex = CounterForCardsAbove++;
                } else {
                    card.ZIndex = NumberOfCards;
                }
            } else if (stackAboveSelectedCard != null && stackAboveSelectedCard.Contains(card.CardType as IStackable)) {
                card.ZIndex = CounterForCardsAbove++;
            } else {
                card.ZIndex = CounterForCardsBelow++;
            }
    }

    /// <summary>
    ///     Called when the left mouse button is pressed.
    /// </summary>
    public void LeftMouseButtonPressed() {
        MouseController.SetMouseCursor(MouseCursorEnum.hand_close);
        SelectedCard = GetTopCardAtMousePosition();

        if (SelectedCard != null) SetZIndexForAllCards(SelectedCard);

        if (SelectedCard != null) {
            SelectedCard.SetIsBeingDragged(true);

            if (SelectedCard.HasNeighbourAbove)
                SelectedCard.IsMovingOtherCards = true;
            else
                // Set the card that is being dragged to the top
                SelectedCard.ZIndex = CardCount + 1;

            // Set the neighbour below to null if the card is moved to make the moved card able to get new neighbours
            // And sets the card below if it exists to not have a neighbour above
            if (SelectedCard.HasNeighbourBelow)
                if (SelectedCard.CardType is IStackable stackable) {
                    if (stackable.NeighbourBelow != null) stackable.NeighbourBelow.NeighbourAbove = null;
                    stackable.NeighbourBelow = null;
                }
        }
    }

    /// <summary>
    ///     Called when the left mouse button is released.
    /// </summary>
    public void LeftMouseButtonReleased() {
        MouseController.SetMouseCursor(MouseCursorEnum.point_small);
        if (SelectedCard != null) {
            SelectedCard.SetIsBeingDragged(false);

            // Checks for a card under the moved card and sets if it exists as a neighbour below. 
            CardNode underCard = GetCardUnderMovedCard();

            if (underCard != null && !SelectedCard.HasNeighbourBelow && !underCard.HasNeighbourAbove)
                SelectedCard.SetOverLappedCardToStack(underCard);

            SelectedCard = null;
        }

        // Checks if a card is supposed to have a craft button above it
        foreach (CardNode card in AllCards)
            if (Stacks.Contains(card) && card.CardType is IStackable stackable &&
                CraftingController.CheckForCraftingWithStackable(stackable.StackAboveWithItself) != null) {
                AddCraftButton(card);
            } else {
                if (card.CraftButton != null) {
                    card.CraftButton.QueueFree();
                    card.CraftButton = null;
                }
            }
    }

    /// <summary>
    ///     Crafts a card from the specified card node.
    /// </summary>
    /// <param name="cardNode">The card node to craft from.</param>
    public void CraftCardFromSpecifiedCardNode(CardNode cardNode) {
        if (cardNode == null) return;

        if (cardNode.CraftButton != null) {
            cardNode.CraftButton.QueueFree();
            cardNode.CraftButton = null;
        }

        if (cardNode.CardType is not IStackable stackable) return;

        // Check for the recipe
        StringAndBoolRet recipe = CraftingController.CheckForCraftingWithStackable(stackable.StackAboveWithItself);

        if (recipe.StringList == null || recipe.StringList.Count == 0) {
            GD.Print("No recipe found for the selected card.");
            return;
        }

        Vector2 spawnPos = cardNode.Position;

        // Remove the cards in the stack part of cardNode
        foreach (IStackable stackableCard in stackable.StackAboveWithItself)
            if (stackableCard is Card card) {
                stackableCard.ClearNeighbours();
                if (stackableCard is CardBuilding || (stackableCard is LivingPlayer && !recipe.BoolValue)) continue;

                if (stackableCard is IDurability durability) {
                    bool ret = durability.DecrementDurability();

                    GD.Print("Ret: " + recipe.BoolValue + " " + ret);

                    if (ret || recipe.BoolValue) card.CardNode.QueueFree();

                    continue;
                }

                card.CardNode.QueueFree();
            }

        foreach (string cardName in recipe.StringList) {
            CardNode card = CreateCard(cardName, cardNode.Position);
            card.ZIndex = cardNode.ZIndex + 1;
            spawnPos += new Vector2(0, -15);
        }
    }

    #region CreateCard

    /// <summary>
    ///     Creates a new card and adds it to the scene, with a random underlying CardType
    /// </summary>
    /// <returns>
    ///     The created card instance.
    /// </returns>
    public CardNode CreateCard(Card card, Vector2 position = default) {
        ArgumentNullException.ThrowIfNull(card);

        CardNode cardInstance = CreateCard();

        cardInstance.CardType = card;
        cardInstance.CardController = this;

        cardInstance.Position = position;
        if (cardInstance.GetParent() != null) cardInstance.GetParent().RemoveChild(cardInstance);

        GameController.AddChild(cardInstance);

        return cardInstance;
    }

    public CardNode CreateCard(string cardType, Vector2 position = default) {
        return CreateCard(CardCreationHelper.GetCreatedInstanceOfCard(cardType), position);
    }

    /// <summary>
    ///     Creates a new card and adds it to the scene, with a random underlying CardType
    /// </summary>
    /// <returns>
    ///     The created card instance.
    /// </returns>
    private CardNode CreateCard() {
        // Create a new card by copying the card from Card scene and adding a instance of CardMaterial to it
        PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
        CardNode cardInstance = cardScene.Instantiate<CardNode>();

        cardInstance.CardController = this;

        cardInstance.ZIndex = CardCount + 1;
        GameController.AddChild(cardInstance);

        return cardInstance;
    }

    #endregion CreateCard

    #region Specific Card

    /// <summary>
    ///     Checks if the card is the top card on the scene.
    /// </summary>
    private bool CardIsTopCard(Node2D cardNode) {
        foreach (CardNode node in HoveredCards)
            if (node.ZIndex > cardNode.ZIndex)
                return false;

        return true;
    }

    /// <summary>
    ///     Removes the card from the hovered cards list and sets its highlighted state to false.
    /// </summary>
    public void RemoveCardFromHoveredCards(CardNode cardNode) {
        HoveredCards.Remove(cardNode);
        CheckForHighLight();
        cardNode.SetHighlighted(false);
    }

    /// <summary>
    ///     Checks if the card is the top card on the scene which the mouse is hovering over and sets the highlighted state.
    /// </summary>
    public void CheckForHighLight() {
        foreach (CardNode card in HoveredCards)
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

        foreach (CardNode card in HoveredCards)
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
        IReadOnlyCollection<CardNode> hoveredCardsSorted = SelectedCard.HoveredCardsSorted;

        CardNode topUnderCard = null;

        foreach (CardNode card in hoveredCardsSorted)
            if (card.ZIndex < SelectedCard.ZIndex && (topUnderCard == null || card.ZIndex > topUnderCard.ZIndex))
                topUnderCard = card;

        return topUnderCard;
    }

    /// <summary>
    ///     Adds a craft button to the specified card node.
    /// </summary>
    /// <param name="cardNode">The card node to add the craft button to.</param>
    private void AddCraftButton(CardNode cardNode) {
        if (cardNode.CraftButton != null) return;

        PackedScene craftButtonScene = GD.Load<PackedScene>("res://Scenes/CraftButton.tscn");
        CraftButton craftButtonInstance = craftButtonScene.Instantiate<CraftButton>();

        craftButtonInstance.Position = cardNode.Position + CraftButtonOffset;

        cardNode.CraftButton = craftButtonInstance;

        craftButtonInstance.CardNode = cardNode;

        craftButtonInstance.CardController = this;

        GameController.AddChild(craftButtonInstance);
    }

    #endregion Specific Card
}