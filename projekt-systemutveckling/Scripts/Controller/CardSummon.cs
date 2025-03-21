using System;
using Godot;

public partial class CardSummon : Sprite2D
{
    Boolean isDragging = false;
    Vector2 oldMousePosition;

    public void CreateNew()
    {
        GD.Print("CardSummon.createNew() called");

        // Create a new instance of the card
        Sprite2D copyNode = new Sprite2D();
        copyNode = this.Duplicate() as Sprite2D;
        copyNode.Name = "CardSummon";
        copyNode.Position = new Vector2(100, 100);
        SetRandomTexture(copyNode);
        GetParent().AddChild(copyNode);
    }

    public void Destroy()
    {
        GD.Print("CardSummon.destroy() called");

        // Remove the card from the scene
        QueueFree();
    }

    // Set random texture
    public void SetRandomTexture(Sprite2D sprite = null)
    {
        Random random = new Random();
        int randomInt = random.Next(0, 10);

        CardTypeEnum type = (CardTypeEnum)randomInt;
        GD.Print("Random card type: " + type);
        SetTexture(type, sprite);
    }

    public void SetTexture(CardTypeEnum type, Sprite2D sprite = null)
    {
        Texture texture;

        switch (type)
        {
            case CardTypeEnum.wood:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Wood.png");
                break;
            case CardTypeEnum.rock:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Rock.png");
                break;
            case CardTypeEnum.water:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Water.png");
                break;
            case CardTypeEnum.stick:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Stick.png");
                break;
            case CardTypeEnum.planks:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Planks.png");
                break;
            case CardTypeEnum.leaves:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Leaves.png");
                break;
            case CardTypeEnum.sword:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Sword Mk1.png");
                break;
            case CardTypeEnum.apple:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Apple.png");
                break;
            case CardTypeEnum.berry:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Berry.png");
                break;
            case CardTypeEnum.tree:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Tree.png");
                break;
            case CardTypeEnum.mine:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Mine.png");
                break;
            default:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Wood.png");
                break;
        }

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
            if (mouseButton.Pressed && isMouseOver())
            {
                isDragging = true;
                oldMousePosition = GetGlobalMousePosition();

                // Set y position so the card is rendered above other cards
                ZIndex = 1;
            }
            else
            {
                isDragging = false;
                ZIndex = 0;
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
