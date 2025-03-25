using System.Collections.Generic;

public partial class LivingAnimal(string textureAddress, bool movable, int cost, int health) : CardLiving(textureAddress, movable, cost, health), IStackable
{
    private List<string> stackableItems;
    private Card neighbourAbove;
    private Card neighbourBelow;
    private int produceTimer;

    


    public List<string> getStackableItems()
    {
        return stackableItems;
    }

    public Card getNeighbourAbove()
    {
        return neighbourAbove;
    }

    public Card getNeighbourBelow()
    {
        return neighbourBelow;
    }

    public void setNeighbourAbove(Card card)
    {
        neighbourAbove = card;
    }

    public void setNeighbourBelow(Card card)
    {
        neighbourBelow = card;
    }

    public int GetProduceTimer()
    {
        return produceTimer;
    }

    public int tickTimer()
    {
        return produceTimer--;
    }

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
        this.neighbourAbove = card;
    }

    public void SetNeighbourBelow(Card card)
    {
        this.neighbourBelow = card;
    }

    public bool CanStackWith(Card card)
    {
        return stackableItems.Contains(card.GetName());
    }
}
