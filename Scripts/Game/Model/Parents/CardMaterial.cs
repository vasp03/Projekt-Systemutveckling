using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model;

public abstract class CardMaterial(string textureAddress, int cardValue) : Card(textureAddress, true, cardValue), IStackable {
    public IStackable NeighbourAbove { get; set; }
    public IStackable NeighbourBelow { get; set; }
}