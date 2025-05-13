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
	public bool MouseIsHovering { get; private set; }
	public bool IsBeingDragged { get; private set; }
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

	/// <summary>
	///     Sets the position of the card node to the given position.
	/// </summary>
	public bool HasNeighbourAbove {
		get {
			if (CardType is IStackable stackable) return stackable.NeighbourAbove is not null;
			return false;
		}
	}

	/// <summary>
	///     Checks if the card has a neighbour below.
	/// </summary>
	public bool HasNeighbourBelow {
		get {
			if (CardType is IStackable stackable) return stackable.NeighbourBelow is not null;
			return false;
		}
	}

	/// <summary>
	///     Sets the position of the card node to the given position.
	/// </summary>
	public void SetIsBeingDragged(bool isBeingDragged) {
		if (!IsInstanceValid(this) || IsQueuedForDeletion()) return;

		oldMousePosition = GetGlobalMousePosition();
		IsBeingDragged = isBeingDragged;


		if (!isBeingDragged) CheckForConsumingCards();

		if (CardType is not IStackable stackable) return;

		CardNode neighbourAbove = ((Card)stackable.NeighbourAbove)?.CardNode;
		if (neighbourAbove is null)
			ZIndex = CardController.CardCount;
		else
			neighbourAbove.SetIsBeingDragged(isBeingDragged);


		if (isBeingDragged && CardType is CardLiving cardLiving) CardController.HideHealthAndHunger();
	}

	private bool CheckForConsumingCards() {
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
		if (underCard is null || underCard == this || !IsInstanceValid(underCard)) return;

		if (CardType is IStackable thisStackable && underCard.CardType is IStackable otherStackable)
			if (ZIndex > underCard.ZIndex) {
				thisStackable.NeighbourBelow = otherStackable;
				otherStackable.NeighbourAbove = thisStackable;

				SetPosition(underCard.Position - CardOverlappingOffset);

				if (thisStackable.NeighbourAbove is Card above &&
					IsInstanceValid(above.CardNode))
					above.CardNode.SetPositionAsPartOfStack(this);
			}
	}

	/// <summary>
	///     Sets the position of the card node as part of a stack.
	///     Does the same as SetOverLappedCardToStack but does not change the neighbours.
	/// </summary>
	/// <param name="underCard"></param>
	public void SetPositionAsPartOfStack(CardNode underCard) {
		if (underCard is null || !IsInstanceValid(underCard)) return;

		SetPosition(underCard.Position - new Vector2(0, -20));

		if (CardType is IStackable { NeighbourAbove: not null } stackable) {
			CardNode aboveCard = ((Card)stackable.NeighbourAbove).CardNode;

			if (aboveCard is not null && IsInstanceValid(aboveCard))
				aboveCard.SetPositionAsPartOfStack(this);
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
			if (HasNeighbourBelow) return;

			MovedOneLastTime = false;

			Vector2 mousePosition = GetGlobalMousePosition();
			IStackable stackable = CardType is IStackable stack
				? stack
				: null;
			CardNode bottomCard = (stackable?.CardAtBottom as Card)?.CardNode ?? this;
			int neighboursAbove = stackable?.StackAbove.Count ?? 0;
			Vector2 newPosition = mousePosition - oldMousePosition;

			if (bottomCard.Position.Y <= 64 && newPosition.Y < 0) newPosition.Y = 0;

			int counter = 1;
			CardNode cardAbove = this;
			while (cardAbove is not null) {
				cardAbove = cardAbove.CardType is IStackable stackableAbove
					? (stackableAbove.NeighbourAbove as Card)?.CardNode
					: null;
				if (cardAbove is not null && cardAbove != this)
					cardAbove.Position = bottomCard.Position - new Vector2(0, counter++ * 20 * -1);
			}

			Position = new Vector2(
				Math.Clamp(Position.X + newPosition.X, 0 + CardSize.X / 2, 1280 - CardSize.X / 2),
				Math.Clamp(Position.Y + newPosition.Y, 0 + CardSize.Y / 2,
					720 - (CardSize.Y + neighboursAbove * 40) / 2)
			);

			if (CraftButton is not null) CraftButton.Position = Position + CardController.CRAFT_BUTTON_OFFSET;

			oldMousePosition = mousePosition;
		} else if (!MovedOneLastTime) {
			CardNode cardAboveThis =
				CardType is IStackable stackable
					? (stackable.NeighbourAbove as Card)?.CardNode
					: null;
			if (cardAboveThis is not null) Position = cardAboveThis.Position + CardOverlappingOffset;

			MovedOneLastTime = true;
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

	#region Events(?)

	public void Destroy() {
		ClearReferences();
        
        if (CraftButton is not null) {
            CraftButton.QueueFree();
            CraftButton = null;
        }
        
		QueueFree();
	}
	
	public void UnlinkFromStack() {
		if (CardType is not IStackable thisStackable)
			return;

		IStackable above = thisStackable.NeighbourAbove;
		IStackable below = thisStackable.NeighbourBelow;

		if (below is not null)
			below.NeighbourAbove = above;

		if (above is not null)
			above.NeighbourBelow = below;

		thisStackable.NeighbourAbove = null;
		thisStackable.NeighbourBelow = null;
	}
	
	public void Sell() {
		if (CardType is null || CardType.Value < 0)
			return;

		Global.Singleton.AddMoney(CardType.Value);
		GameController.Singleton.HUD.ShowFloatingMoneyLabel(CardType.Value);
		UnlinkFromStack();
		Destroy();
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
