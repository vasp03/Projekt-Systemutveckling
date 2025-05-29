using Godot;
using System;

public partial class CardValueOverlay : Node2D {
    private Sprite2D valueBorder;
    private Label valueLabel;
    
    public override void _Ready() {
        valueBorder = GetNode<Sprite2D>("ValueBorder");
        valueLabel = GetNode<Label>("ValueBorder/Centering/HContainer/ValueLabel");
    }

    /// <summary>
    /// Sets the value label to the cards value. If value is 0 (Can't be sold), X is shown.
    /// </summary>
    /// <param name="value">The value that is being set</param>
    public void SetValue(int value) {
        valueLabel ??= GetNode<Label>("ValueBorder/Centering/HContainer/ValueLabel");
        valueLabel.Text = value is -1 ? "X" : value.ToString();
    }
}



