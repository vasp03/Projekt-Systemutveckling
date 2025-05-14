using System.Collections.Generic;

namespace Goodot15.Scripts.Game.Model.Div;

public class CardPack {
    public CardPack(string name, int cost, List<string> commonCards, List<string> rareCards) {
        Name = name;
        Cost = cost;
        CommonCards = commonCards;
        RareCards = rareCards;
    }

    public string Name { get; }
    public int Cost { get; }
    public List<string> CommonCards { get; }
    public List<string> RareCards { get; }
}