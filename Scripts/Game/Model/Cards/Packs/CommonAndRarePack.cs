using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

/// <summary>
///     Packs categorizes cards in two: common and rare. <see cref="CommonCards" /> and <see cref="RareCards" /> are pulled
///     from the collection when <see cref="GenerateCards" /> is invoked.
/// </summary>
public abstract class CommonAndRarePack : CardPack {
    /// <summary>
    ///     The chance for a Rare Card to spawn when picking a card. Value ranges from <b>0-1</b> (inclusive).<b />
    ///     Defaults to <b>0.1</b>.
    /// </summary>
    protected virtual float RareCardChance => 0.1f;

    /// <summary>
    ///     Available pool of common cards, will always be picked.
    /// </summary>
    protected abstract IReadOnlyList<Card> CommonCards { get; }

    /// <summary>
    ///     Available pool of rare cards, being picked is determined by <see cref="RareCardChance" />.
    /// </summary>
    protected abstract IReadOnlyList<Card> RareCards { get; }

    /// <summary>
    ///     The count of cards to spawn.<br />
    ///     Defaults to 3-5.
    /// </summary>
    protected virtual int CardCount => GD.RandRange(3, 5);

    public override IReadOnlyCollection<Card> GenerateCards() {
        IList<Card> collectedCardsInpack = [];
        for (int i = 0; i < CardCount; i++) {
            int randomIdxCommonCard = GD.RandRange(0, CommonCards.Count - 1);
            int randomIdxRareCard = GD.RandRange(0, RareCards.Count - 1);

            collectedCardsInpack.Add(CommonCards[randomIdxCommonCard]);
            if (GD.Randf() < RareCardChance) collectedCardsInpack.Add(RareCards[randomIdxRareCard]);
        }

        return collectedCardsInpack.AsReadOnly();
    }
}