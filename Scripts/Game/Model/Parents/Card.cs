using System.Linq;

namespace Goodot15.Scripts.Game.Model.Parents;

/// <summary>
///     Base class for any card-data
/// </summary>
public abstract class Card {
    private const string BASE_TEXTURE_PATH = "res://Assets/Cards/Ready To Use/";
    private const string TEXTURE_ENDING = ".png";

    /// <summary>
    ///     Instantiates a new Card class<br />
    ///     Texture address should be the address after "res://Assets/Cards/Ready To Use/".
    /// </summary>
    /// <param name="cardTextureName">Path of Card texture</param>
    /// <param name="movable">Determines if this card is movable/interactable</param>
    public Card(string cardTextureName, bool movable) {
        // Generate a unique uuid as name
        TexturePath = BASE_TEXTURE_PATH + cardTextureName + TEXTURE_ENDING;
        Movable = movable;
    }

    /// <summary>
    ///     Attached CardNode reference
    /// </summary>
    public CardNode CardNode { get; set; }


    /// <summary>
    ///     Tests if this Card may stack with the <see cref="cardBelow" /> that is below this card
    /// </summary>
    /// <param name="cardBelow">Card below this card</param>
    /// <returns>True if capable of stacking, false will deny it</returns>
    public virtual bool CanStackBelow(Card cardBelow) {
        return true;
    }

    /// <summary>
    ///     Tests if this Card may stack with the <see cref="cardAbove" /> that is above this card
    /// </summary>
    /// <param name="cardAbove">Card above this card</param>
    /// <returns>True if capable of stacking, false will deny it</returns>
    public virtual bool CanStackAbove(Card cardAbove) {
        return true;
    }


    #region Card data

    /// <summary>
    ///     Determines the value of the card. -1 implies not sellable.
    /// </summary>
    public abstract int Value { get; }

    /// <summary>
    ///     Determines if the card can be moved or not
    /// </summary>
    public bool Movable { get; private set; }

    /// <summary>
    ///     The full asset texture path for this given card instance.
    /// </summary>
    public string TexturePath { get; }

    public string CardName {
        get {
            string[] split = TexturePath.Split("/");
            string textureType = split.Last();
            textureType = textureType.Substring(0, textureType.Length - 4);
            return textureType;
        }
    }

    public virtual bool AlwaysOnTop => false;

    #endregion
}