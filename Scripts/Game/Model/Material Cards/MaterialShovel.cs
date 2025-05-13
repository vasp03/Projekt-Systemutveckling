namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialShovel() : CardMaterial("Shovel", 8), IDurability {
    public int Durability { get; private set; } = 10;
    public override int Value => 55;

    public bool DecrementDurability() {
        Durability--;
        return Durability <= 0;
    }
}