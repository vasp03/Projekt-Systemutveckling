using Goodot15.Scripts.Game.Model;

public abstract class LivingEnemy(string name, string textureAddress, bool movable, int cost, int health)
	: CardLiving(name, textureAddress, movable, cost, health) {
	public int AttackDamage { get; protected set; }
}