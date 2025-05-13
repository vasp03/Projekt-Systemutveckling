using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialFood(string textureAddress, int startFood, int cardValue)
    : CardMaterial(textureAddress, cardValue), IEdible {
    private int _remainingFood = startFood;

    public override int Value => 15;

    public virtual int RemainingFood {
        get => _remainingFood;
        set {
            if (value <= 0) CardNode.Destroy();
            _remainingFood = value;
        }
    }
}