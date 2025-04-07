namespace Goodot15.Scripts.Game.Model.Living;

public abstract class LivingEnemy
	: CardLiving {
	public LivingEnemy(string textureAddress, bool movable, int cost, int health, CardNode cardNode) : base(
		textureAddress, movable) {
	}

	public int AttackDamage { get; protected set; }
}