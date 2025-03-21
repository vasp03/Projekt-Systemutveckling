using Godot;

namespace Goodot15.Scripts.Model;

public partial class CustomCursor : Node
{
    private Texture2D _openHand;
    private Texture2D _closedHand;

    public override void _Ready()
    {
        _openHand = LoadTexture("res://Assets/MouseCursor/hand_open.png");
        _closedHand = LoadTexture("res://Assets/MouseCursor/hand_closed.png");

        if (_openHand == null || _closedHand == null)
        {
            GD.PrintErr("Custom cursor not found");
            return;
        }

        SetCursor(_openHand);
    }

    // Detect when the mouse is pressed
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed)
            {
                SetCursor(_closedHand);
            }
            else
            {
                SetCursor(_openHand);
            }
        }
    }

    private void SetCursor(Texture2D texture)
    {
        if (texture == null || _openHand == null || _closedHand == null)
        {
            GD.PrintErr("Custom cursor not found");
            return;
        }

        Vector2 cursorOffset = new Vector2(texture.GetWidth() / 2, texture.GetHeight() / 2);
        Input.SetCustomMouseCursor(texture, Input.CursorShape.Arrow, cursorOffset);
    }

    private Texture2D LoadTexture(string path)
    {
        Texture2D customMouse = ResourceLoader.Load<Texture2D>(path);

        if (customMouse is CompressedTexture2D compressedTex)
        {
            Image image = compressedTex.GetImage();
            ImageTexture newTexture = ImageTexture.CreateFromImage(image);
            return newTexture;
        }

        return customMouse;
    }

    public override void _ExitTree()
    {
        Input.SetCustomMouseCursor(null);
    }
}
