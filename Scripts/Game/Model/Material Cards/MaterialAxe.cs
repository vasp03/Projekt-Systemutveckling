namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialAxe() : CardMaterial("Axe", 5), IDurability {
    private int _durabilty = 10;
    public int Durability => _durabilty;

    public bool DecrementDurability() {
        _durabilty--;
        return _durabilty <= 0;
    }
}