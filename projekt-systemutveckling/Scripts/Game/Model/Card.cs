using System;

public class Card
{
    /// <summary>
    /// Name of the card type.
    /// </summary>
    public String Name { get; private set; }

    /// <summary>
    /// Location for Texture Address
    /// </summary>
    public String TextureAddress { get; protected set; }

    /// <summary>
    /// Determines if the Card can be moved.
    /// </summary>
    public bool Movable { get; protected set; }
    public int Cost { get; protected set; }

    public Card(String name, String textureAddress, bool movable, int cost)
    {
        this.Name = name;
        this.TextureAddress = textureAddress;
        this.Movable = movable;
        this.Cost = cost;
    }

    // public String GetName()
    // {
    //     return Name;
    // }
// 
    // public String GetTextureAddress()
    // {
    //     return TextureAddress;
    // }

    // public bool IsMovable()
    // {
    //     return movable;
    // }

    // public int GetCost()
    // {
    //     return cost;
    // }
}
