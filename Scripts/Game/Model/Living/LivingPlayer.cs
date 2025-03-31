using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;

public class LivingPlayer(string textureAddress, bool movable, int cost, int health, Goodot15.Scripts.Game.Model.CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode), IStackable {
	public int Saturation { get; set; }
	public int AttackDamage { get; set; }
}