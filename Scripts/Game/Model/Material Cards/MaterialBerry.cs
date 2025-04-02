using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialBerry(string textureAddress, int cost, CardNode cardNode)
	: CardMaterial(textureAddress, cost, cardNode), IEdible {
	public int RemainingFood { get; set; }
}