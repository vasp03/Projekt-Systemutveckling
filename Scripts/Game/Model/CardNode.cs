using System;
using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public partial class CardNode : Node2D {
	private const float HighLightFactor = 1.3f;

	private CardNode LastOverlappedCard;

	private CardController cardController;

	private bool oldIsHighlighted;

	private Vector2 oldMousePosition;

	private Sprite2D sprite;

	public Card CardType { get; private set; }

	public bool MouseIsHovering { get; private set; }

	public bool IsBeingDragged { get; private set; }

	public CardNode() {
		AddToGroup(CardController.CARD_GROUP_NAME);
	}

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
		oldMousePosition = GetGlobalMousePosition();
		IsBeingDragged = isBeingDragged;
	}

	public bool GetIsBeingDragged() {
		return IsBeingDragged;
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
	}

	public void _on_area_2d_area_exited(Area2D area) {
		GD.Print("Running area exited from: " + this.CardType.TextureType);

		LastOverlappedCard = null;

		// Check which card that was removed and remove it from either neighbour above or below
		if (area.GetParent() is CardNode cardNode) {
			if (cardNode.CardType is IStackable stackable) {
				if (stackable.NeighbourAbove == this) {
					stackable.SetNeighbourAbove(null);
				}
				else if (stackable.NeighbourBelow == this) {
					stackable.SetNeighbourBelow(null);
				}
			}
		}
		else {
			GD.PrintErr("Area2D parent is not a CardNode");
		}
	}

	public void SetOverLappedCardToStack(CardNode underCard) {
		GD.Print("Running SetOverLappedCardToStack from: " + this.CardType.TextureType);

		if (underCard == null || LastOverlappedCard == this) {
			return;
		}

		if (this.CardType is IStackable thisStackable && underCard.CardType is IStackable otherStackable) {
			if (this.ZIndex > underCard.ZIndex) {
				thisStackable.SetNeighbourBelow(otherStackable);
				otherStackable.SetNeighbourAbove(thisStackable);

				this.SetPosition(underCard.Position - new Vector2(0, -15));
			}
		}
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