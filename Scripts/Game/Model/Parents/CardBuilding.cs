using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Parents;

public abstract class CardBuilding : Card, IStackable {
    protected CardBuilding(string textureAddress, bool movable, int cardValue) : base(textureAddress, movable,
        cardValue) {
    }

    public IStackable NeighbourAbove { get; set; }
    public IStackable NeighbourBelow { get; set; }
}