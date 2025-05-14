using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Parents;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

public class CardController {
    public const string CARD_GROUP_NAME = "CARDS";

    public static readonly Vector2 CRAFT_BUTTON_OFFSET = new(0, -110);
    public static readonly Vector2 CARD_LIVING_OVERLAY_OFFSET = new(-67, 70);

    private readonly List<CardNode> hoveredCards = [];

    private CardLivingOverlay currentOverlay;
    private Timer overlayUpdateTimer;

    private CardNode selectedCard;


    // Constructor
    public CardController(GameController gameController, MouseController mouseController) {
        GameController = gameController;
        MouseController = mouseController;
        CardCreationHelper = new CardCreationHelper(gameController);
        CraftingController = new CraftingController(CardCreationHelper);
    }

    public CardCreationHelper CardCreationHelper { get; }
    public CraftingController CraftingController { get; }
    public GameController GameController { get; }
    public MouseController MouseController { get; }

    public int CardCount => AllCards.Count;

    public IReadOnlyCollection<CardNode> AllCards =>
        GameController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

    public IReadOnlyCollection<CardNode> AllCardsSorted => AllCards.OrderBy(x => x.ZIndex).ToArray();

    private IReadOnlyCollection<CardNode> Stacks =>
        AllCards.Where(x => x.HasNeighbourAbove && !x.HasNeighbourBelow).ToArray();

    /// <summary>
    ///     Sets the ZIndex of all cards based on the selected card.
    /// </summary>
    /// <param name="cardNode">The card node to set the ZIndex from and its neighbours above.</param>
    public void SetZIndexForAllCards(CardNode cardNode) {
        int NumberOfCards = AllCards.Count;
        IEnumerable<CardNode> stackAboveSelectedCard = cardNode.StackAbove;
        //cardNode.CardType is IStackable stackableCard ? stackableCard.StackAbove : null;
        int numberOfCardsAbove = stackAboveSelectedCard?.Count() ?? 0;
        int counterForCardsAbove = NumberOfCards - numberOfCardsAbove;
        int counterForCardsBelow = 1;

        foreach (CardNode card in AllCardsSorted)
            if (card == cardNode)
                card.ZIndex = !card.HasNeighbourAbove ? NumberOfCards : counterForCardsAbove++;
            else if (stackAboveSelectedCard is not null &&
                     stackAboveSelectedCard.Contains(card))
                card.ZIndex = counterForCardsAbove++;
            else
                card.ZIndex = counterForCardsBelow++;
    }

    /// <summary>
    ///     Checks if the card is the top card on the scene.
    /// </summary>
    private bool CardIsTopCard(CardNode cardNode) {
        return hoveredCards.All(node => node.ZIndex <= cardNode.ZIndex);
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
        if (cardNode.CardType is CardLiving) HideHealthAndHunger();
    }

    /// <summary>
    ///     Checks if the card is the top card on the scene which the mouse is hovering over and sets the highlighted state.
    /// </summary>
    public void CheckForHighLight() {
        foreach (CardNode card in hoveredCards)
            if (CardIsTopCard(card)) {
                card.SetHighlighted(true);
                if (!card.Dragged && card.CardType is CardLiving cardLiving) ShowHealthAndHunger(cardLiving);
            } else {
                card.SetHighlighted(false);
            }
    }

    private void ShowHealthAndHunger(CardLiving cardLiving) {
        HideHealthAndHunger();

        PackedScene cardLivingOverlay = GD.Load<PackedScene>("res://Scenes/ProgressBars/CardLivingOverlay.tscn");
        currentOverlay = cardLivingOverlay.Instantiate<CardLivingOverlay>();

        currentOverlay.Position = cardLiving.CardNode.Position + CARD_LIVING_OVERLAY_OFFSET;


        currentOverlay.UpdateHealthBar(cardLiving.Health, cardLiving.BaseHealth);
        currentOverlay.UpdateSaturationBar(cardLiving.Saturation, cardLiving.MaximumSaturation);

        // Add the overlay to the same parent as the card
        cardLiving.CardNode.GetParent().AddChild(currentOverlay);

        overlayUpdateTimer = new Timer();
        overlayUpdateTimer.WaitTime = 0.2f;
        overlayUpdateTimer.Autostart = true;
        overlayUpdateTimer.OneShot = false;
        cardLiving.CardNode.GetParent().AddChild(overlayUpdateTimer);
        overlayUpdateTimer.Timeout += () => {
            currentOverlay.UpdateHealthBar(cardLiving.Health, cardLiving.BaseHealth);
            currentOverlay.UpdateSaturationBar(cardLiving.Saturation, cardLiving.MaximumSaturation);
        };
    }

    public void HideHealthAndHunger() {
        if (overlayUpdateTimer is not null) {
            overlayUpdateTimer.QueueFree();
            overlayUpdateTimer = null;
        }

        // Remove the existing overlay if it exists
        if (currentOverlay is not null && currentOverlay.IsInsideTree()) {
            currentOverlay.QueueFree();
            currentOverlay = null;
        }
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
            if (topCard is null)
                topCard = card;
            else if (card.GetZIndex() > topCard.GetZIndex()) topCard = card;

        return topCard;
    }

    ///// <summary>
    /////     Gets the card under the moved card.
    ///// </summary>
    ///// <returns>
    /////     The card under the moved card or null if no card is found.
    ///// </returns>
    //private CardNode GetCardUnderMovedCard() {
    //    IReadOnlyCollection<CardNode> hoveredCardsSorted = selectedCard.HoveredCardsSorted;
//
    //    CardNode topUnderCard = null;
//
    //    foreach (CardNode card in hoveredCardsSorted)
    //        if (card.ZIndex < selectedCard.ZIndex && (topUnderCard is null || card.ZIndex > topUnderCard.ZIndex))
    //            topUnderCard = card;
//
    //    return topUnderCard;
    //}

