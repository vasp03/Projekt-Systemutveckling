using System;
using Godot;
using Godot.Collections;

public partial class Cards : Node2D
{
    Boolean isDragging = false;
    Vector2 oldMousePosition;

    public Sprite2D GetSprite2D()
    {
        return (Sprite2D)GetNode("Sprite2D");
    }

    public override void _Ready()
    {
        // Check if the node is not called CardTemplate
        if (Name != "CardTemplate")
        {
            // Set random texture
            SetRandomTexture();

            // Make the node visible
            Visible = true;
        }
    }

    // Set random texture
    public void SetRandomTexture()
    {
        SetTexture(CardTypeEnum.TypeEnum.Random);
    }

    private void SetTexture(CardTypeEnum.TypeEnum type)
    {
        Texture texture;
        Sprite2D sprite = (Sprite2D)GetNode("Sprite2D");

        texture = CardTypeEnum.GetTexture(type);

        if (sprite != null)
        {
            sprite.SetTexture((Texture2D)texture);
            Visible = true;
        }
        else
        {
            GD.Print("Sprite is null");
            Visible = true;
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

    public void setDragging(Boolean value)
    {
        isDragging = value;
        if (value)
        {
            oldMousePosition = GetGlobalMousePosition();
        }
    }
}
