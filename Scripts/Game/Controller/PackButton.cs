namespace Goodot15.Scripts.Game.Controller;

using Godot;
using Goodot15.Scripts.Game.Model.Div;

public partial class PackButton : Button {
    public CardPack Pack { get; private set; }

    public delegate void PackClickedHandler(CardPack pack);
    public event PackClickedHandler PackClicked;

    private Label _costLabel;
    private TextureRect _textureRect;

    public override void _Ready() {
        _costLabel = GetNode<Label>("CostLabel");
        _textureRect = GetNode<TextureRect>("TextureRect");
    }

    public void SetPack(CardPack pack) {
        Pack = pack;
        Text = pack.Name;
        _costLabel.Text = pack.Cost == 0 ? "Free" : $"{pack.Cost}g";

        // Load and set the texture for the pack
        string texturePath = $"res://Assets/Packs/{pack.Name.Replace(" ", "_")}.png";
        if (ResourceLoader.Exists(texturePath)) {
            _textureRect.Texture = GD.Load<Texture2D>(texturePath);
        } else {
            GD.PrintErr($"Texture not found for pack: {pack.Name}");
        }
    }

    public override void _Pressed() {
        PackClicked?.Invoke(Pack);
    }
    
    public void SetPriceColor(Color color) {
        _costLabel.Modulate = color;
    }
}