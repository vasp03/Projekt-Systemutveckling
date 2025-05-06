using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Interface;
using System.Linq;

public abstract class Card {
    private const string BaseTexturePath = "res://Assets/Cards/Ready To Use/";
    private const string TextureEnding = ".png";
    public readonly int CardValue;
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
    ///     Constructor for the Card class
    ///     Texture address should be the address after "res://Assets/Cards/Ready To Use/".
    /// </summary>
    /// <param name="textureAddress"></param>
    /// <param name="movable"></param>
    /// <param name="name"></param>
    public Card(string textureAddress, bool movable, int cardValue) {
        // Generate a unique uuid as name
        ID = Guid.NewGuid().ToString();
        TexturePath = BaseTexturePath + textureAddress + TextureEnding;
        Movable = movable;
        CardValue = cardValue;
    }
}