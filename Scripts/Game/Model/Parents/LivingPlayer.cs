using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Living;

public class LivingPlayer(string texturePath) : CardLiving(texturePath, true) {
    public readonly static int STARVATION_TICK_DELAY = Utilities.GameScaledTimeToTicks(days: 3);
    public readonly static int HUNGER_TICK_DELAY = Utilities.GameScaledTimeToTicks(days: 1);

    public readonly static int HEAL_TICK_DELAY = Utilities.GameScaledTimeToTicks(hours: 1);

    public readonly static int HEAL_GAIN_PER_CYCLE = 25;
    public readonly static int SATURATION_LOSS_PER_HEAL = 25;

    public override int Value => -1;
    public override int BaseHealth => 100;
    public override int HealthGainPerCycle => HEAL_GAIN_PER_CYCLE;
    public override int TicksUntilHeal => HEAL_TICK_DELAY;
    public override int MaximumSaturation => 100;
    public override int TicksUntilFullyStarved => STARVATION_TICK_DELAY;
    public override int TicksUntilSaturationDecrease => HUNGER_TICK_DELAY;
    public override int SaturationLossPerCycle => 30;
    public override int SaturationLossPerHeal => 25;

    public override bool ConsumeCard(Card otherCard) {
        if (otherCard is not IEdible edibleCard) return false;
        Saturation += edibleCard.ConsumeFood(Hunger);
        return false;
    }
}