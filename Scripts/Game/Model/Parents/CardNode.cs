using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game;
using Goodot15.Scripts.Game.Model.Interface;

/// <summary>
///     Represents a card node in the game.
///     It inherits from Node2D and is used to represent a card in the game.
/// </summary>
public partial class CardNode : Node2D {
    private const float HighLightFactor = 1.3f;

    private Card _cardType;

    private CraftButton _craftButton;

    private CardNode LastOverlappedCard;

    private bool oldIsHighlighted;

    private Vector2 oldMousePosition;

    public CardNode() {
        AddToGroup(CardController.CARD_GROUP_NAME);
    }

    public CardController CardController { get; set; }

    private Sprite2D sprite => GetNode<Sprite2D>("Sprite2D");


    private Area2D area2D => GetNode<Area2D>("Area2D");

    public Vector2 CardOverlappingOffset { get; private set; } = new(0, -20);

    public Card CardType {
        get => _cardType;
        set {
            if (_cardType is not null) _cardType.CardNode = null;

            value.CardNode = this;
            _cardType = value;
            ApplyTexture();
        }
    }

    public bool MouseIsHovering { get; private set; }

    public bool IsBeingDragged { get; private set; }

    public List<CardNode> HoveredCards { get; } = [];

    public IReadOnlyList<CardNode> HoveredCardsSorted => HoveredCards.OrderBy(x => x.ZIndex).ToList();

    public bool IsMovingOtherCards { get; set; } = false;

    public CraftButton CraftButton { get; set; }

    /// <summary>
    ///     Sets the position of the card node to the given position.
    /// </summary>
    public bool HasNeighbourAbove {
        get {
            if (CardType is IStackable stackable) return stackable.NeighbourAbove != null;
            return false;
        }
    }

    /// <summary>
    ///     Checks if the card has a neighbour below.
    /// </summary>
    public bool HasNeighbourBelow {
        get {
            if (CardType is IStackable stackable) return stackable.NeighbourBelow != null;
            return false;
        }
    }

    /// <summary>
    ///     Sets the position of the card node to the given position.
    /// </summary>
    public void SetIsBeingDragged(bool isBeingDragged) {
        oldMousePosition = GetGlobalMousePosition();
        IsBeingDragged = isBeingDragged;

        if (CardType is IStackable stackable) {
            CardNode neighbourAbove = ((Card)stackable.NeighbourAbove)?.CardNode;
            if (neighbourAbove == null)
                ZIndex = CardController.CardCount;
            else
                neighbourAbove.SetIsBeingDragged(isBeingDragged);
        }

        if (!isBeingDragged) CheckForConsumingCards();
    }

    private void CheckForConsumingCards() {
        CardNode cardUnder = area2D.GetOverlappingAreas().Select(GetCardNodeFromArea2D).OrderBy(e => e.ZIndex)
            .LastOrDefault(e => e.ZIndex <= ZIndex);

        if (cardUnder is not null) {
            if (cardUnder.CardType is ICardConsumer cardConsumer) {
                if (cardConsumer.ConsumeCard(CardType)) {
                    Destroy();
                }
            }
        }
    }

