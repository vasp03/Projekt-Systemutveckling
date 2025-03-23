using System;
using Godot;
using Godot.Collections;

public partial class Cards : Node2D
{
    private Boolean isDragging = false;
    private Vector2 oldMousePosition;

    private CardTypeInfomationHolder cardTypeInformationHolder;

    private int cardId;

    public Sprite2D GetSprite2D()
    {
        return (Sprite2D)GetNode("Sprite2D");
    }

    public void StartCard(int cardId, CardTypeEnum.TypeEnum type = CardTypeEnum.TypeEnum.Random, Vector2 position = new Vector2())
    {
        this.cardId = cardId;

        if (position == new Vector2())
        {
            position = new Vector2(100, 100);
        }

        SetTexture(type);

        // Add card to group "Card"
        AddToGroup("Card");

        // Set the card position
        Position = new Vector2(100, 100);
    }

    private void SetTexture(CardTypeEnum.TypeEnum type)
    {
        cardTypeInformationHolder = CardTypeEnum.GetCardTypeInfomationHolder(type);

        Sprite2D sprite = (Sprite2D)GetNode("Sprite2D");

        if (sprite != null)
        {
            sprite.SetTexture((Texture2D)GD.Load<Texture>(cardTypeInformationHolder.GetTexture()));
            Visible = true;
        }
        else
        {
            GD.Print("Sprite is null for card: " + type);
        }
    }

    public override void _Process(double delta)
    {
        if (isDragging)
        {
            // Get the global mouse position
            Vector2 mousePosition = GetGlobalMousePosition();

            // Set the card position to the mouse position
            Position += mousePosition - oldMousePosition;

            // Update the old mouse position
            oldMousePosition = mousePosition;
        }
    }

    public void SetDragging(Boolean value)
    {
        isDragging = value;
        if (value)
        {
            oldMousePosition = GetGlobalMousePosition();
        }
    }

    public CardTypeInfomationHolder GetCardTypeInformationHolder()
    {
        return cardTypeInformationHolder;
    }

    public int GetCardId()
    {
        return cardId;
    }
}
