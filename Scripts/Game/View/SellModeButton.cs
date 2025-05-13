using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class SellModeButton : TextureButton {
    private static readonly Texture2D ICON_OFF = GD.Load<Texture2D>("res://Assets/UI/Sell/sell_on.png");
    private static readonly Texture2D _iconOn = GD.Load<Texture2D>("res://Assets/UI/Sell/sell_off.png");

    public GameController GameController { get; set; }

    public override void _Ready() {
        Pressed += OnButtonPressed;
    }

    private void OnButtonPressed() {
        GameController.ToggleSellMode();
        UpdateIcon();
    }

    public void UpdateIcon() {
        TextureNormal = GameController.SellModeActive
            ? _iconOn
            : ICON_OFF;
    }
}