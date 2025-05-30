using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Living;
using Goodot15.Scripts.Game.Model.Parents;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

public class CardController {
    public const string CARD_GROUP_NAME = "CARDS";

    public static readonly Vector2 CRAFT_BUTTON_OFFSET = new(0, -110);
    public static readonly Vector2 CARD_LIVING_OVERLAY_OFFSET = new(-67, 70);
    public static readonly Vector2 CARD_VALUE_OVERLAY_OFFSET = new(-40, -92);

    private CardLivingOverlay currentOverlay;
    private Timer overlayUpdateTimer;

    private CardNode selectedCard;
    private CardValueOverlay valueOverlay;


    // Constructor
    public CardController(GameController gameController, MouseController mouseController,
        MenuController menuController) {
        GameController = gameController;
        MouseController = mouseController;
        CardCreationHelper = new CardCreationHelper(gameController);
        CraftingController = new CraftingController();
        this.menuController = menuController;
    }

    public IReadOnlyCollection<CardNode> HoveredCards => AllCards.Where(e => e.MouseIsHovering).ToArray();

    public CardCreationHelper CardCreationHelper { get; }
    public CraftingController CraftingController { get; }
    public GameController GameController { get; }
    public MouseController MouseController { get; }
    private MenuController menuController { get; }

    public int CardCount => AllCards.Count;

    public IReadOnlyCollection<CardNode> AllCards =>
        GameController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

    public IReadOnlyCollection<CardNode> AllCardsSorted => AllCards.OrderBy(x => x.ZIndex).ToArray();

    private IReadOnlyCollection<CardNode> Stacks =>
        AllCards.Where(x => x.HasNeighbourAbove && !x.HasNeighbourBelow).ToArray();

