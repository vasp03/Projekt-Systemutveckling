using Godot;
using Goodot15.Scripts.Game.Model.Div;

namespace Goodot15.Scripts.Game.Controller;

public partial class PackButton : TextureButton {
    public delegate void PackClickedHandler(CardPack pack);

    private Label _costLabel;
    private Vector2 _originalPosition;
    private TextureRect _textureRect;

    private Tween _tween;
    public CardPack Pack { get; private set; }
    public event PackClickedHandler PackClicked;

    public override void _Ready() {
        _costLabel = GetNode<Label>("PriceLabel");
        _originalPosition = Position;

        this.MouseEntered += OnMouseEntered;
        this.MouseExited += OnMouseExited;
    }

    public void SetPack(CardPack pack) {
        Pack = pack;

        _costLabel ??= GetNode<Label>("PriceLabel");

        _costLabel.Text = pack.Cost == 0 ? "Free" : $"{pack.Cost}g";

        string texturePath = $"res://Assets/Packs/{pack.Name.Replace(" ", "_")}.png";
        if (ResourceLoader.Exists(texturePath))
            TextureNormal = GD.Load<Texture2D>(texturePath);
        else
            GD.PrintErr($"Texture not found for pack: {pack.Name}");
    }

    public void OnMouseEntered() {
        _originalPosition = Position;
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.1f);
    }

    public void OnMouseExited() {
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", Vector2.One, 0.1f);
        _tween.TweenProperty(this, "position", _originalPosition, 0.1f);
    }

    public override void _Pressed() {
        PackClicked?.Invoke(Pack);
    }

    public void SetPriceColor(Color color) {
        _costLabel.Modulate = color;
    }
}