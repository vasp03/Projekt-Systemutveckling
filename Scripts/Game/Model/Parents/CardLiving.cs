namespace Goodot15.Scripts.Game.Model;

public abstract class CardLiving(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: Card(textureAddress, movable, cost, cardNode) {
	public int Health { get; protected set; } = health;
}