    /// <summary>
    ///     Called when the left mouse button is pressed.
    /// </summary>
    public void LeftMouseButtonPressed() {
        MouseController.SetMouseCursor(MouseCursorEnum.hand_close);
        selectedCard = GetTopCardAtMousePosition();
        if (selectedCard is null) return;

        switch (GameController.SellModeActive)
        {
            case true when selectedCard.CardType.Value >= 0:
                Global.Singleton.AddMoney(selectedCard.CardType.Value);
                selectedCard.Destroy();
                return;
            case false:
                selectedCard.Dragged = true;
                break;
        }

        // if (!GodotObject.IsInstanceValid(selectedCard) || selectedCard.IsQueuedForDeletion()) {
        //     selectedCard = null;
        //     return;
        // }
// 
        // if (selectedCard is not null) {
        //     SetZIndexForAllCards(selectedCard);
        //     selectedCard.Dragged = true;
// 
        //     if (selectedCard.HasNeighbourAbove)
        //         selectedCard.IsMovingOtherCards = true;
        //     else
        //         // Set the card that is being dragged to the top
        //         selectedCard.ZIndex = CardCount + 1;

        // Set the neighbour below to null if the card is moved to make the moved card able to get new neighbours
        // And sets the card below if it exists to not have a neighbour above
        // TODO: ?
        // if (selectedCard.HasNeighbourBelow)
        //     if (selectedCard.CardType is IStackable stackable) {
        //         if (stackable.NeighbourBelow is not null) stackable.NeighbourBelow.NeighbourAbove = null;
        //         stackable.NeighbourBelow = null;
        //     }
    }

    /// <summary>
    ///     Called when the left mouse button is released.
    /// </summary>
    public void LeftMouseButtonReleased() {
        MouseController.SetMouseCursor(MouseCursorEnum.point_small);
        if (selectedCard is not null) selectedCard.Dragged = false;
        // // Checks for a card under the moved card and sets if it exists as a neighbour below. 
        // CardNode underCard = GetCardUnderMovedCard();
        // if (underCard is not null && !selectedCard.HasNeighbourBelow) {
        //     selectedCard.SetOverLappedCardToStack(underCard);
        // } else {
        //     selectedCard.NeighbourBelow = null;
        // }
        // Checks if a card is supposed to have a craft button above it
        foreach (CardNode card in AllCards)
            if (Stacks.Contains(card) &&
                CraftingController.CheckForCraftingWithStackable(card.StackAboveWithItself.Select(e => e.CardType)
                    .ToArray()) is not null) {
                AddCraftButton(card);
            } else {
                if (card.CraftButton is not null) {
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
        if (cardNode is null) return;

        if (cardNode.CraftButton is not null) {
            cardNode.CraftButton.QueueFree();
            cardNode.CraftButton = null;
        }

        // if (cardNode.CardType is not IStackable stackable) return;

        // Check for the recipe
        Pair<IReadOnlyList<string>, bool> recipe =
            CraftingController.CheckForCraftingWithStackable(cardNode.StackAboveWithItself.Select(e => e.CardType)
                .ToArray());

        if (recipe.Left is null || recipe.Left.Count == 0) {
            GD.Print("No recipe found for the selected card.");
            return;
        }

        Vector2 spawnPos = cardNode.Position;

        // Remove the cards in the stack part of cardNode
        foreach (CardNode cardInStackAbove in cardNode.StackAboveWithItself)
            if (cardInStackAbove.CardType is Card card) {
                // cardInStackAbove.ClearNeighbours();

                if (cardInStackAbove.CardType is IDurability durability) {
                    bool ret = durability.DecrementDurability();

                    GD.Print("Ret: " + recipe.Right + " " + ret);

                    if (ret || recipe.Right) card.CardNode.Destroy();

                    continue;
                }

                card.CardNode.Destroy();
            }

        foreach (string cardName in recipe.Left) {
            CardNode card = CreateCard(cardName, spawnPos);
            card.ZIndex = cardNode.ZIndex + 1;
            spawnPos += new Vector2(0, -15);

            card.NeighbourAbove = null;
            card.NeighbourBelow = null;
            // if (card.CardType is IStackable craftedStackable) {
            //     craftedStackable.NeighbourAbove = null;
            //     craftedStackable.NeighbourBelow = null;
            // }
        }
    }

    #region Specific Card

    /// <summary>
    ///     Adds a craft button to the specified card node.
    /// </summary>
    /// <param name="cardNode">The card node to add the craft button to.</param>
    private void AddCraftButton(CardNode cardNode) {
        if (cardNode.CraftButton is not null) return;

        PackedScene craftButtonScene = GD.Load<PackedScene>("res://Scenes/CraftButton.tscn");
        CraftButton craftButtonInstance = craftButtonScene.Instantiate<CraftButton>();

        craftButtonInstance.Position = cardNode.Position + CRAFT_BUTTON_OFFSET;

        cardNode.CraftButton = craftButtonInstance;

        craftButtonInstance.CardNode = cardNode;

        craftButtonInstance.CardController = this;

        GameController.AddChild(craftButtonInstance);
    }

    #endregion Specific Card

    #region Create Card

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
        // CardNode.CardController = this;

        cardInstance.Position = position;
        if (cardInstance.GetParent() is not null) cardInstance.GetParent().RemoveChild(cardInstance);
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

        // CardNode.CardController = this;

        cardInstance.ZIndex = CardCount + 1;
        GameController.AddChild(cardInstance);

        return cardInstance;
    }

    #endregion Create Card
}