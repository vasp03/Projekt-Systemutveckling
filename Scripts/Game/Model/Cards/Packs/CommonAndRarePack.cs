using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public abstract class CommonAndRarePack : CardPack {
    private const float RARE_CARD_CHANCE = 0.1f;
    protected abstract IReadOnlyList<Card> CommonCards { get; }
    protected abstract IReadOnlyList<Card> RareCards { get; }

    protected abstract int CardCount { get; }

    public override IReadOnlyCollection<Card> GenerateCards() {
        IList<Card> collectedCardsInpack = [];
        for (int i = 0; i < CardCount; i++) {
            int randomIdxCommonCard = GD.RandRange(0, CommonCards.Count - 1);
            int randomIdxRareCard = GD.RandRange(0, RareCards.Count - 1);

            collectedCardsInpack.Add(CommonCards[randomIdxCommonCard]);
            if (GD.Randf() < RARE_CARD_CHANCE) {
                collectedCardsInpack.Add(RareCards[randomIdxRareCard]);
            }
        }

        return collectedCardsInpack.AsReadOnly();
    }
}