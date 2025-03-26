using Goodot15.Scripts.Game.Model.Interface;

public class MaterialJam(string name, string textureAddress, int cost)
	: CardMaterial(name, textureAddress, cost), IEdible
{
	public int FoodAmount { get; set; }
}