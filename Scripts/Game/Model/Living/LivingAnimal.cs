using System;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Living;

public abstract class LivingAnimal(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode), ITickable, ICardProducer {
	public virtual int? TicksUntilProducedCard => Utilities.timeToTicks(days: 0.5d);

	private int _produceTimer;
	public int ProduceTickProgress {
		get => _produceTimer;
		set => this._produceTimer = Math.Max(0, value);
	}

	public override int? TicksUntilFullyStarved => Utilities.timeToTicks(days: 3d);
	public override int? TicksUntilSaturationDecrease => Utilities.timeToTicks(days: 1d);
	public void postTick() {
		base.postTick();
		if (TicksUntilProducedCard is not null && Saturation > 0) {
			this.ProduceTickProgress++;
		}
	}

	protected override void CheckTickProgress() {
		base.CheckTickProgress();
		if (this.ProduceTickProgress >= this.TicksUntilProducedCard) {
			this.ProduceTickProgress = 0;
			this.CardNode.CardController.CreateCard(this.ProduceCard(), this.CardNode.Position + Vector2.Down * 15f);
		}
	}

	public abstract Card ProduceCard();
}