using System;
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
}