using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialBerry(int cost, CardNode cardNode)
	: CardMaterial("Berry", cardNode), IEdible {
	public int RemainingFood { get; set; }
}