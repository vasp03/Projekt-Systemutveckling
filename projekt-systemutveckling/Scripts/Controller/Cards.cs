using System;
using Godot;

public partial class Cards : Sprite2D
{
    Boolean isDragging = false;
    Vector2 oldMousePosition;

    public void CreateNew()
    {
        // Create a new instance of the card
        Sprite2D copyNode = new Sprite2D();
        copyNode = (Sprite2D)this.Duplicate();
        copyNode.Name = "CardSummon";
        copyNode.Position = new Vector2(100, 100);
        SetRandomTexture(copyNode);
        GetParent().AddChild(copyNode);
    }

    public void Destroy()
    {
        // Remove the card from the scene
        QueueFree();
    }

    // Set random texture
    public void SetRandomTexture(Sprite2D sprite = null)
    {
        Random random = new Random();
        int randomInt = random.Next(0, 10);

        CardTypeEnum.TypeEnum type = (CardTypeEnum.TypeEnum)randomInt;
        SetTexture(type, sprite);
    }

    public void SetTexture(CardTypeEnum.TypeEnum type, Sprite2D sprite = null)
    {
        Texture texture;

        texture = CardTypeEnum.GetTexture(type);

        if (sprite == null)
        {
            SetTexture((Texture2D)texture);
        }
        else
        {
            sprite.SetTexture((Texture2D)texture);
        }

        // Set the visible property to true
        Visible = true;
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

    // When mouse is pressed on the card; move it with the cursor
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && isMouseOver() && Global.GetCardSelected() == false)
            {
                Global.SetCardSelected(true);
                isDragging = true;
                oldMousePosition = GetGlobalMousePosition();

                // Set y position so the card is rendered above other cards
                ZIndex = 1;
            }
            else if(mouseButton.Pressed && !isMouseOver())
            {
                Global.SetCardSelected(false);
                isDragging = false;
                ZIndex = 0;
            }
            else
            {
                Global.SetCardSelected(false);
                isDragging = false;
            }
        }
    }

    private Boolean isMouseOver()
    {
        // Get the global mouse position
        Vector2 mousePosition = GetGlobalMousePosition();

        var x = GetTexture().GetWidth();
        var y = GetTexture().GetHeight();

        // Get the cards area
        Rect2 cardArea = new Rect2(Position - (new Vector2(x / 2, y / 2)), new Vector2(x, y));

        // Check if the mouse is inside the card area
        return cardArea.HasPoint(mousePosition);
    }
}
