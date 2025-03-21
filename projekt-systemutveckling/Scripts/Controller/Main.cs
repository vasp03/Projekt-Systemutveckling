using System;
using Godot;

public partial class Main : Node
{
    // Get when a key is pressed
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.Space)
            {
                GD.Print("Space key pressed");

                // Get the child named "CardTemplate"
                var cardTemplate = GetNode<Cards>("CardTemplate");

                if(cardTemplate == null)
                {
                    GD.PrintErr("CardTemplate not found");
                    return;
                }

                // Call the createNew method
                cardTemplate.CreateNew();
            }else if(eventKey.Pressed && eventKey.Keycode == Key.Escape)
            {
                // Prints number of cilhden
                GD.Print(GetChildCount());

            }
        }
    }
}
