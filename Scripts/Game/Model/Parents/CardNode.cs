using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;
using Goodot15.Scripts.Game.View;
using Vector2 = Godot.Vector2;

/// <summary>
///     Represents a card node in the game.
///     It inherits from Node2D and is used to represent a card in the game.
/// </summary>
public partial class CardNode : Node2D {
    private static int startingZIndex;


    /// <summary>
    ///     Constructs a new CardNode object
    /// </summary>
    public CardNode() {
        AddToGroup(CardController.CARD_GROUP_NAME);

        ZIndex = startingZIndex;
        startingZIndex = ++startingZIndex % 1024;
    }

    /// <summary>
    ///     Gets the card node from the area2D.
    ///     This is used to get the card node from the area2D when the mouse enters or exits the area2D.
    /// </summary>
    /// <param name="area2D"></param>
    /// <returns>
    ///     The card node that is the parent of the area2D.
    /// </returns>
    public static CardNode GetCardNodeFromArea2D(Area2D area2D) {
        return area2D.GetParent<CardNode>();
    }

    #region Generic instance methods

    public void Destroy() {
        ClearReferences();
        QueueFree();
    }

    #endregion Generic instance methods


    #region Drag logic

    /// <summary>
    ///     Sets the highlighted state of the card node.
    ///     It sets the modulate of the sprite to the highlighted color if the card is highlighted.
    /// </summary>
    public void SetHighlighted(bool isHighlighted) {
        switch (isHighlighted) {
            case true when !oldIsHighlighted:
                CardSprite.SetModulate(CardSprite.Modulate * HIGHTLIGHT_MODULATE_FACTOR);
                oldIsHighlighted = true;
                break;
            case false when oldIsHighlighted:
                oldIsHighlighted = false;
                CardSprite.SetModulate(CardSprite.Modulate / HIGHTLIGHT_MODULATE_FACTOR);
                break;
        }
    }


