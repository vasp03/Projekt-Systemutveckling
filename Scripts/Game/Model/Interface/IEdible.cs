using Goodot15.Scripts.Game.Model.Parents;
using Math = System.Math;

namespace Goodot15.Scripts.Game.Model.Interface;

/// <summary>
///     <see cref="Card" />s implementing this interface are considered edible
/// </summary>
public interface IEdible {
    /// <summary>
    ///     Remaining food value in the card
    /// </summary>
    public int RemainingFood { get; protected set; }

    /// <summary>
    ///     Attempts to consume the supplied <see cref="foodAmount" />.
    /// </summary>
    /// <param name="foodAmount">Food amount to be consumed</param>
    /// <returns>
    ///     Either <see cref="foodAmount" /> or <see cref="RemainingFood" /> if <see cref="foodAmount" />&gt;
    ///     <see cref="RemainingFood" />
    /// </returns>
    public int ConsumeFood(int foodAmount) {
        int consumedAmount = Math.Min(foodAmount, RemainingFood);

        RemainingFood -= consumedAmount;

        return consumedAmount;
    }
}