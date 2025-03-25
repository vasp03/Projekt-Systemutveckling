using System;

public partial class Card
{
    private String name;
    private String textureAddress;
    private bool movable;
    private int cost;
    private bool highlighted { get; set; }

    public Card(String name, String textureAddress, bool movable, int cost)
    {
        this.name = name;
        this.textureAddress = textureAddress;
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
