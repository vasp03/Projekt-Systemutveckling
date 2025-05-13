using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;
using Vector2 = Godot.Vector2;

/// <summary>
///     Represents a card node in the game.
///     It inherits from Node2D and is used to represent a card in the game.
/// </summary>
public partial class CardNode : Node2D {
    private const float HighLightFactor = 1.3f;
    private Card _cardType;
    private CardNode lastOverlappedCard { get; set; }
    private CardNode OverlappingCard => area2D.GetOverlappingAreas().Select(GetCardNodeFromArea2D).Where(e=>e.ZIndex<ZIndex).OrderByDescending(e=>e.ZIndex).FirstOrDefault();
    private bool oldIsHighlighted;
    private Vector2 oldMousePosition;

    public CardNode() {
        AddToGroup(CardController.CARD_GROUP_NAME);
    }

    public CardController CardController { get; set; }
    private Sprite2D sprite => GetNode<Sprite2D>("Sprite2D");
    private Area2D area2D => GetNode<Area2D>("Area2D");
    public Vector2 CardOverlappingOffset { get; private set; } = new(0, -20);
    public bool MouseIsHovering { get; private set; }
    private bool dragged;
    public bool Dragged { 
        get => dragged;
        set {
            dragged = value;
            OnDragChanged(value);
        }
    }
    public List<CardNode> HoveredCards { get; } = [];
    public IReadOnlyList<CardNode> HoveredCardsSorted => HoveredCards.OrderBy(x => x.ZIndex).ToList();
    public bool IsMovingOtherCards { get; set; } = false;
    public CraftButton CraftButton { get; set; }
    public Vector2 CardSize => sprite?.Texture?.GetSize() ?? new Vector2(80, 128);
    private bool MovedOneLastTime { get; set; }

    public Card CardType {
        get => _cardType;
        set {
            if (_cardType is not null) _cardType.CardNode = null;

            value.CardNode = this;
            _cardType = value;
            ApplyTexture();
        }
    }
    
    #region Stack-related properties

    private CardNode cardAbove;
    private CardNode cardBelow;

    public CardNode NeighbourAbove {
        get => IsInstanceValid(cardAbove) ? cardAbove : null;
        set {
            // Setting it to null means clearing the reference as well
            if (value is null) {
                if (HasNeighbourAbove) {
                    cardAbove.cardBelow = null;
                }
                cardAbove = null;
                return;
            }
            cardAbove = value;
            cardAbove.cardBelow = this;
        }
    }

    public CardNode NeighbourBelow {
        get => IsInstanceValid(cardBelow) ? cardBelow : null;
        set {
            // Setting it to null means clearing the reference as well
            if (value is null) {
                if (HasNeighbourBelow) {
                    cardBelow.cardAbove = null;
                }
                cardBelow = null;
                return;
            }
            cardBelow = value;
            cardBelow.cardAbove = this;
        }
    }
    
    /// <summary>
    ///     Traverses the entire stack (Both Forward and Backwards) from the current instance.
    ///     Gets the current entire stack collection
    /// </summary>
    public IReadOnlyCollection<CardNode> Stack => new List<CardNode>(this.StackBelow).Append(this).Union(StackAbove).ToArray();

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
    
