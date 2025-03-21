using System;
using Godot;

public partial class CardSummon : Sprite2D
{
    Boolean isDragging = false;

    public void CreateNew()
    {
        GD.Print("CardSummon.createNew() called");
    }

    public override void _Process(double delta)
    {
        if (isDragging)
        {
            // Get the global mouse position
            Vector2 mousePosition = GetGlobalMousePosition();

            // Set the card position to the mouse position
            Position = mousePosition;            
        }
    }

    // When mouse is pressed on the card; move it with the cursor
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && isMouseOver())
            {
                isDragging = true;
            }
            else
            {
                isDragging = false;
            }
        }
    }

    private Boolean isMouseOver()
    {
        // Get the global mouse position
        Vector2 mousePosition = GetGlobalMousePosition();

        // Get the cards area
        Rect2 cardArea = new Rect2(Position, new Vector2(GetTexture().GetWidth(), GetTexture().GetHeight()));
         
        // Check if the mouse is inside the card area
        return cardArea.HasPoint(mousePosition);
    }
}
