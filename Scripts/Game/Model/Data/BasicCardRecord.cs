namespace Goodot15.Scripts.Game.Model.Data;

public class BasicCardRecord : CardRecord {
    public BasicCardRecord(Card card) {
        this.CardTypeName = card.GetType().FullName;
        this.CardPosition = card.CardNode.Position;
        this.ZIndex = card.CardNode.ZIndex;
    }
}