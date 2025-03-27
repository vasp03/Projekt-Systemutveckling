using System;
using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;

public class LivingAnimal(string textureAddress, bool movable, int cost, int health)
	: CardLiving(textureAddress, movable, cost, health), IStackable {
	private int produceTimer;

	public IReadOnlyCollection<Type> GetStackableTypes() {
		throw new NotImplementedException();
	}

	public IStackable NeighbourAbove { get; set; }
	public IStackable NeighbourBelow { get; set; }

	public void SetNeighbourAbove(IStackable card) {
		throw new NotImplementedException();
	}

	public void SetNeighbourBelow(IStackable card) {
		throw new NotImplementedException();
	}

	public int tickTimer() {
		return produceTimer--;
	}
}