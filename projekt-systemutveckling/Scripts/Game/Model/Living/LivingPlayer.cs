using System.Collections.Generic;

public partial class LivingPlayer(string name, string textureAddress, bool movable, int cost, int health) : CardLiving(name, textureAddress, movable, cost, health), IStackable
{
    private List<string> stackableItems;
    private Card neighbourAbove;
    private Card neighbourBelow;
    private int saturation;
    private int attackDamage;

    public List<string> GetStackableItems()
    {
        return stackableItems;
    }

    public Card GetNeighbourAbove()
    {
        return neighbourAbove;
    }

    public Card GetNeighbourBelow()
    {
        return neighbourBelow;
    }

    public void SetNeighbourAbove(Card card)
    {
        neighbourAbove = card;
    }

    public void SetNeighbourBelow(Card card)
    {
        neighbourBelow = card;
    }


    public int GetSaturation()
    {
        return saturation;
    }

    public void SetSaturation(int value)
    {
        saturation = value;
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }

    public void SetAttackDamage(int value)
    {
        attackDamage = value;
    }
}
