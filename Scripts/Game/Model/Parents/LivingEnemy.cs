namespace Goodot15.Scripts.Game.Model.Living;

public abstract class LivingEnemy(string textureAddress, bool movable, int cardValue)
    : CardLiving(textureAddress, movable, cardValue) {
    public int AttackDamage { get; protected set; }
}