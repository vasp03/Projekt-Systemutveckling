using System.Collections.Generic;

public interface IStackable
{
    public abstract List<string> GetStackableItems();
    public abstract Card GetNeighbourAbove();
    public abstract Card GetNeighbourBelow();
    public abstract void SetNeighbourAbove(Card card);
    public abstract void SetNeighbourBelow(Card card);
}
