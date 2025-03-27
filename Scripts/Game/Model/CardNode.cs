using System;
using System.Collections.Generic;
using Godot;

public partial class CardNode : Node2D {
	private const float HighLightFactor = 1.3f;

	private readonly List<CardNode> overlappingCards = new();

	private CardController cardController;

	private bool oldIsHighlighted;

	private Vector2 oldMousePosition;

	// private bool hasBeenCreated = false;

	private Sprite2D sprite;

	public CardNode() {
		AddToGroup(CardController.CARD_GROUP_NAME);
	}

	public Card CardType { get; private set; }

	public bool MouseIsHovering { get; private set; }

	public bool IsBeingDragged { get; private set; }

	public IReadOnlyCollection<CardNode> OverlappingCards => overlappingCards.AsReadOnly();

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
#if DEBUG
		GD.Print("Area entered");
#endif
		overlappingCards.Add(GetCardNodeFromArea2D(area));
	}

	public void _on_area_2d_area_exited(Area2D area) {
#if DEBUG
		GD.Print("Area exited");
#endif
		overlappingCards.Remove(GetCardNodeFromArea2D(area));
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