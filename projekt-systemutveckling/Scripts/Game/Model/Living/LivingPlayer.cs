using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;

public class LivingPlayer(string textureAddress, bool movable, int cost, int health)
	: CardLiving(textureAddress, movable, cost, health), IStackable {
	public int Saturation { get; set; }
	public int AttackDamage { get; set; }

	public IReadOnlyCollection<Type> GetStackableTypes() {
		return [];
	}

	public IStackable NeighbourAbove { get; set; }
	public IStackable NeighbourBelow { get; set; }

	public void SetNeighbourAbove(IStackable card) {
		throw new NotImplementedException();
	}

	public void SetNeighbourBelow(IStackable card) {
		throw new NotImplementedException();
	}

	public bool CanStackWith(Card card) {
		throw new NotImplementedException();
	}
}