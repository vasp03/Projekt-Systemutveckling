using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Interface;
using System.Linq;
using Goodot15.Scripts;

public abstract class Card {

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
        TexturePath = Global.GetTexturePath(textureAddress);
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
}