using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game;

public interface ICardConsumer {
    /// <summary>
    ///     Triggered if a card above is placed on another card.
    /// </summary>
    /// <param name="otherCard">Other card that is above <b>this</b> card</param>
    /// <returns>True will delete the other card that is put on <b>this</b> card</returns>
    public bool ConsumeCard(Card otherCard);
}