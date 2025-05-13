namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class ErrorCard : Card {
    public ErrorCard() : base("Error", true) {
    }

    protected override int SetValue() {
        return -1;
    }
}