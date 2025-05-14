using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialShovel() : CardMaterial("Shovel"), IDurability {
    public override int Value => 55;
    public int Durability { get; private set; } = 10;

    public bool DecrementDurability() {
        Durability--;
        return Durability <= 0;
    }
}