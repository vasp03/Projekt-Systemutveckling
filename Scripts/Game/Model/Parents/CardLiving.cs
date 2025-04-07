using System;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model;

public abstract class CardLiving(string textureAddress, bool movable)
	: Card(textureAddress, movable), ITickable, ICardConsumer {
	private int _health;

	private int _hungerTickCount;

	private int _saturation;

	private int _starvationTickCount;
	public abstract int? TicksUntilFullyStarved { get; }
	public abstract int? TicksUntilSaturationDecrease { get; }

	public int Health {
		get => _health;
		set => _health = Math.Max(0, value);
	}

	public int HungerTickProgress {
		get => TicksUntilSaturationDecrease.HasValue ? _hungerTickCount : -1;
		protected set => _hungerTickCount = Math.Max(0, value);
	}

	public int Saturation {
		get => TicksUntilSaturationDecrease.HasValue ? _hungerTickCount : -1;
		set => _saturation = Math.Max(0, value);
	}

	public int StarvationTickProgress {
		get => TicksUntilFullyStarved.HasValue ? _starvationTickCount : -1;
		set => _starvationTickCount = Math.Clamp(value, 0, TicksUntilFullyStarved ?? 0);
	}

	public abstract bool ConsumeCard(Card otherCard);

	public virtual void PostTick() {
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