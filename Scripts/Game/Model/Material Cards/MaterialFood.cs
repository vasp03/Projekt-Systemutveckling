using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialFood(string textureAddress, int cost, CardNode cardNode)
	: CardMaterial(textureAddress, cardNode), IEdible {
	private int _remainingFood;

	public int RemainingFood {
		get => _remainingFood;
		set {
			if (value <= 0) CardNode.QueueFree();
			_remainingFood = value;
		}
	}
}