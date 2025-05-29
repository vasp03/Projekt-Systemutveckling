using Godot;
using Goodot15.Scripts.Game.Model;

namespace Goodot15.Scripts.Game.View;

/// <summary>
///     A graphical display of 2 bars showing the health and saturation for a <see cref="CardLiving" />
/// </summary>
public partial class CardLivingOverlay : Node2D {
    public override void _Ready() {
        InitializeReferences();
    }

    /// <summary>
    ///     Updates the health bar with <see cref="currentHealth" /> out of <see cref="maxHealth" />
    /// </summary>
    /// <param name="currentHealth">Current health</param>
    /// <param name="maxHealth">Max health</param>
    public void UpdateHealthBar(int currentHealth, int maxHealth) {
        HealthBar.MaxValue = maxHealth;
        HealthBar.Value = currentHealth;
    }

    /// <summary>
    ///     Updates the saturation bar with <see cref="currentSaturation" /> out of <see cref="maxSaturation" />
    /// </summary>
    /// <param name="currentSaturation">Current saturation</param>
    /// <param name="maxSaturation">Max saturation</param>
    public void UpdateSaturationBar(int currentSaturation, int maxSaturation) {
        SaturationBar.MaxValue = maxSaturation;
        SaturationBar.Value = currentSaturation;
    }

    #region Object references

    private TextureProgressBar HealthBar => GetNode<TextureProgressBar>("HealthBar");
    private Sprite2D HealthIcon { get; set; }
    private TextureProgressBar SaturationBar => GetNode<TextureProgressBar>("SaturationBar");
    private Sprite2D SaturationIcon { get; set; }

    private void InitializeReferences() {
        HealthIcon = GetNode<Sprite2D>("HealthIcon");
        SaturationIcon = GetNode<Sprite2D>("SaturationIcon");
    }

    #endregion Object references
}