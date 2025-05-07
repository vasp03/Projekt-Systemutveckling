namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialFishingPole() : CardMaterial("FishingPole", 5), IDurability {
    public int Durability { get; private set; } = 10;

    public bool DecrementDurability() {
        Durability--;
        return Durability <= 0;
    }
}