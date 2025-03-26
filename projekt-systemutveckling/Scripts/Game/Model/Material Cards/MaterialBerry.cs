using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialBerry(string name, string textureAddress, int cost)
	: CardMaterial(name, textureAddress, cost), IEdible {
	public int FoodAmount { get; set; }
}