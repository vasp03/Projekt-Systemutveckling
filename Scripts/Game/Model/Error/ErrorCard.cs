using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class ErrorCard() : Card("Error", true) {
    public override int Value => -1;

    public override bool CanStackWith(Card otherCard) {
        return false;
    }
}