namespace Goodot15.Scripts.Game.Model;

public abstract class CardLiving(string name, string textureAddress, bool movable, int cost, int health)
	: Card(name, textureAddress, movable, cost) {
	public int Health { get; protected set; } = health;
}