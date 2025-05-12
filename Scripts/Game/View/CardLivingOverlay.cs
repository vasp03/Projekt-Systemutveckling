using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class CardLivingOverlay : Node2D {
    private TextureProgressBar healthBar;
    private Sprite2D healthIcon;
    private TextureProgressBar saturationBar;
    private Sprite2D saturationIcon;

    public override void _Ready() {
        healthBar = GetNode<TextureProgressBar>("HealthBar");
        saturationBar = GetNode<TextureProgressBar>("SaturationBar");
        healthIcon = GetNode<Sprite2D>("HealthIcon");
        saturationIcon = GetNode<Sprite2D>("SaturationIcon");
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth) {
        if (healthBar is null) healthBar = GetNode<TextureProgressBar>("HealthBar");
        healthBar.MaxValue = maxHealth;
        healthBar.Value = currentHealth;
    }

    public void UpdateSaturationBar(int currentSaturation, int maxSaturation) {
        if (saturationBar is null) saturationBar = GetNode<TextureProgressBar>("SaturationBar");
        saturationBar.MaxValue = maxSaturation;
        saturationBar.Value = currentSaturation;
    }
}