    public IReadOnlyList<CardNode> StackAbove {
        get {
            List<CardNode> stackForwards = [];

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

    public IReadOnlyList<CardNode> StackAboveWithItself => new List<CardNode>([this]).Union(StackAbove).ToArray();

    public IReadOnlyList<CardNode> StackBelowWithItself => new List<CardNode>(StackBelow).Append(this).ToArray();
    
    public CardNode BottomCardOfStack => this.StackBelowWithItself.FirstOrDefault();
    public CardNode TopCardOfStack => this.StackAboveWithItself.LastOrDefault();
    
    #endregion Stack-related properties
    /// <summary>
    ///     Sets the position of the card node to the given position.
    /// </summary>
    public bool HasNeighbourAbove => this.NeighbourAbove is not null;

    /// <summary>
    ///     Checks if the card has a neighbour below.
    /// </summary>
    public bool HasNeighbourBelow => this.NeighbourBelow is not null;

    /// <summary>
    ///     Sets the position of the card node to the given position.
    /// </summary>
    private void OnDragChanged(bool newDragValue) {
        if (!IsInstanceValid(this) || IsQueuedForDeletion()) return;

        oldMousePosition = GetGlobalMousePosition();
        
        // Executed once a card is dropped (no longer being dragged)
        if (!newDragValue) ExecuteCardConsumptionLogic();

        // if (!HasNeighbourAbove)
        //     ZIndex = CardController.CardCount;
        // else
        //     NeighbourAbove.Dragged = newDragValue;
        
        ExecuteStackingLogic();


        if (newDragValue && CardType is CardLiving) CardController.HideHealthAndHunger();
    }

    private bool ExecuteCardConsumptionLogic() {
        CardNode cardUnder = area2D.GetOverlappingAreas().Select(GetCardNodeFromArea2D).OrderBy(e => e.ZIndex)
            .LastOrDefault(e => e.ZIndex <= ZIndex);

        if (cardUnder is not null)
            if (cardUnder.CardType is ICardConsumer cardConsumer)
                if (cardConsumer.ConsumeCard(CardType)) {
                    Destroy();
                    return true;
                }

        return false;
    }

    private void ExecuteStackingLogic() {
        // if (!HasNeighbourAbove) {
// 
        // } else {
        //     NeighbourAbove.Dragged = Dragged;
        // }

        if (Dragged) {
            NeighbourBelow = null;
            if (!HasNeighbourAbove) {
                ZIndex = CardController.AllCards.Max(e => e.ZIndex) + 1;
            }
        } else {
            if (OverlappingCard is null) {
                this.ZIndex = 1;
            }
        }

        if (OverlappingCard is not null && !Dragged && !OverlappingCard.HasNeighbourAbove) {
            NeighbourBelow = OverlappingCard;
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

    // /// <summary>
    // ///     Sets the position of the card node to the position of the underCard.
    // /// </summary>
    // public void SetOverLappedCardToStack(CardNode underCard) {
    //     if (underCard is null || underCard == this || !IsInstanceValid(underCard)) { return; }
// 
    //     // if (CardType is IStackable thisStackable && underCard.CardType is IStackable otherStackable)
    //         if (ZIndex > underCard.ZIndex) {
    //             NeighbourBelow = underCard;
    //             // thisStackable.NeighbourBelow = otherStackable;
    //             // otherStackable.NeighbourAbove = thisStackable;
// 
    //             SetPosition(underCard.Position - CardOverlappingOffset);
// 
    //             if (HasNeighbourAbove) {
    //                 NeighbourAbove.SetPositionAsPartOfStack(this);
    //             }
    //         }
    // }

    /// <summary>
    ///     Sets the position of the card node as part of a stack.
    ///     Does the same as SetOverLappedCardToStack but does not change the neighbours.
    /// </summary>
    /// <param name="underCard"></param>
    public void SetPositionAsPartOfStack(CardNode underCard) {
        if (underCard is null || !IsInstanceValid(underCard)) return;

        SetPosition(underCard.Position - new Vector2(0, -20));

        if (HasNeighbourAbove) {
            NeighbourAbove.SetPositionAsPartOfStack(this);
        }

        // if (CardType is IStackable { NeighbourAbove: not null } stackable) {
        //     CardNode aboveCard = ((Card)stackable.NeighbourAbove).CardNode;
// 
        //     if (aboveCard is not null && IsInstanceValid(aboveCard))
        //         
        // }
    }

    private void ClearReferences() {
        NeighbourAbove = null;
        NeighbourBelow = null;

        HoveredCards.Remove(this);
        CardController.RemoveCardFromHoveredCards(this);
    }

    /// <summary>
    ///     Processes the card node and checks if the card node is being dragged.
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta) {
        if (Dragged) {
            // if (HasNeighbourBelow) return;

            MovedOneLastTime = false;

            Vector2 mousePosition = GetGlobalMousePosition();
            // IStackable stackable = CardType is IStackable stack ? stack : null;
            CardNode bottomCard = this.BottomCardOfStack;
            int neighboursAbove = this.StackAbove.Count;
            Vector2 mousePositionDelta = mousePosition - oldMousePosition;

            if (bottomCard.Position.Y <= 64 && mousePositionDelta.Y < 0) mousePositionDelta.Y = 0;

            // int counter = 1;
            // CardNode currentCard = this;
            // while (currentCard is not null) {
            //     currentCard = currentCard.NeighbourAbove;//cardAbove.CardType is IStackable stackableAbove
            //         // ? (stackableAbove.NeighbourAbove as Card)?.CardNode
            //         // : null;
            //     if (currentCard is not null && currentCard != this)
            //         currentCard.Position = bottomCard.Position - new Vector2(0, counter++ * 20 * -1);
            // }
            ClampPositionInGameSpace(mousePositionDelta);


            if (CraftButton is not null) CraftButton.Position = Position + CardController.CRAFT_BUTTON_OFFSET;

            oldMousePosition = mousePosition;
        } else if (!MovedOneLastTime) {
            CardNode cardAboveThis = NeighbourAbove;
                //CardType is IStackable stackable ? (stackable.NeighbourAbove as Card)?.CardNode : null;
            if (cardAboveThis is not null) Position = cardAboveThis.Position + CardOverlappingOffset;

            MovedOneLastTime = true;
        } else if (HasNeighbourBelow && !Dragged) {
            this.Position = NeighbourBelow.Position - CardOverlappingOffset;
        }
    }

    public override void _PhysicsProcess(double delta) {
        ITickable tickableCardType = CardType as ITickable;
        tickableCardType?.PreTick();
        tickableCardType?.PostTick();
    }

    void ClampPositionInGameSpace(Vector2 mousePositionDelta) {
        Position = new Vector2(
            Math.Clamp(Position.X + mousePositionDelta.X, 0 + CardSize.X / 2, 1280 - CardSize.X / 2),
            Math.Clamp(Position.Y + mousePositionDelta.Y, 0 + CardSize.Y / 2,
                720 - (CardSize.Y + StackAbove.Count * 40) / 2)
        );
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

    // public void _on_area_2d_area_entered(Area2D area) {
    //     CardNode lastOverlappedCardTemp = GetCardNodeFromArea2D(area);
    //     if (lastOverlappedCardTemp.ZIndex > this.ZIndex)
    //         return;
    //     else
    //         lastOverlappedCard = lastOverlappedCardTemp;
    //     // HoveredCards.Add(GetCardNodeFromArea2D(area));
    // }

    // public void _on_area_2d_area_exited(Area2D area) {
    //     lastOverlappedCard = null;
    //     // HoveredCards.Remove(GetCardNodeFromArea2D(area));
    // }

    #endregion Events(?)
}