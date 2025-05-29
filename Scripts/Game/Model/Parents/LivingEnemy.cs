namespace Goodot15.Scripts.Game.Model.Living;

public abstract class LivingEnemy(string cardTextureName, bool movable)
    : CardLiving(cardTextureName, movable) {
    public int AttackDamage { get; protected set; }
}