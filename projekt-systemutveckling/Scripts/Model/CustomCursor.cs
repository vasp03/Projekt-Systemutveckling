using Godot;
using System;

public partial class CustomCursor : Node
{
    private Texture2D openHand;
    private Texture2D closedHand;

    public override void _Ready()
    {
        
        openHand = LoadTexture("res://Assets/MouseCursor/hand_open.png");
        closedHand = LoadTexture("res://Assets/MouseCursor/hand_closed.png");

        if (openHand == null || closedHand == null)
        {
            GD.PrintErr("Custom cursor not found");
            return;
        }

        SetCursor(openHand);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("LMB"))
        {
            SetCursor(closedHand);
        }
        else
        {
            SetCursor(openHand);
        }
    }

    private void SetCursor(Texture2D texture)
    {
        if (texture == null) return;

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