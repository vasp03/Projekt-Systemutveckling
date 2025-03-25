using System;

public partial class Card
{
    private String name;
    private String textureAddress;
    private bool movable;
    private int cost;
    private bool highlighted { get; set; }
    private String textureFolder = "res://Assets/Cards/Ready To Use/";
    private String textureEnding = ".png";

    /// <summary>
    /// Constructor for the Card class
    /// Texture address should be the address after "res://Assets/Cards/Ready To Use/".
    /// </summary>
    /// <param name="name"></param>
    /// <param name="textureAddress"></param> 
    /// <param name="movable"></param>
    /// <param name="cost"></param>
    public Card(String name, String textureAddress, bool movable, int cost)
    {
        this.name = name;
        this.textureAddress = textureFolder+textureAddress+textureEnding;
        this.movable = movable;
        this.cost = cost;
    }

    public String GetName()
    {
        return name;
    }

    public String GetTextureAddress()
    {
        return textureAddress;
    }

    public bool IsMovable()
    {
        return movable;
    }

    public int GetCost()
    {
        return cost;
    }
}
