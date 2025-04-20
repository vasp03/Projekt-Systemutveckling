using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Interface;
using System.Linq;

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

    public CardNode CardNode { get; set; }

    public string ID { get; private set; }
    public string TexturePath { get; }

    public bool Movable { get; set; }

    public string TextureType {
        get {
            string[] split = TexturePath.Split("/");
            string textureType = split.Last();
            textureType = textureType.Substring(0, textureType.Length - 4);
            return textureType;
        }
    }

    public static explicit operator Card(List<IStackable> v) {
        throw new NotImplementedException();
    }

}