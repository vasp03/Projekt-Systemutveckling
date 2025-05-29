using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialFood(string cardTextureName, int startFood)
    : CardMaterial(cardTextureName), IEdible {
    private int remainingFood = startFood;

    public override int Value => 15;

    public virtual int RemainingFood {
        get => remainingFood;
        set {
            if (value <= 0) CardNode.Destroy();
            remainingFood = value;
        }
    }
}