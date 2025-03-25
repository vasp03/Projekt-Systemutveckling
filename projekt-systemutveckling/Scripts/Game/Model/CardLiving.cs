using System;

public partial class CardLiving(String name, String textureAddress, bool movable, int cost, int health) : Card(name, textureAddress, movable, cost)
{
    private int health = health;
}
