namespace Goodot15.Scripts.Game.Model.Interface;

public interface IDurability {
    public int Durability { get; }
    public bool DecrementDurability();
}