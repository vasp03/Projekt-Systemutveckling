using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;

public abstract class LivingAnimal(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode), ITickable, ICardProducer {
	private int _produceTimer;
	public virtual int? TicksUntilProducedCard => Utilities.timeToTicks(days: 0.5d);

	public int ProduceTickProgress {
		get => _produceTimer;
		set => _produceTimer = Math.Max(0, value);
	}

	public override int? TicksUntilFullyStarved => Utilities.timeToTicks(days: 3d);
	public override int? TicksUntilSaturationDecrease => Utilities.timeToTicks(days: 1d);

	public abstract Card ProduceCard();

	public override void PostTick() {
		base.PostTick();
		if (TicksUntilProducedCard is not null && Saturation > 0) ProduceTickProgress++;
	}

	protected override void CheckTickProgress() {
		base.CheckTickProgress();
		if (ProduceTickProgress >= TicksUntilProducedCard) {
			ProduceTickProgress = 0;
			CardNode.CardController.CreateCard(ProduceCard(), CardNode.Position + Vector2.Down * 15f);
		}
	}
}