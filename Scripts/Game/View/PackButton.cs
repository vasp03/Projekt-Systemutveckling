using Godot;
using Goodot15.Scripts.Game.Model.Div;

namespace Goodot15.Scripts.Game.Controller;

public partial class PackButton : TextureButton {
    public delegate void PackClickedHandler(CardPack pack);

    private Tween _tween;
    private Vector2 originalPosition;
    private TextureRect textureRect;

    private Label CostLabel => GetNode<Label>("PriceLabel");
    private CardPack Pack { get; set; }
    public event PackClickedHandler PackClicked;

    public override void _Ready() {
        originalPosition = Position;

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    public void SetPack(CardPack pack) {
        Pack = pack;

        CostLabel.Text = pack.Cost <= 0
            ? "Free"
            : $"{pack.Cost}g";

        string texturePath = Pack.PackButtonTexture;
        if (ResourceLoader.Exists(texturePath))
            TextureNormal = GD.Load<Texture2D>(texturePath);
        else
            GD.PrintErr($"Texture not found for pack: {pack.Name}");
    }

    public void OnMouseEntered() {
        originalPosition = Position;
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.1f);
    }

    public void OnMouseExited() {
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", Vector2.One, 0.1f);
        _tween.TweenProperty(this, "position", originalPosition, 0.1f);
    }

    public override void _Pressed() {
        PackClicked?.Invoke(Pack);
    }

    public void SetPriceColor(Color color) {
        CostLabel.Modulate = color;
    }
}