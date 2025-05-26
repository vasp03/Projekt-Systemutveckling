using System;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model;

/// <summary>
///     Cards that are considered to be alive - have health and hunger
/// </summary>
public abstract class CardLiving
    : Card, ITickable, ICardConsumer {
    public abstract bool ConsumeCard(Card otherCard);

    public virtual void PostTick(double delta) {
        ExecuteTickLogic();
    }

    protected virtual void ExecuteTickLogic() {
        // Resets starvation tick progress if no longer starving
        if (TicksUntilFullyStarved != -1 && StarvationTickProgress >= TicksUntilFullyStarved)
            StarvationTickProgress = 0;

        ExecuteHungerProgress();

        ExecuteHungerLogic();

        ExecuteHealingLogic();

        ExecuteDeathLogic();
    }

    /// <summary>
    ///     Increments the tick counter progress for starvation or hunger
    /// </summary>
    protected virtual void ExecuteHungerProgress() {
        if (Saturation <= 0)
            StarvationTickProgress++;
        else
            HungerTickProgress++;
    }

    /// <summary>
    ///     Decreases <see cref="Saturation" /> by <see cref="SaturationLossPerCycle" /> when <see cref="HungerTickProgress" />
    ///     is more than or equal to <see cref="TicksUntilSaturationDecrease" />
    /// </summary>
    protected virtual void ExecuteHungerLogic() {
        // Resets hunger tick progress once it reaches TicksUntilSaturationDecrease
        if (TicksUntilSaturationDecrease == -1 || HungerTickProgress < TicksUntilSaturationDecrease) return;
        HungerTickProgress = 0;
        Saturation -= SaturationLossPerCycle != -1
            ? SaturationLossPerCycle
            : 0;
    }

    /// <summary>
    ///     Increases <see cref="Health" /> by <see cref="HealthGainPerCycle" /> when <see cref="HealTickProgress" /> is more
    ///     than or equal to <see cref="TicksUntilHeal" /> and <see cref="Saturation" /> is more than
    ///     <see cref="MaximumSaturation" />/2 and <see cref="Health" /> is less than <see cref="MaximumHealth" />
    /// </summary>
    protected virtual void ExecuteHealingLogic() {
        if (MaximumSaturation / 2 > Saturation || Health >= MaximumHealth) return;
        if (HealTickProgress >= TicksUntilHeal) {
            HealTickProgress = 0;
            Health += HealthGainPerCycle;
            Saturation -= SaturationLossPerHeal;
        } else {
            HealTickProgress++;
        }
    }

    /// <summary>
    ///     Determines if this living card should be dead or not
    /// </summary>
    protected virtual void ExecuteDeathLogic() {
        // Determines if the CardLiving is dead
        if (Health <= 0 || Saturation <= 0) {
            deathTimer--;
            if (deathTimer <= 0) {
                CardNode.CardController.CheckForGameOver(true);
                CardNode.Destroy();
            } else {
                CardNode.Modulate = new Color(1f, .5f, .5f);
            }
            // CardNode.CardType = new ErrorCard();
        } else {
            if (damageEffectPulseTickCount <= 0) return;
            damageEffectPulseTickCount--;

            CardNode.Modulate = new Color(
                1f,
                1f - damageEffectPulseTickCount / (float)DAMAGE_EFFECT_PULSE_TICK_DELAY / 2f,
                1f - damageEffectPulseTickCount / (float)DAMAGE_EFFECT_PULSE_TICK_DELAY / 2f
            );
        }
    }

    #region Health-related

    private int deathTimer = Utilities.TimeToTicks(5);

    private static readonly int DAMAGE_EFFECT_PULSE_TICK_DELAY = Utilities.TimeToTicks(1);
    private int damageEffectPulseTickCount;

    /// <summary>
    ///     Health for this unit
    /// </summary>
    private int health;

    public CardLiving(string textureAddress, bool movable) : base(textureAddress, movable) {
        Health = MaximumHealth;
        Saturation = MaximumSaturation;
    }

    /// <summary>
    ///     Current hit/health points
    /// </summary>
    public int Health {
        get => health;
        set {
            // If the health was decreased
            if (value < health) {
                damageEffectPulseTickCount = DAMAGE_EFFECT_PULSE_TICK_DELAY;
                HurtSound();
            }

            health = Math.Max(0, value);
        }
    }

    private const string HURT_SFX = "General Sounds/Negative Sounds/sfx_sounds_damage1.wav";

    private void HurtSound() {
        if (health > 0)
            GameController.Singleton.SoundController
                .PlaySound(HURT_SFX);
    }

    /// <summary>
    ///     Starting health
    /// </summary>
    public abstract int MaximumHealth { get; }

    /// <summary>
    ///     Health regained for each heal cycle
    /// </summary>
    public abstract int HealthGainPerCycle { get; }

    /// <summary>
    ///     How many ticks between each heal cycle
    /// </summary>
    public abstract int TicksUntilHeal { get; }

    private int healTickProgress;

    /// <summary>
    ///     Current healing tick counter progress
    /// </summary>
    public int HealTickProgress {
        get => TicksUntilHeal == -1 ? 0 : healTickProgress;
        protected set => healTickProgress = Math.Max(0, value);
    }

    public void Kill() {
        Health = 0;
    }

    #endregion Health-related

    #region Hunger-related mechanics

    /// <summary>
    ///     Counter for ticks, used for decreasing hunger when it reaches <see cref="TicksUntilSaturationDecrease" />
    /// </summary>
    private int hungerTickCount;

    /// <summary>
    ///     Current Hunger Tick Progress, in ticks
    /// </summary>
    public int HungerTickProgress {
        get => TicksUntilSaturationDecrease == -1
            ? 0
            : hungerTickCount;
        protected set => hungerTickCount = Math.Max(0, value);
    }

    /// <summary>
    ///     Current saturation, Ranges from 0-<see cref="MaximumSaturation" />
    /// </summary>
    private int currentSaturation;

    /// <summary>
    ///     Current saturation points
    /// </summary>
    public int Saturation {
        get => TicksUntilSaturationDecrease != -1
            ? currentSaturation
            : -1;
        set => currentSaturation = Math.Max(0, value);
    }

    /// <summary>
    ///     How many points required to fully feed
    /// </summary>
    public int Hunger => MaximumSaturation - Saturation;

    /// <summary>
    ///     Determines the maximum saturation
    /// </summary>
    public abstract int MaximumSaturation { get; }

    /// <summary>
    ///     Starvation progress in ticks
    /// </summary>
    private int starvationTickCount;

    /// <summary>
    ///     Current starvation progress, in ticks
    /// </summary>
    public int StarvationTickProgress {
        get => TicksUntilFullyStarved == -1
            ? starvationTickCount
            : -1;
        set => starvationTickCount = Math.Clamp(value, 0, TicksUntilFullyStarved);
    }

    /// <summary>
    ///     Determines for how many ticks a card living can exist until it starves to death. <code>-1</code> means it does
    ///     nothing<br />
    ///     Make use of <see cref="Utilities.TimeToTicks" /> or <see cref="Utilities.GameScaledTimeToTicks" />
    /// </summary>
    public abstract int TicksUntilFullyStarved { get; }

    /// <summary>
    ///     Determines for how many ticks a card living can exist until it starves to death. <code>-1</code> means it does
    ///     nothing<br />
    ///     Make use of <see cref="Utilities.TimeToTicks" /> or <see cref="Utilities.GameScaledTimeToTicks" />
    /// </summary>
    public abstract int TicksUntilSaturationDecrease { get; }

    /// <summary>
    ///     How many saturation points are lost during each cycle of <see cref="TicksUntilSaturationDecrease" />
    /// </summary>
    public abstract int SaturationLossPerCycle { get; }

    /// <summary>
    ///     How many saturation points are lost during each cycle of healing
    /// </summary>
    public abstract int SaturationLossPerHeal { get; }

    #endregion
}