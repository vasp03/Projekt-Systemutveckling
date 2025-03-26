using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialJam(string textureAddress, int cost)
	: CardMaterial(textureAddress, cost), IEdible {
	public int FoodAmount { get; set; }
}