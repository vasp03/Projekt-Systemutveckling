using Goodot15.Scripts.Game.Model;

public abstract class LivingEnemy(string textureAddress, bool movable, int cost, int health)
	: CardLiving(textureAddress, movable, cost, health) {
	public int AttackDamage { get; protected set; }
}