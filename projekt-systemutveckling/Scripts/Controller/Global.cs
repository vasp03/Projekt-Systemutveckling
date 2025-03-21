using Godot;
using System;

public partial class Global : Node
{
    private static Boolean cardSelected = false;

    public static void SetCardSelected(Boolean value)
    {
        cardSelected = value;
    }

    public static Boolean GetCardSelected()
    {
        return cardSelected;
    }
}
