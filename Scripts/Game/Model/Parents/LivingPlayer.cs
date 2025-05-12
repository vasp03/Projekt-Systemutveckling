using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Living;

public class LivingPlayer(string texturePath) : CardLiving(texturePath, true) {
    public static readonly int STARVATION_TICK_DELAY = Utilities.GameScaledTimeToTicks(days: 3);
    public static readonly int HUNGER_TICK_DELAY = Utilities.GameScaledTimeToTicks(days: 1);

    public int AttackDamage { get; set; }

    public override int BaseHealth => 100;

    public override int MaximumSaturation => 100;
    public override int TicksUntilFullyStarved => STARVATION_TICK_DELAY;
    public override int TicksUntilSaturationDecrease => HUNGER_TICK_DELAY;
    public override int SaturationLossPerCycle => 30;
    
    protected override int SetValue() {
        return -1;
    }

    public override bool ConsumeCard(Card otherCard) {
        if (otherCard is not IEdible edibleCard) return false;
        Saturation += edibleCard.ConsumeFood(Hunger);
        return false;
    }
}