using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialAxe() : CardMaterial("Axe"), IDurability {
    public override int Value => 100;
    public int Durability { get; private set; } = 10;

    public bool DecrementDurability() {
        Durability--;
        return Durability <= 0;
    }
}