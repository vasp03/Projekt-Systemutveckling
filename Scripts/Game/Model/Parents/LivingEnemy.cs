namespace Goodot15.Scripts.Game.Model.Living;

public abstract class LivingEnemy(string textureAddress, bool movable)
    : CardLiving(textureAddress, movable) {
    public int AttackDamage { get; protected set; }
}