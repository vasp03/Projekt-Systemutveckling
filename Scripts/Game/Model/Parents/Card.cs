using System;
using System.Linq;

namespace Goodot15.Scripts.Game.Model.Parents;

public abstract class Card {
    private const string baseTexturePath = "res://Assets/Cards/Ready To Use/";
    private const string textureEnding = ".png";

    /// <summary>
    ///     Constructor for the Card class
    ///     Texture address should be the address after "res://Assets/Cards/Ready To Use/".
    /// </summary>
    /// <param name="textureAddress"></param>
    /// <param name="movable"></param>
    /// <param name="name"></param>
    public Card(string textureAddress, bool movable) {
        // Generate a unique uuid as name
        ID = Guid.NewGuid().ToString();
        TexturePath = baseTexturePath + textureAddress + textureEnding;
        Movable = movable;
    }

    public abstract int Value { get; }
    public string ID { get; private set; }
    public string TexturePath { get; }
    public CardNode CardNode { get; set; }
    public bool Movable { get; set; }

    public string TextureType {
        get {
            string[] split = TexturePath.Split("/");
            string textureType = split.Last();
            textureType = textureType.Substring(0, textureType.Length - 4);
            return textureType;
        }
    }

    /// <summary>
    ///     Determines if this Card may stack with the <see cref="cardBelow" /> that is below this card
    /// </summary>
    /// <param name="cardBelow">CX</param>
    /// <returns>True will allow stacking, false will deny it</returns>
    public virtual bool CanStackBelow(Card cardBelow) {
        return true;
    }

    /// <summary>
    ///     Determines if this Card may stack with the <see cref="cardAbove" /> that is above this card
    /// </summary>
    /// <param name="cardAbove"></param>
    /// <returns></returns>
    public virtual bool CanStackAbove(Card cardAbove) {
        return true;
    }
}