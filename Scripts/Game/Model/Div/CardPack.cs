using System.Collections.Generic;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Model.Div;

/// <summary>
///     A pack, used with <see cref="PackController" />.
/// </summary>
public record CardPack {
    /// <summary>
    ///     Constructs a new CardPack instance
    /// </summary>
    /// <param name="name">Name of card pack</param>
    /// <param name="cost">Cost in coins</param>
    /// <param name="commonCards">Common dropped cards</param>
    /// <param name="rareCards">Rarely dropped cards</param>
    public CardPack(string name, int cost, IReadOnlyList<string> commonCards, IReadOnlyList<string> rareCards) {
        Name = name;
        Cost = cost;
        CommonCards = commonCards;
        RareCards = rareCards;
    }

    /// <summary>
    ///     Name of card pack
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Cost of card pack in coins
    /// </summary>
    public int Cost { get; }

    /// <summary>
    ///     Common dropped cards
    /// </summary>
    public IReadOnlyList<string> CommonCards { get; }

    /// <summary>
    ///     Rarely dropped cards
    /// </summary>
    public IReadOnlyList<string> RareCards { get; }
}