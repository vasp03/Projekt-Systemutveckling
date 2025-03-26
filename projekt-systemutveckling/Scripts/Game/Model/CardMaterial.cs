namespace Goodot15.Scripts.Game.Model;

public abstract class CardMaterial(string name, string textureAddress, int cost)
	: Card(name, textureAddress, true, cost) {
}