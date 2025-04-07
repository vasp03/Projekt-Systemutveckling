using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialBerry()
	: CardMaterial("Berry"), IEdible {
	public int RemainingFood { get; set; }
}