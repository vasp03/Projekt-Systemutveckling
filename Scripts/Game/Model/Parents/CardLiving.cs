using System;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model;

public abstract class CardLiving
    : Card, ITickable, ICardConsumer {
    public abstract bool ConsumeCard(Card otherCard);

    public virtual void PostTick() {
        if (Saturation <= 0)
            StarvationTickProgress++;
        else
            HungerTickProgress++;

        ExecuteTickLogic();
    }

    protected virtual void ExecuteTickLogic() {
        if (TicksUntilFullyStarved != -1 && StarvationTickProgress >= TicksUntilFullyStarved)
            StarvationTickProgress = 0;

        if (TicksUntilSaturationDecrease != -1 && HungerTickProgress >= TicksUntilSaturationDecrease) {
            HungerTickProgress = 0;
            Saturation -= SaturationLossPerCycle != -1
                ? SaturationLossPerCycle
                : 0;
        }

        if (Health <= 0 || Saturation <= 0) {
            deathTimer--;
            if (deathTimer <= 0) {
                CardNode.Destroy();
                CardNode.CardController.CheckForGameOver(true);
            } else {
                CardNode.Modulate = new Color(1f, .5f, .5f);
            }
            // CardNode.CardType = new ErrorCard();
        } else {
            if (remainingDamageEffectPulseTimer <= 0) return;
            remainingDamageEffectPulseTimer--;

            CardNode.Modulate = new Color(
                1f,
                1f - remainingDamageEffectPulseTimer / (float)damageEffectPulseTimer / 2f,
                1f - remainingDamageEffectPulseTimer / (float)damageEffectPulseTimer / 2f
            );
        }
    }

    #region Health-related

    private int deathTimer = Utilities.TimeToTicks(5);

    private static readonly int damageEffectPulseTimer = Utilities.TimeToTicks(1);
    private int remainingDamageEffectPulseTimer;

    /// <summary>
    ///     Health for this unit
    /// </summary>
    private int _health;

    public CardLiving(string textureAddress, bool movable) : base(textureAddress, movable) {
        Health = BaseHealth;
        Saturation = MaximumSaturation;
    }

    /// <summary>
    ///     Current hit/health points
    /// </summary>
    public int Health {
        get => _health;
        set {
            if (value < _health) {
                remainingDamageEffectPulseTimer = damageEffectPulseTimer;
                HurtSound();
            }

            _health = Math.Max(0, value);
        }
    }

    private void HurtSound() {
        if (_health > 0)
            GameController.Singleton.SoundController
                .PlaySound("General Sounds/Negative Sounds/sfx_sounds_damage1.wav");
    }

    /// <summary>
    ///     Starting health
    /// </summary>
    public abstract int BaseHealth { get; }

    #endregion Health-related

    #region Hunger-related mechanics

    /// <summary>
    ///     Counter for ticks, used for decreasing hunger when it reaches <see cref="TicksUntilSaturationDecrease" />
    /// </summary>
    private int _hungerTickCount;

    /// <summary>
    ///     Current Hunger Tick Progress, in ticks
    /// </summary>
    public int HungerTickProgress {
        get => TicksUntilSaturationDecrease == -1
            ? 0
            : _hungerTickCount;
        protected set => _hungerTickCount = Math.Max(0, value);
    }

    /// <summary>
    ///     Current saturation, Ranges from 0-<see cref="MaximumSaturation" />
    /// </summary>
    private int _saturation;

    /// <summary>
    ///     Current saturation points
    /// </summary>
    public int Saturation {
        get => TicksUntilSaturationDecrease != -1
            ? _saturation
            : -1;
        set => _saturation = Math.Max(0, value);
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
    ///     Starvation progres in ticks
    /// </summary>
    private int _starvationTickCount;

    /// <summary>
    ///     Current starvation progress, in ticks
    /// </summary>
    public int StarvationTickProgress {
        get => TicksUntilFullyStarved == -1
            ? _starvationTickCount
            : -1;
        set => _starvationTickCount = Math.Clamp(value, 0, TicksUntilFullyStarved);
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

    #endregion
}