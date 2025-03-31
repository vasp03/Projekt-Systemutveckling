using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public partial class CardNode : Node2D {
	private const float HIGHLIGT_FACTOR = 1.3f;

	private Card _cardType;

	private CardController cardController;

	private CardNode LastOverlappedCard;

	private Vector2 oldMousePosition;

	public CardNode() {
		AddToGroup(CardController.CARD_GROUP_NAME);


		// this.area2D.AreaEntered += this._on_area_2d_area_entered;
		// this.area2D.AreaExited += this._on_area_2d_area_exited;
	}

	private Sprite2D sprite => GetNode<Sprite2D>("Sprite2D");
	private Area2D area2D => GetNode<Area2D>("Area2D");

	public Card CardType {
		get => _cardType;
		set {
			_cardType = value;
			ApplyTexture();
		}
	}

	private bool CreateNode(Card card, Vector2 position, CardController cardController) {
		this.cardController = cardController;

		CardType = card;

		// Set the name of the card to the name of the card
		Name = card.ID;

		area2D.MouseExited += OnMouseExited;
		area2D.MouseEntered += OnMouseEntered;

		return true;
	}

	public bool CreateNode(Card card, CardController cardController) {
		return CreateNode(card, new Vector2(100, 100), cardController);
	}

	public override void _Process(double delta) {
		SetHighlighted(cardController.TopHoveredCard == this && MouseIntersecting);
		if (IsBeingDragged) {
			Vector2 mousePosition = GetGlobalMousePosition();

			Position += mousePosition - oldMousePosition;

			oldMousePosition = mousePosition;
		}
		else if (HasCardBelow) {
			Position = CardBelow.Position + Vector2.Down * 15f;
		}
	}


	public static CardNode GetCardNodeFromArea2D(Area2D area2D) {
		return area2D.GetParent<CardNode>();
	}

	#region Visual logic

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

	private bool oldIsHighlighted;

	public void SetHighlighted(bool isHighlighted) {
		if (isHighlighted && !oldIsHighlighted) {
			sprite.SetModulate(sprite.Modulate * HIGHLIGT_FACTOR);
			oldIsHighlighted = true;
		}
		else if (!isHighlighted && oldIsHighlighted) {
			oldIsHighlighted = false;
			sprite.SetModulate(sprite.Modulate / HIGHLIGT_FACTOR);
		}
	}

	#endregion Visual logic

	#region Collision & overlapping, drag logic

	public IReadOnlyCollection<CardNode> OverlappingCards =>
		area2D.GetOverlappingAreas().Select(GetCardNodeFromArea2D).OrderBy(e => e.ZIndex).ToArray();

	public bool MouseIntersecting { get; private set; }
	public bool MouseHovered { get; private set; }

	private bool isDragging;

	public bool IsBeingDragged {
		get => isDragging;
		set {
			switch (value) {
				case true:
					startDragging();
					break;
				case false:
					stopDragging();
					break;
			}

			isDragging = value;
		}
	}

	private void startDragging() {
		ZIndex = cardController.AllCardsSorted.LastOrDefault().ZIndex + 1;

		UpdateZIndexOnDragging();

		// this.SetAsTopLevel(true);
		oldMousePosition = GetGlobalMousePosition();
	}

	private void stopDragging() {
		// this.SetAsTopLevel(false);
		oldMousePosition = Vector2.Zero;

		CheckForStacking();
	}


	private void OnMouseEntered() {
		MouseIntersecting = true;
	}

	private void OnMouseExited() {
		MouseIntersecting = false;
	}

	// private void OnCardHoverUpdate() {
	// 	this.MouseHovered = true;
	// }
	// 
	// private void OnCardUnhoveredUpdate() {
	// 	this.MouseHovered = false;
	// }

	#endregion


	#region Stacking logic

	private void CheckForStacking() {
		if (CardType is IStackable stackable) {
			CardNode other = OverlappingCards.LastOrDefault(e => !e.HasCardAbove && !e.Stack.Contains(this));

			if (other is not null && stackable.CanStackWith(other?.CardType) && !this.Stack.Contains(other))
				CardBelow = other;
			else
				CardBelow = null;

			UpdateZIndex();
		}
	}

	private void UpdateZIndexOnDragging() {
		foreach (CardNode cardNode in StackAbove) cardNode.UpdateZIndex();
	}

	private void UpdateZIndex() {
		ZIndex = HasCardBelow ? ZIndexInStack : ZIndex - 1;
	}

	/// <summary>
	///     Traverses the entire stack (Both Forward and Backwards) from the current instance.
	///     Gets the current entire stack collection
	/// </summary>
	public IReadOnlyCollection<CardNode> Stack => StackBelow.Append(this).Union(this.StackAbove).ToImmutableArray();

	public IReadOnlyCollection<CardNode> StackBelow {
		get {
			ICollection<CardNode> stackBackwards = [];


			CardNode current = this;
			CardNode next = null;

			// Traverse backwards
			while (current != null && current.HasCardBelow) {
				next = current.CardBelow;
				stackBackwards.Add(next);
				current = next;
			}

			return stackBackwards.Reverse().ToImmutableArray();
		}
	}

	public IReadOnlyCollection<CardNode> StackAbove {
		get {
			ICollection<CardNode> stackForwards = [];


			CardNode current = this;
			CardNode next = null;

			// Traverse forwards
			while (current != null && current.HasCardAbove) {
				next = current.CardAbove;
				stackForwards.Add(next);
				current = next;
			}

			return stackForwards.ToImmutableArray();
		}
	}

	public int ZIndexInStack => HasCardBelow ? CardBelow.ZIndexInStack + 1 : ZIndex;
	public int PositionInStack => StackBelow.Count == 0 ? -1 : StackBelow.Count;

	private CardNode? _cardAbove;
	private CardNode? _cardBelow;

	public CardNode? CardAbove {
		get => _cardAbove;
		set {
			if (HasCardAbove && value is null)
				// remove the reference to <this> card on the card above
				CardAbove._cardBelow = null;
			_cardAbove = value;
			if (_cardAbove is not null) _cardAbove._cardBelow = this;
		}
	}

	public CardNode? CardBelow {
		get => _cardBelow;
		set {
			if (HasCardBelow && value is null) CardBelow._cardAbove = null;

			_cardBelow = value;
			if (_cardBelow is not null) _cardBelow._cardAbove = this;
		}
	}

	public bool HasCardAbove => CardAbove is not null;
	public bool HasCardBelow => CardBelow is not null;

	#endregion
}