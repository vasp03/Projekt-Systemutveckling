namespace Goodot15.Scripts.Game.Model;

public abstract class CardLiving(string textureAddress, bool movable, int cost, int health)
	: Card(textureAddress, movable, cost) {
	public int Health { get; protected set; } = health;
}