using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class SellModeButton : TextureButton {
    private Texture2D iconOff;
    private Texture2D iconOn;

    public override void _Ready() {
        Pressed += OnButtonPressed;

        iconOn = GD.Load<Texture2D>("res://Assets/UI/Sell/sell_on.png");
        iconOff = GD.Load<Texture2D>("res://Assets/UI/Sell/sell_off.png");

        Setup();
    }

    private void Setup() {
        UpdateIcon();
    }

    private void OnButtonPressed() {
        GD.Print($"[DEBUG] CurrentScene: {GetTree().CurrentScene?.Name}");
        GD.Print($"[DEBUG] GameController.Singleton: {GameController.Singleton}");

        GameController.Singleton.ToggleSellMode();
        UpdateIcon();
    }

    public void UpdateIcon() {
        TextureNormal = GameController.Singleton.SellModeActive
            ? iconOn
            : iconOff;
    }
}