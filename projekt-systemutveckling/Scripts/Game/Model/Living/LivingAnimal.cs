using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Interface;

public class LivingAnimal(string name, string textureAddress, bool movable, int cost, int health) : CardLiving(name, textureAddress, movable, cost, health), IStackable
{
    private int produceTimer;
    public int tickTimer()
    {
        return produceTimer--;
    }

    public IReadOnlyCollection<Type> GetStackableTypes()
    {
        throw new NotImplementedException();
    }

    public IStackable NeighbourAbove { get; set; }
    public IStackable NeighbourBelow { get; set; }
    public void SetNeighbourAbove(IStackable card)
    {
        throw new NotImplementedException();
    }

    public void SetNeighbourBelow(IStackable card)
    {
        throw new NotImplementedException();
    }
}