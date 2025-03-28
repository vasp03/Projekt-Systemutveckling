using System;
using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model;

public abstract class CardMaterial(string textureAddress, int cost, CardNode cardNode) : Card(textureAddress, true, cost, cardNode), IStackable {
    public IStackable NeighbourAbove { get; set; }
    public IStackable NeighbourBelow { get; set; }

    public IReadOnlyCollection<Type> GetStackableTypes() {
        throw new NotImplementedException();
    }

    public void SetNeighbourAbove(IStackable card) {
        NeighbourAbove = card;
        return;
    }

    public void SetNeighbourBelow(IStackable card) {
        NeighbourBelow = card;
        return;
    }
}