using Goodot15.Scripts.Game.Model;

public abstract class LivingEnemy(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode) {
	public int AttackDamage { get; protected set; }
}