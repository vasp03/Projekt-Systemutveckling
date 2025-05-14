namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialAxe() : CardMaterial("Axe"), IDurability {
    public int Durability { get; private set; } = 10;
    public override int Value => 45;

    public bool DecrementDurability() {
        Durability--;
        return Durability <= 0;
    }
}