using Godot;
using System;

public partial class LivingEnemy(string textureAddress, bool movable, int cost, int health) : CardLiving(textureAddress, movable, cost, health)
{
    private int attackDamage;


    public int GetAttackDamage()
    {
        return attackDamage;
    }

    public void SetAttackDamage(int value)
    {
        attackDamage = value;
    }
}
