using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Living;

public class LivingPlayer()
	: CardLiving("Villager", true) {
	public static readonly int STARVATION_TICK_DELAY = Utilities.TimeToTicks(days: 3);
	public static readonly int HUNGER_TICK_DELAY = Utilities.TimeToTicks(days: 1);

	public int AttackDamage { get; set; }

	public override int? TicksUntilFullyStarved => STARVATION_TICK_DELAY;
	public override int? TicksUntilSaturationDecrease => HUNGER_TICK_DELAY;

	public override bool ConsumeCard(Card otherCard) {
		if (otherCard is not IEdible edibleCard) return false;
		Saturation += edibleCard.ConsumeFood(1);

		GD.Print(edibleCard.RemainingFood);
		return false;
	}
}