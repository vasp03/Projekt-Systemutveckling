using System;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model;

public abstract class CardLiving(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: Card(textureAddress, movable, cost, cardNode), ITickable, ICardConsumer {
	public abstract int? TicksUntilFullyStarved { get; }
	public abstract int? TicksUntilSaturationDecrease { get; }

	private int _health;
	public int Health { 
		get => _health;
		set => _health = Math.Max(0, value);
	}
	
	private int _hungerTickCount;
	public int HungerTickProgress {
		get => TicksUntilSaturationDecrease.HasValue ? _hungerTickCount : -1;
		protected set => _hungerTickCount = Math.Max(0, value);
	}

	private int _saturation;
	public int Saturation {
		get => TicksUntilSaturationDecrease.HasValue ? _hungerTickCount : -1;
		set => _saturation = Math.Max(0, value);
	}
	
	private int _starvationTickCount;
	public int StarvationTickProgress {
		get => TicksUntilFullyStarved.HasValue ? _starvationTickCount : -1;
		set => _starvationTickCount = Math.Clamp(value, 0, this.TicksUntilFullyStarved ?? 0);
	}

	public abstract bool ConsumeCard(Card otherCard);

	public void postTick() {
		if (Saturation <= 0)
			StarvationTickProgress++;
		else
			HungerTickProgress++;
		
		CheckTickProgress();
	}

	protected virtual void CheckTickProgress() {
		if (TicksUntilFullyStarved is not null && StarvationTickProgress >= TicksUntilFullyStarved) {
			// TODO: Death(?)
		}
		
		if (TicksUntilSaturationDecrease is not null && HungerTickProgress >= TicksUntilFullyStarved) Saturation--;
	}
}