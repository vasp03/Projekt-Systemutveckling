using System;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model;

public abstract class CardLiving
    : Card, ITickable, ICardConsumer {
    public abstract bool ConsumeCard(Card otherCard);

    public virtual void PostTick(double delta) {
        if (Saturation <= 0)
            StarvationTickProgress++;
        else
            HungerTickProgress++;

        ExecuteTickLogic();
    }

    protected virtual void ExecuteTickLogic() {
        // Resets starvation tick progress if no longer starving
        if (TicksUntilFullyStarved != -1 && StarvationTickProgress >= TicksUntilFullyStarved)
            StarvationTickProgress = 0;

        ExecuteHungerLogic();

        ExecuteHungerLogic();

        ExecuteHealingLogic();

        ExecuteDeathLogic();
    }

    protected virtual void ExecuteHungerLogic() {
        // Resets hunger tick progress once it reaches TicksUntilSaturationDecrease
        if (TicksUntilSaturationDecrease == -1 || HungerTickProgress < TicksUntilSaturationDecrease) return;
        HungerTickProgress = 0;
        Saturation -= SaturationLossPerCycle != -1
            ? SaturationLossPerCycle
            : 0;
    }

    /// <summary>
    ///     Heals Villagers and makes card flash green after a fixed amount of ticks, when health is below maximum and
    ///     saturation is above half of maximum saturation.
    /// </summary>
    protected virtual void ExecuteHealingLogic() {
        if (MaximumSaturation / 2 > Saturation || Health >= BaseHealth) {
            if (healingEffectPulseTickCount > 0) {
                healingEffectPulseTickCount--;
                CardNode.Modulate = new Color(
                    1f - healingEffectPulseTickCount / (float)HEALING_EFFECT_PULSE_TICK_DELAY / 2f,
                    1f,
                    1f - healingEffectPulseTickCount / (float)HEALING_EFFECT_PULSE_TICK_DELAY / 2f
                );
            } else {
                CardNode.Modulate = new Color(1f, 1f, 1f);
            }
            return;
        }

        if (HealTickProgress >= TicksUntilHeal) {
            HealTickProgress = 0;
            Health += HealthGainPerCycle;
            Saturation -= SaturationLossPerHeal;
            healingEffectPulseTickCount = HEALING_EFFECT_PULSE_TICK_DELAY;
        } else {
            HealTickProgress++;
        }

        if (healingEffectPulseTickCount > 0) {
            healingEffectPulseTickCount--;
            CardNode.Modulate = new Color(
                1f - healingEffectPulseTickCount / (float)HEALING_EFFECT_PULSE_TICK_DELAY / 2f,
                1f,
                1f - healingEffectPulseTickCount / (float)HEALING_EFFECT_PULSE_TICK_DELAY / 2f
            );
        }
    }

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

    private static readonly int HEALING_EFFECT_PULSE_TICK_DELAY = Utilities.TimeToTicks(1);
    private int healingEffectPulseTickCount;
    
    private static readonly int DAMAGE_EFFECT_PULSE_TICK_DELAY = Utilities.TimeToTicks(1);
    private int damageEffectPulseTickCount;

    /// <summary>
    ///     Health for this unit
    /// </summary>
    private int health;

    public CardLiving(string textureAddress, bool movable) : base(textureAddress, movable) {
        Health = BaseHealth;
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
                HealTickProgress = 0;
            }

            health = Math.Max(0, value);
        }
    }

    private void HurtSound() {
        if (health > 0)
            GameController.Singleton.SoundController
                .PlaySound("General Sounds/Negative Sounds/sfx_sounds_damage1.wav");
    }

    /// <summary>
    ///     Starting health
    /// </summary>
    public abstract int BaseHealth { get; }

    /// <summary>
    ///     Health regained for each heal cycle
    /// </summary>
    public abstract int HealthGainPerCycle { get; }

    /// <summary>
    ///     How many ticks between each heal cycle
    /// </summary>
    public abstract int TicksUntilHeal { get; }

    private int healTickProgress;

    public int HealTickProgress {
        get => TicksUntilHeal == -1
            ? 0
            : healTickProgress;
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