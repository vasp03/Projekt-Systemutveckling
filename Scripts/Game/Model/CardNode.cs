using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public partial class CardNode : Node2D {
	private const float HighLightFactor = 1.3f;

	private CardController cardController;

	private CardNode LastOverlappedCard;

	private bool oldIsHighlighted;

	private Vector2 oldMousePosition;

	private Sprite2D sprite;

	public CardNode() {
		AddToGroup(CardController.CARD_GROUP_NAME);
	}

	public Card CardType { get; private set; }

	public bool MouseIsHovering { get; private set; }

	public bool IsBeingDragged { get; private set; }

	public List<CardNode> HoveredCards { get; } = new();

	public IReadOnlyList<CardNode> HoveredCardsSorted => HoveredCards.OrderBy(x => x.ZIndex).ToList();

	public bool IsMovingOtherCards { get; set; } = false;

	public bool CreateNode(Card card, Vector2 position, CardController cardController) {
		this.cardController = cardController;

		CardType = card;
		sprite = GetNode<Sprite2D>("Sprite2D");

		ApplyTexture();

		// Set the name of the card to the name of the card
		Name = card.ID;

		return true;
	}

	public void SetIsBeingDragged(bool isBeingDragged) {
		Global.AntiInfinity += 1;

		if (Global.AntiInfinity > 10000) {
			GD.PrintErr("AntiInfinity has reached above 1000: " + Global.AntiInfinity);
			return;
		}

		oldMousePosition = GetGlobalMousePosition();
		IsBeingDragged = isBeingDragged;

		if (CardType is IStackable stackable) {
			CardNode neighbourAbove = ((Card)stackable.NeighbourAbove)?.CardNode;
			if (neighbourAbove == null)
				ZIndex = cardController.CardCount;
			else
				neighbourAbove.SetIsBeingDragged(isBeingDragged);
		}
	}

	public bool HasNeighbourAbove() {
		if (CardType is IStackable stackable) return stackable.NeighbourAbove != null;
		return false;
	}

	public bool HasNeighbourBelow() {
		if (CardType is IStackable stackable) return stackable.NeighbourBelow != null;
		return false;
	}

	public bool CreateNode(Card card, CardController cardController) {
		return CreateNode(card, new Vector2(100, 100), cardController);
	}

	private void ApplyTexture() {
		Texture2D texture;
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

	public void SetHighlighted(bool isHighlighted) {
		if (isHighlighted && !oldIsHighlighted) {
			sprite.SetModulate(sprite.Modulate * HighLightFactor);
			oldIsHighlighted = true;
		}
		else if (!isHighlighted && oldIsHighlighted) {
			oldIsHighlighted = false;
			sprite.SetModulate(sprite.Modulate / HighLightFactor);
		}
	}

	public void _on_area_2d_mouse_entered() {
		MouseIsHovering = true;
		cardController.AddCardToHoveredCards(this);
	}

	public void _on_area_2d_mouse_exited() {
		MouseIsHovering = false;
		cardController.RemoveCardFromHoveredCards(this);
	}

	public void _on_area_2d_area_entered(Area2D area) {
		LastOverlappedCard = GetCardNodeFromArea2D(area);
		HoveredCards.Add(GetCardNodeFromArea2D(area));
	}

	public void _on_area_2d_area_exited(Area2D area) {
		LastOverlappedCard = null;
		HoveredCards.Remove(GetCardNodeFromArea2D(area));

		// Check which card that was removed and remove it from either neighbour above or below
		if (area.GetParent() is CardNode cardNode) {
		}
	}

	public void SetOverLappedCardToStack(CardNode underCard) {
		if (underCard == null || LastOverlappedCard == this) return;

		if (CardType is IStackable thisStackable && underCard.CardType is IStackable otherStackable)
			if (ZIndex > underCard.ZIndex) {
				thisStackable.SetNeighbourBelow(otherStackable);
				otherStackable.SetNeighbourAbove(thisStackable);

				SetPosition(underCard.Position - new Vector2(0, -15));

				if (CardType is IStackable stackable && stackable.NeighbourAbove != null)
					((Card)stackable.NeighbourAbove).CardNode.SetPositionAsPartOfStack(this);
			}
	}

	public void SetPositionAsPartOfStack(CardNode underCard) {
		SetPosition(underCard.Position - new Vector2(0, -15));

		if (CardType is IStackable stackable && stackable.NeighbourAbove != null)
			((Card)stackable.NeighbourAbove).CardNode.SetPositionAsPartOfStack(this);
	}

	public override void _Process(double delta) {
		if (IsBeingDragged) {
			Vector2 mousePosition = GetGlobalMousePosition();

			Position += mousePosition - oldMousePosition;

			oldMousePosition = mousePosition;
		}
	}

	public static CardNode GetCardNodeFromArea2D(Area2D area2D) {
		return (CardNode)area2D.GetParent();
	}
}