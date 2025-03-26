using System;

public abstract class CardLiving(String name, String textureAddress, bool movable, int cost, int health) : Card(name, textureAddress, movable, cost)
{
    public int Health { get; protected set; } = health;
}
