using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Living;

public class LivingPlayer(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode) {
	public static readonly int STARVATION_TICK_DELAY = Utilities.timeToTicks(days: 3);
	public static readonly int HUNGER_TICK_DELAY = Utilities.timeToTicks(days: 1);

	public int AttackDamage { get; set; }

	public override int? TicksUntilFullyStarved => STARVATION_TICK_DELAY;
	public override int? TicksUntilSaturationDecrease => HUNGER_TICK_DELAY;

	public override bool ConsumeCard(Card otherCard) {
		if (otherCard is IEdible edibleCard) {
			this.Saturation += edibleCard.ConsumeFood(1);
		}
		return false;
	}
}