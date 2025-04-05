using System;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;

public class LivingAnimal(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode), IStackable, ITickable {
	private int produceTimer;

	public IStackable NeighbourAbove { get; set; }
	public IStackable NeighbourBelow { get; set; }

	public void SetNeighbourAbove(IStackable card) {
		throw new NotImplementedException();
	}

	public void SetNeighbourBelow(IStackable card) {
		throw new NotImplementedException();
	}

	public void preTick() {
	}

	public void postTick() {
	}

	public int GetProduceTimer() {
		return produceTimer--;
	}
}