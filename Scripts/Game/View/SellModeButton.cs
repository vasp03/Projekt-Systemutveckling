using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class SellModeButton : TextureButton {
    private Texture2D iconOff;
    private Texture2D iconOn;

    public GameController GameController { get; set; }

    public override void _Ready() {
        Pressed += OnButtonPressed;

        iconOn = GD.Load<Texture2D>("res://Assets/UI/Sell/sell_on.png");
        iconOff = GD.Load<Texture2D>("res://Assets/UI/Sell/sell_off.png");

        CallDeferred(nameof(Setup));
    }

    private void Setup() {
        GameController = GameController.Singleton;

        if (GameController is null) {
            GD.PrintErr("SellModeButton: GameController.Singleton is still null.");
            return;
        }

        UpdateIcon();
    }

    private void OnButtonPressed() {
        GD.Print($"[DEBUG] CurrentScene: {GetTree().CurrentScene?.Name}");
        GD.Print($"[DEBUG] GameController.Singleton: {GameController.Singleton}");
        if (GameController is null) {
            GD.PrintErr("SellModeButton: GameController is null.");
            return;
        }

        GameController.ToggleSellMode();
        UpdateIcon();
    }

    public void UpdateIcon() {
        TextureNormal = GameController.SellModeActive ? iconOn : iconOff;
    }
}