using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;

public class LivingPlayer(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode), IStackable {
	private int _saturation;

	public int Saturation {
		get => _saturation;
		set => _saturation = Math.Max(0, value);
	}

	public int AttackDamage { get; set; }

	public IStackable NeighbourAbove { get; set; }
	public IStackable NeighbourBelow { get; set; }

	public void SetNeighbourAbove(IStackable card) {
		throw new NotImplementedException();
	}

	public void SetNeighbourBelow(IStackable card) {
		throw new NotImplementedException();
	}

	public IReadOnlyCollection<Type> GetStackableTypes() {
		return [];
	}

	public bool CanStackWith(Card card) {
		throw new NotImplementedException();
	}
}