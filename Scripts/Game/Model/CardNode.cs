using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public partial class CardNode : Node2D {
	private const float HIGHLIGT_FACTOR = 1.3f;

	private CardController cardController;

	private CardNode LastOverlappedCard;

	private Vector2 oldMousePosition;

	private Sprite2D sprite => GetNode<Sprite2D>("Sprite2D");
	private Area2D area2D => GetNode<Area2D>("Area2D");

	private Card _cardType;
	public Card CardType {
		get => _cardType;
		set {
			_cardType = value;
			ApplyTexture();
		}
	}
	
	public CardNode() {
		AddToGroup(CardController.CARD_GROUP_NAME);


		
		// this.area2D.AreaEntered += this._on_area_2d_area_entered;
		// this.area2D.AreaExited += this._on_area_2d_area_exited;
	}

	bool CreateNode(Card card, Vector2 position, CardController cardController) {
		this.cardController = cardController;
		
		CardType = card;

		// Set the name of the card to the name of the card
		Name = card.ID;
		
		this.area2D.MouseExited += this.OnMouseExited;
		this.area2D.MouseEntered += this.OnMouseEntered;

		return true;
	}
	public bool CreateNode(Card card, CardController cardController) {
		return CreateNode(card, new Vector2(100, 100), cardController);
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
		this.area2D.GetOverlappingAreas().Select(GetCardNodeFromArea2D).OrderBy(e=>e.ZIndex).ToArray();
	public bool MouseIntersecting { get; private set; }
	public bool MouseHovered { get; private set; }

	private bool isDragging;
	
	public bool IsBeingDragged {
		get => isDragging;
		set {
			switch (value) {
				case true:
					this.startDragging();
					break;
				case false:
					this.stopDragging();
					break;
			}

			this.isDragging = value;
		}
	}

	private void startDragging() {
		this.ZIndex = this.cardController.AllCardsSorted.LastOrDefault().ZIndex+1;
		
		this.UpdateZIndexOnDragging();
		
		// this.SetAsTopLevel(true);
		this.oldMousePosition = GetGlobalMousePosition();
	}
	
	private void stopDragging() {
		// this.SetAsTopLevel(false);
		this.oldMousePosition = Vector2.Zero;

		this.CheckForStacking();
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

	public override void _Process(double delta) {
		SetHighlighted(this.cardController.TopHoveredCard == this && this.MouseIntersecting); 
		if (IsBeingDragged) {
			Vector2 mousePosition = GetGlobalMousePosition();

			Position += mousePosition - oldMousePosition;

			oldMousePosition = mousePosition;
		}
		else if (this.HasCardBelow) {
			this.Position = this.CardBelow.Position + Vector2.Down * 15f;
		}
	}


	public static CardNode GetCardNodeFromArea2D(Area2D area2D) {
		return area2D.GetParent<CardNode>();
	}
	
	

	#region Stacking logic

	private void CheckForStacking() {
		if (this.CardType is IStackable stackable) {
			CardNode other = this.OverlappingCards.LastOrDefault(e => !e.HasCardAbove);

			if (other is not null && stackable.CanStackWith(other?.CardType) && !this.Stack.Contains(other)) {
				this.CardBelow = other;
			}
			else {
				this.CardBelow = null;
			}

			this.UpdateZIndex();
		}
	}

	private void UpdateZIndexOnDragging() {
		foreach (CardNode cardNode in this.StackAbove)
		{
			cardNode.UpdateZIndex();
		}
	}
	private void UpdateZIndex() {
		this.ZIndex = this.HasCardBelow ? ZIndexInStack : this.ZIndex - 1;
	}

	/// <summary>
	///     Traverses the entire stack (Both Forward and Backwards) from the current instance.
	///     Gets the current entire stack collection
	/// </summary>
	public IReadOnlyCollection<CardNode> Stack => this.StackBelow.Union(this.StackAbove).ToArray();

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
			
			return stackBackwards.Reverse().ToArray();
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
			
			return stackForwards.ToArray();
		}
	}

	public int ZIndexInStack => this.HasCardBelow ? CardBelow.ZIndexInStack + 1 : this.ZIndex;
	public int PositionInStack => this.StackBelow.Count == 0 ? -1 : this.StackBelow.Count;

	private CardNode? _cardAbove;
	private CardNode? _cardBelow;
	public CardNode? CardAbove {
		get => _cardAbove;
		set {
			if (this.HasCardAbove && value is null) {
				// remove the reference to <this> card on the card above
				this.CardAbove.CardBelow = null;
			}
			this._cardAbove = value;
			if (_cardAbove is not null) {
				this._cardAbove._cardBelow = this;
			}
		}
	}

	public CardNode? CardBelow {
		get => _cardBelow;
		set {
			if (this.HasCardBelow && value is null) {
				this.CardBelow.CardAbove = null;
			}

			this._cardBelow = value;
			if (_cardBelow is not null) {
				this._cardBelow.CardBelow = this;
			}
		}
	}

	public bool HasCardAbove => CardAbove is not null;
	public bool HasCardBelow => CardBelow is not null;

	#endregion
}