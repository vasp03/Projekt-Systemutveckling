using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Living;

public abstract class LivingPlayer(string cardTexturePath) : CardLiving(cardTexturePath, true) {
    public const int HEAL_GAIN_PER_CYCLE = 25;
    public const int SATURATION_LOSS_PER_HEAL = 25;

    public const int HUNGER_LOSS_PER_CYCLE = 30;

    public const int MAXMIMUM_HEALTH = 100;
    public const int MAXIMUM_HUNGER = 100;
    public readonly static int STARVATION_TICK_DELAY = Utilities.GameScaledTimeToTicks(days: 3);
    public readonly static int HUNGER_TICK_DELAY = Utilities.GameScaledTimeToTicks(days: 1);

    public readonly static int HEAL_TICK_DELAY = Utilities.GameScaledTimeToTicks(hours: 1);

    public override int Value => -1;
    public override int MaximumHealth => MAXMIMUM_HEALTH;
    public override int HealthGainPerCycle => HEAL_GAIN_PER_CYCLE;
    public override int TicksUntilHeal => HEAL_TICK_DELAY;
    public override int MaximumSaturation => MAXIMUM_HUNGER;
    public override int TicksUntilFullyStarved => STARVATION_TICK_DELAY;
    public override int TicksUntilSaturationDecrease => HUNGER_TICK_DELAY;
    public override int SaturationLossPerCycle => HUNGER_LOSS_PER_CYCLE;
    public override int SaturationLossPerHeal => SATURATION_LOSS_PER_HEAL;

    /// <summary>
    ///     <inheritdoc cref="ICardConsumer.ConsumeCard" /><br />
    ///     Attempts to consume any <see cref="IEdible" /> cards.
    /// </summary>
    /// <param name="otherCard">Other card placed on top of this card</param>
    /// <returns>Always false</returns>
    public override bool ConsumeCard(Card otherCard) {
        if (otherCard is not IEdible edibleCard) return false;
        Saturation += edibleCard.ConsumeFood(Hunger);
        return false;
    }
}