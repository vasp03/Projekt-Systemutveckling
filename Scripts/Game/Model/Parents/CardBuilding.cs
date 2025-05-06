using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Parents;

public abstract class CardBuilding : Card {
    protected CardBuilding(string textureAddress, bool movable, int cardValue) : base(textureAddress, movable, cardValue) {
    }
}