using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Div;

/// <summary>
///     Base class used for Card Packs
/// </summary>
public abstract class CardPack {
    /// <summary>
    ///     Full texture address to the Pack button. <see cref="PackTexture" /> can be used to obtain the full path by name
    /// </summary>
    public abstract string PackButtonTexture { get; }

    /// <summary>
    ///     Determines if the pack is Single-time use only.
    /// </summary>
    public abstract bool SingleUse { get; }

    /// <summary>
    ///     Flag if the Card Pack has been used or not, used together with <see cref="SingleUse" />
    /// </summary>
    public bool Consumed { get; set; }

    /// <summary>
    ///     Name of Card Pack
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    ///     Cost of card pack in coins
    /// </summary>
    public abstract int Cost { get; }

    /// <summary>
    ///     Provides a full path to an card pack texture asset provided by name
    /// </summary>
    /// <param name="packName">Name of card pack</param>
    /// <returns>Full path for pack texture asset</returns>
    protected static string PackTexture(string packName) {
        return $"res://Assets/Packs/{packName.Replace(" ", "_")}.png";
    }

    /// <summary>
    ///     Provides a collection of randomized cards.
    /// </summary>
    /// <returns></returns>
    public abstract IReadOnlyCollection<Card> GenerateCards();
}