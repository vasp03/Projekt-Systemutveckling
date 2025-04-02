using System;
using System.Collections.Generic;
using Goodot15.Scripts;
using Goodot15.Scripts.Game;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;

public class LivingPlayer(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode), ITickable, ICardConsumer {
	public static readonly int STARVATION_TICK_DELAY = Utilities.timeToTicks(days: 3);
	public static readonly int HUNGER_TICK_DELAY = Utilities.timeToTicks(days: 1);
	
	private int _saturation;
	private int _hungerTickCount;
	private int _starvationTickCount;

	public int Saturation {
		get => _saturation;
		set => _saturation = Math.Max(0, value);
	}

	public int StarvationTickProgress {
		get => _starvationTickCount;
		set => this._starvationTickCount = Math.Clamp(value, 0, STARVATION_TICK_DELAY);
	}

	public int HungerTickProgress {
		get => _hungerTickCount;
		set => _hungerTickCount = Math.Max(0, value);
	}

	public int AttackDamage { get; set; }

	public void postTick() {
		if (Saturation <= 0)
			this.StarvationTickProgress++;
		else
			this.HungerTickProgress++;
	}

	private void checkTickProgress() {
		if (this.StarvationTickProgress >= STARVATION_TICK_DELAY) {
			// Death(?)
		}

		if (this.HungerTickProgress >= HUNGER_TICK_DELAY) {
			this.Saturation--;
		}
	}

	public bool ConsumeCard(Card otherCard) {
		throw new NotImplementedException();
	}
}