    /// <summary>
    ///     Processes the card node and checks if the card node is being dragged.
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta) {
        if (!Dragged) return;
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 mousePositionDelta = mousePosition - oldMousePosition;

        if (BottomCardOfStack.Position.Y <= 64 && mousePositionDelta.Y < 0) mousePositionDelta.Y = 0;
        ClampPositionInGameSpace(mousePositionDelta);

        UpdateCraftButtonPosition();
        UpdateCardPositions();

        oldMousePosition = mousePosition;
    }

    private void UpdateCraftButtonPosition() {
        if (CraftButton is not null) CraftButton.Position = Position + CardController.CRAFT_BUTTON_OFFSET;
    }

    /// <summary>
    ///     "Recursively" updates each card position to account for their stack position
    /// </summary>
    private void UpdateCardPositions() {
        int indexInStack = 0;
        foreach (CardNode cardNode in StackAbove) cardNode.Position = Position + CARD_OVERLAP_OFFSET * ++indexInStack;
    }

    public override void _PhysicsProcess(double delta) {
        ITickable tickableCardType = CardType as ITickable;
        tickableCardType?.PreTick();
        tickableCardType?.PostTick();
    }

    private void ClampPositionInGameSpace(Vector2 mousePositionDelta) {
        Position = new Vector2(
            Math.Clamp(Position.X + mousePositionDelta.X, 0 + CardSize.X / 2, 1280 - CardSize.X / 2),
            Math.Clamp(Position.Y + mousePositionDelta.Y, 0 + CardSize.Y / 2,
                720 - (CardSize.Y + StackAbove.Count * 40) / 2)
        );
    }

    #endregion Drag logic

    #region Constants

    private const string SELL_SFX = "General Sounds/Coins/sfx_coin_double2.wav";

    private const float HIGHTLIGHT_MODULATE_FACTOR = 1.3f;
    private static readonly Vector2 CARD_OVERLAP_OFFSET = Vector2.Down * 20;

    #endregion Constants

    #region External instance references

    /// <summary>
    ///     Static reference to the CardController instance. Gathered from <see cref="GameController.Singleton" />
    /// </summary>
    public static CardController CardController => GameController.Singleton.CardController;

    /// <summary>
    ///     Card sprite reference; Holds the texture for the card
    /// </summary>
    public Sprite2D CardSprite => GetNode<Sprite2D>("Sprite2D");

    /// <summary>
    ///     Card Area2D reference; Used for mouse enter/exit events and card overlap checks.
    /// </summary>
    public Area2D CardArea2D => GetNode<Area2D>("Area2D");

    /// <summary>
    ///     Reference to the connected CraftButton
    /// </summary>
    public CraftButton CraftButton { get; set; }

    #endregion External instance references

    #region Basic card properties

    private bool oldIsHighlighted;
    private Vector2 oldMousePosition;

    public bool MouseIsHovering { get; private set; }

    private bool dragged;

    /// <summary>
    ///     Gets or sets if this Card instance is being dragged
    /// </summary>
    public bool Dragged {
        get => dragged;
        set {
            dragged = value;
            OnDragChanged(dragged);
        }
    }

    #endregion Basic card properties

    #region Underlying Card Data

    public Vector2 CardSize => CardSprite?.Texture?.GetSize() ?? new Vector2(80, 128);

    private Card cardType;

    /// <summary>
    ///     The card type this <see cref="CardNode" /> instance is.<br />
    ///     Changing this property will automatically update the texture of this Card
    /// </summary>
    public Card CardType {
        get => cardType;
        set {
            if (cardType is not null) cardType.CardNode = null;

            value.CardNode = this;
            cardType = value;
            UpdateCardTexture();
        }
    }

    /// <summary>
    ///     Applies the texture to the sprite of the card node.
    ///     It tries to load the texture from the address of the card type.
    ///     If the texture is not found, it loads the error texture.
    /// </summary>
    private void UpdateCardTexture() {
        Texture2D texture;
        // Check if the path is not null or empty and if there is a file at the path
        if (string.IsNullOrEmpty(CardType.TexturePath) || !ResourceLoader.Exists(CardType.TexturePath)) {
            GD.PrintErr("Texture path is null or empty for card: " + CardType.CardNode.CardType + " " +
                        string.IsNullOrEmpty(CardType.TexturePath) + " " + FileAccess.FileExists(CardType.TexturePath));
            GD.PrintErr("Expected Texture path: " + CardType.TexturePath);
            texture = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Error.png");
            CardSprite.Texture = texture;
            return;
        }

        // try to load the texture from the address
        try {
            texture = GD.Load<Texture2D>(CardType.TexturePath);
        }
        catch (Exception) {
            texture = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Error.png");
            GD.PrintErr("Texture not found for card: " + CardType.TexturePath);
        }

        CardSprite.Texture = texture;
    }

    /// <summary>
    ///     Runs logic for checking for card consumption capability and logic
    /// </summary>
    /// <returns></returns>
    private bool ExecuteCardConsumptionLogic() {
        CardNode cardUnder = OverlappingCard;

        if (cardUnder?.CardType is not ICardConsumer cardConsumer) return false;
        if (!cardConsumer.ConsumeCard(CardType)) return false;

        Destroy();
        return true;
    }

    #endregion

    #region Stack-related properties

    private const string ON_PICKUP_SFX = "8bit_Sounds/sfx_movement_stairs1a.wav";
    private const string ON_DROP_SFX = "8bit_Sounds/sfx_movement_stairs1b.wav";
    private const string ON_STACK_SFX = "General Sounds/Impacts/sfx_sounds_impact1.wav";

    /// <summary>
    ///     Gets the closest overlapping card with the highest ZIndex.
    /// </summary>
    private CardNode OverlappingCard => CardArea2D.GetOverlappingAreas().Select(GetCardNodeFromArea2D)
        .Where(e => e.ZIndex < ZIndex).OrderByDescending(e => e.ZIndex).FirstOrDefault();

    private CardNode cardAbove;
    private CardNode cardBelow;

    /// <summary>
    ///     The Card reference above this <see cref="CardNode" /> instance. Null if no card or disposed/dead card.<br />
    ///     Setting the value also updates the other card's <see cref="NeighbourBelow" /> value to this <see cref="CardNode" />
    ///     reference
    /// </summary>
    public CardNode NeighbourAbove {
        get => IsInstanceValid(cardAbove) ? cardAbove : null;
        set {
            // Setting it to null means clearing the reference as well
            if (value is null) {
                if (HasNeighbourAbove) cardAbove.cardBelow = null;
                cardAbove = null;
                return;
            }

            cardAbove = value;
            cardAbove.cardBelow = this;
        }
    }

    /// <summary>
    ///     The Card reference below this <see cref="CardNode" /> instance. Null if no card or disposed/dead card.<br />
    ///     Setting the value also updates the other card's <see cref="NeighbourAbove" /> value to this <see cref="CardNode" />
    ///     reference
    /// </summary>
    public CardNode NeighbourBelow {
        get => IsInstanceValid(cardBelow) ? cardBelow : null;
        set {
            // Setting it to null means clearing the reference as well
            if (value is null) {
                if (HasNeighbourBelow) cardBelow.cardAbove = null;
                cardBelow = null;
                return;
            }

            cardBelow = value;
            cardBelow.cardAbove = this;
        }
    }

    /// <summary>
    ///     Sets the position of the card node to the given position.
    /// </summary>
    public bool HasNeighbourAbove => NeighbourAbove is not null;

    /// <summary>
    ///     Checks if the card has a neighbour below.
    /// </summary>
    public bool HasNeighbourBelow => NeighbourBelow is not null;

    #region Stack collection getters

    /// <summary>
    ///     Traverse the stack from this <see cref="CardNode" /> instance, both forward and backwards.<br />
    ///     Gets the current entire stack collection
    /// </summary>
    public IReadOnlyCollection<CardNode> Stack =>
        new List<CardNode>(StackBelow).Append(this).Union(StackAbove).ToArray();

    /// <summary>
    ///     Traverse the stack from this <see cref="CardNode" /> instance, forwards only.<br />
    ///     Gets the current stack collection above
    /// </summary>
    public IReadOnlyList<CardNode> StackBelow {
        get {
            IList<CardNode> stackBackwards = [];

            CardNode current = this;
            // Traverse backwards
            while (current is not null && current.HasNeighbourBelow) {
                CardNode next = current.NeighbourBelow;
                stackBackwards.Add(next);
                current = next;
            }

            return stackBackwards.Reverse().ToArray();
        }
    }

    /// <summary>
    ///     Traverse the stack from this <see cref="CardNode" /> instance, backwards only.<br />
    ///     Gets the current stack collection below
    /// </summary>
    public IReadOnlyList<CardNode> StackAbove {
        get {
            IList<CardNode> stackForwards = [];

            CardNode current = this;
            // Traverse forwards
            while (current is not null && current.HasNeighbourAbove) {
                CardNode next = current.NeighbourAbove;
                stackForwards.Add(next);
                current = next;
            }

            return stackForwards.ToArray();
        }
    }

    /// <summary>
    ///     Similar to <see cref="StackAbove" />; But also includes this instance in the collection, positioned first in the
    ///     collection.
    /// </summary>
    public IReadOnlyList<CardNode> StackAboveWithItself => new List<CardNode>([this]).Concat(StackAbove).ToArray();

    /// <summary>
    ///     Similar to <see cref="StackBelow" />; But also includes this instance in the collection, positioned last in the
    ///     collection.
    /// </summary>
    public IReadOnlyList<CardNode> StackBelowWithItself => new List<CardNode>(StackBelow).Append(this).ToArray();

    /// <summary>
    ///     Gets the <see cref="CardNode" /> at the bottom of the stack.
    /// </summary>
    public CardNode BottomCardOfStack => StackBelowWithItself[0];

    /// <summary>
    ///     Gets the <see cref="CardNode" /> at the top the stack.
    /// </summary>
    public CardNode TopCardOfStack => StackAboveWithItself[^1];

    #endregion Stack collection getters

    #region Stack-related methods

    /// <summary>
    ///     Clears references and unlinks it from a stack. Ensures that the object can be cleaned up safely and does not leak
    ///     and have dead references
    /// </summary>
    private void ClearReferences() {
        // Clear references
        NeighbourAbove = null;
        NeighbourBelow = null;

        CardController.OnCardUnhovered(this);
    }

    /// <summary>
    ///     Runs stacking logic - attempting to stack on top of other cards if possible
    /// </summary>
    private void ExecuteStackingLogic() {
        if (Dragged) {
            NeighbourBelow = null;
            UpdateZIndex();
        } else {
            if (OverlappingCard is not null && !OverlappingCard.HasNeighbourAbove &&
                (CardType?.CanStackBelow(OverlappingCard.CardType) ?? false) &&
                (OverlappingCard.CardType?.CanStackAbove(CardType) ?? false)) {
                NeighbourBelow = OverlappingCard;

                GameController.Singleton.SoundController.PlaySound(ON_STACK_SFX);
            }

            ResetZIndex();

            if (HasNeighbourBelow) NeighbourBelow.UpdateCardPositions();
        }
    }

    #region Z-Index logic

    /// <summary>
    ///     Resets this card instance ZIndex to 1 and forces the stack to update its ZIndex
    /// </summary>
    private void ResetZIndex() {
        BottomCardOfStack.ZIndex = 1;
        BottomCardOfStack.UpdateZIndexForStack();
    }

    private void UpdateZIndex() {
        ZIndex = CardController.AllCards.Max(c => c.ZIndex) + 1;
        UpdateZIndexForStack();
    }

    /// <summary>
    ///     Stack will update its ZIndex from this <see cref="CardNode" /> instance being called. Cards will get <b>z+p</b>
    ///     ZIndex value where z is this instance ZIndex and P being position in stack
    /// </summary>
    private void UpdateZIndexForStack() {
        int zIndexCounter = ZIndex;
        foreach (CardNode cardNode in StackAbove) cardNode.ZIndex = ++zIndexCounter;
    }

    #endregion Z-Index logic

    #endregion Stack-related methods

    #endregion Stack-related properties

    #region Events

    /// <summary>
    ///     Triggers and executes logic when the card is picked up or dropped
    /// </summary>
    private void OnDragChanged(bool newDragValue) {
        if (!IsInstanceValid(this) || IsQueuedForDeletion()) return;

        GameController.Singleton.SoundController.PlaySound(newDragValue ? ON_PICKUP_SFX : ON_DROP_SFX);

        oldMousePosition = GetGlobalMousePosition();

        // Executed once a card is dropped (no longer being dragged)
        if (!newDragValue) ExecuteCardConsumptionLogic();

        ExecuteStackingLogic();


        if (newDragValue && CardType is CardLiving) CardController.HideHealthAndHunger();
    }

    /// <summary>
    ///     Rewards the player (if possible to sell) and also invokes <see cref="Destroy" />.
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public bool Sell() {
        if ((CardType?.Value ?? -1) < 0) return false;
        int cardValue = CardType.Value;
        Global.Singleton.AddMoney(cardValue);
        GameController.Singleton.HUD.ShowFloatingMoneyLabel(cardValue);

        SoundController.Singleton.PlaySound(SELL_SFX);

        Destroy();
        return true;
    }

    /// <summary>
    ///     Updates <see cref="MouseIsHovering" /> to true when hovered over
    /// </summary>
    public void OnMouseEnteredCard() {
        MouseIsHovering = true;
        CardController.OnCardHovered(this);
    }

    /// <summary>
    ///     Updates <see cref="MouseIsHovering" /> to false when mouse no longer hovers over
    /// </summary>
    public void OnMouseExitedCard() {
        MouseIsHovering = false;
        CardController.OnCardUnhovered(this);
    }

    public override void _Ready() {
        CardArea2D.MouseEntered += OnMouseEnteredCard;
        CardArea2D.MouseExited += OnMouseExitedCard;
    }

    #endregion Events
}