    private int NumberOfPlayerCards =>
        AllCards.Count(x => x.CardType is LivingPlayer);


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
                card.ZIndex = !card.HasNeighbourAbove
                    ? NumberOfCards
                    : counterForCardsAbove++;
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
        return HoveredCards.All(node => node.ZIndex <= cardNode.ZIndex);
    }

    /// <summary>
    ///     Fired when a mouse enters the boundary of a card.
    /// </summary>
    /// <param name="cardNodeInstance">The CardNode instance to be added</param>public void OnCardHovered(CardNode cardNodeInstance) {
    public void OnCardHovered(CardNode cardNodeInstance) {
        UpdateHighlights(cardNodeInstance);
        UpdateOverlays(cardNodeInstance);
    }

    /// <summary>
    ///     Fired when a card is being destroyed through <see cref="CardNode.Destroy" />.
    /// </summary>
    /// <param name="cardNodeRemoving">Card node being removed</param>
    public void CardRemoved(CardNode cardNodeRemoving) {
        UpdateHighlights(cardNodeRemoving);
        UpdateOverlays(cardNodeRemoving);
    }

    /// <summary>
    ///     Fired when the mouse leaves the boundary of a card.
    /// </summary>
    public void OnCardUnhovered(CardNode cardNodeInstance) {
        UpdateHighlights(cardNodeInstance);
        UpdateOverlays(cardNodeInstance);
    }
        private void UpdateOverlays(CardNode cardNodeInstance) {
        if (!CardIsTopCard(cardNodeInstance)) return;
        if (cardNodeInstance.MouseIsHovering) {
            if (!cardNodeInstance.Dragged && !cardNodeInstance.HasNeighbourAbove &&
                cardNodeInstance.CardType is CardLiving cardLiving && !GameController.SellModeActive)
                ShowHealthAndHunger(cardLiving);
            if (GameController.SellModeActive)
                ShowCardValue(cardNodeInstance);
        } else { HideHealthAndHunger();
    HideCardValue();
        }
    }

    /// <summary>
    ///     Checks if the card is the top card on the scene which the mouse is hovering over and sets the highlighted state.
    /// </summary>
    public void UpdateHighlights(CardNode cardNodeInstance) {
        cardNodeInstance?.SetHighlighted(false);
        foreach (CardNode card in HoveredCards)
            if (CardIsTopCard(card)) 
                card.SetHighlighted(true);
                 else
                card.SetHighlighted(false);
            }
    

    private void ShowHealthAndHunger(CardLiving cardLiving) {
        HideHealthAndHunger();

        PackedScene cardLivingOverlay = GD.Load<PackedScene>("res://Scenes/ProgressBars/CardLivingOverlay.tscn");
        currentOverlay = cardLivingOverlay.Instantiate<CardLivingOverlay>();

        currentOverlay.Position = cardLiving.CardNode.Position + CARD_LIVING_OVERLAY_OFFSET;


        currentOverlay.UpdateHealthBar(cardLiving.Health, cardLiving.MaximumHealth);
        currentOverlay.UpdateSaturationBar(cardLiving.Saturation, cardLiving.MaximumSaturation);

        
        cardLiving.CardNode.GetParent().AddChild(currentOverlay);

        overlayUpdateTimer = new Timer();
        overlayUpdateTimer.WaitTime = 0.2f;
        overlayUpdateTimer.Autostart = true;
        overlayUpdateTimer.OneShot = false;
        cardLiving.CardNode.GetParent().AddChild(overlayUpdateTimer);
        overlayUpdateTimer.Timeout += () => {
            currentOverlay.UpdateHealthBar(cardLiving.Health, cardLiving.MaximumHealth);
            currentOverlay.UpdateSaturationBar(cardLiving.Saturation, cardLiving.MaximumSaturation);
        };
    }

    public void HideHealthAndHunger() {
        if (overlayUpdateTimer is not null) {
            overlayUpdateTimer.QueueFree();
            overlayUpdateTimer = null;
        }

        
        if (currentOverlay is not null && currentOverlay.IsInsideTree()) {
            currentOverlay.QueueFree();
            currentOverlay = null;
        }
    }

    /// <summary>
    ///     Shows value of a card that is being hovered, if sell mode is active.
    /// </summary>
    /// <param name="card">The card that is being hovered</param>
    private void ShowCardValue(CardNode card) {
        HideCardValue();

        PackedScene cardValueOverlay = GD.Load<PackedScene>("res://Scenes/ProgressBars/CardValueOverlay.tscn");
        valueOverlay = cardValueOverlay.Instantiate<CardValueOverlay>();

        valueOverlay.Position = card.Position + CARD_VALUE_OVERLAY_OFFSET;
        valueOverlay.SetValue(card.CardType.Value);

        card.GetParent().AddChild(valueOverlay);
    }

    /// <summary>
    ///     Hides the card value overlay if it exists.
    /// </summary>
    public void HideCardValue() {
        if (valueOverlay is null) return;
        valueOverlay.QueueFree();
        valueOverlay = null;
    }

    /// <summary>
    ///     Gets the top card at the mouse position.
    /// </summary>
    /// <returns>
    ///     The top card at the mouse position or null if no card is hovered.
    /// </returns>
    public CardNode GetTopCardAtMousePosition() {
        CardNode topCard = null;

        return HoveredCards.OrderByDescending(e => e.ZIndex).FirstOrDefault();
    }

    /// <summary>
    ///     Called when the left mouse button is pressed.
    /// </summary>
    public void LeftMouseButtonPressed() {
        MouseController.SetMouseCursor(MouseCursorIcon.HAND_CLOSE);
        selectedCard = GetTopCardAtMousePosition();
        if (selectedCard is null) return;

        if (!GameController.SellModeActive)
            selectedCard.Dragged = true;
        else
            selectedCard.Sell();
    }

    /// <summary>
    ///     Called when the left mouse button is released.
    /// </summary>
    public void LeftMouseButtonReleased() {
        MouseController.SetMouseCursor(MouseCursorIcon.POINT_SMALL);
        if (selectedCard is not null) selectedCard.Dragged = false;

        // Checks if a card is supposed to have a craft button above it
        foreach (CardNode card in AllCards)
            if (Stacks.Contains(card) &&
                CraftingController.CheckForCraftingWithStackable(card.StackAboveWithItself.Select(e => e.CardType)
                    .ToArray()) is not null) {
                AddCraftButton(card);
            } else {
                if (card.CraftButton is null) continue;
                card.CraftButton.QueueFree();
                card.CraftButton = null;
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
        Pair<IReadOnlyCollection<string>, IReadOnlyCollection<string>>? recipe =
            CraftingController.CheckForCraftingWithStackable(cardNode.StackAboveWithItself.Select(e => e.CardType)
                .ToArray());

        if (recipe.Left is null || recipe.Left.Count == 0) {
            GD.Print("No recipe found for the selected card.");
            return;
        }

        Vector2 spawnPos = cardNode.Position;

        // Remove the cards in the stack part of cardNode
        foreach (CardNode cardInStackAbove in cardNode.StackAboveWithItself)
            if (cardInStackAbove.CardType is not null) {
                if (recipe.Right.Contains(cardInStackAbove.CardType.CardName)) {
                    cardInStackAbove.Destroy();
                } else if (cardInStackAbove.CardType is IDurability durability) {
                    bool ret = durability.DecrementDurability();

                    if (ret) cardInStackAbove.Destroy();
                }
            }

        foreach (string cardName in recipe.Left) {
            CardNode card = CreateCard(cardName, spawnPos);
            card.ZIndex = cardNode.ZIndex + 1;
            spawnPos += new Vector2(0, -15);

            card.NeighbourAbove = null;
            card.NeighbourBelow = null;
        }
    }

    public void CheckForGameOver(bool livingHasJustDied = false) {
        int livingCardsAmount = NumberOfPlayerCards;

        if (livingHasJustDied) livingCardsAmount--;

        if (livingCardsAmount <= 0) menuController.OpenGameOverMenu();
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

        // craftButtonInstance.CardController = this;

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
        // if (cardInstance.GetParent() is not null) cardInstance.GetParent().RemoveChild(cardInstance);
        // GameController.AddChild(cardInstance);

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