    /// <summary>
    ///     Applies the texture to the sprite of the card node.
    ///     It tries to load the texture from the address of the card type.
    ///     If the texture is not found, it loads the error texture.
    /// </summary>
    private void ApplyTexture() {
        Texture2D texture;

        // Check if the path is not null or empty and if there is a file at the path
        if (string.IsNullOrEmpty(CardType.TexturePath) || !ResourceLoader.Exists(CardType.TexturePath)) {
            GD.PrintErr("Texture path is null or empty for card: " + CardType.CardNode.CardType + " " +
                        string.IsNullOrEmpty(CardType.TexturePath) + " " + FileAccess.FileExists(CardType.TexturePath));
            GD.PrintErr("Expected Texture path: " + CardType.TexturePath);
            texture = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Error.png");
            sprite.Texture = texture;
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

        sprite.Texture = texture;
    }

    /// <summary>
    ///     Sets the highlighted state of the card node.
    ///     It sets the modulate of the sprite to the highlighted color if the card is highlighted.
    /// </summary>
    public void SetHighlighted(bool isHighlighted) {
        switch (isHighlighted) {
            case true when !oldIsHighlighted:
                sprite.SetModulate(sprite.Modulate * HighLightFactor);
                oldIsHighlighted = true;
                break;
            case false when oldIsHighlighted:
                oldIsHighlighted = false;
                sprite.SetModulate(sprite.Modulate / HighLightFactor);
                break;
        }
    }

    /// <summary>
    ///     Sets the position of the card node to the position of the underCard.
    /// </summary>
    public void SetOverLappedCardToStack(CardNode underCard) {
        if (underCard == null || LastOverlappedCard == this) return;

        if (CardType is IStackable thisStackable && underCard.CardType is IStackable otherStackable)
            if (ZIndex > underCard.ZIndex) {
                thisStackable.NeighbourBelow = otherStackable;
                otherStackable.NeighbourAbove = thisStackable;

                SetPosition(underCard.Position - CardOverlappingOffset);

                if (CardType is IStackable stackable && stackable.NeighbourAbove != null) {
                    ((Card)stackable.NeighbourAbove).CardNode.SetPositionAsPartOfStack(this);
                }
            }
    }

    /// <summary>
    ///     Sets the position of the card node as part of a stack.
    ///     Does the same as SetOverLappedCardToStack but does not change the neighbours.
    /// </summary>
    /// <param name="underCard"></param>
    public void SetPositionAsPartOfStack(CardNode underCard) {
        SetPosition(underCard.Position - new Vector2(0, -15));

        if (CardType is IStackable { NeighbourAbove: not null } stackable) {
            ((Card)stackable.NeighbourAbove).CardNode.SetPositionAsPartOfStack(this);
        }
    }

    private void ClearReferences() {
        if (CardType is IStackable stackable) {
            if (HasNeighbourBelow) stackable.NeighbourBelow.NeighbourAbove = null;

            if (HasNeighbourAbove) stackable.NeighbourAbove.NeighbourBelow = null;
        }

        HoveredCards.Remove(this);
        CardController.RemoveCardFromHoveredCards(this);
    }

    /// <summary>
    ///     Processes the card node and checks if the card node is being dragged.
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta) {
        ITickable tickable = CardType as ITickable;
        tickable?.PreTick();

        if (IsBeingDragged) {
            Vector2 mousePosition = GetGlobalMousePosition();

            Position += mousePosition - oldMousePosition;

            if (CraftButton != null) {
                CraftButton.Position = Position + CardController.CraftButtonOffset;
            }

            oldMousePosition = mousePosition;
        }

        tickable?.PostTick();
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

    public void SetCraftButton(CraftButton craftButton) {
        _craftButton = craftButton;
    }

    public CraftButton GetCraftButton() {
        return _craftButton;
    }

    #region Events(?)

    public void Destroy() {
        ClearReferences();
        QueueFree();
    }

    public void _on_area_2d_mouse_entered() {
        MouseIsHovering = true;
        CardController.AddCardToHoveredCards(this);
    }

    public void _on_area_2d_mouse_exited() {
        MouseIsHovering = false;
        CardController.RemoveCardFromHoveredCards(this);
    }

    public void _on_area_2d_area_entered(Area2D area) {
        LastOverlappedCard = GetCardNodeFromArea2D(area);
        HoveredCards.Add(GetCardNodeFromArea2D(area));
    }

    public void _on_area_2d_area_exited(Area2D area) {
        LastOverlappedCard = null;
        HoveredCards.Remove(GetCardNodeFromArea2D(area));
    }

    #endregion Events(?)
}