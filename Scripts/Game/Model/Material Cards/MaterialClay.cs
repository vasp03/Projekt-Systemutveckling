using System;
using System.Collections.Generic;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialClay(string textureAddress, int cost, CardNode cardNode)
    : CardMaterial(textureAddress, cost, cardNode) {
    public override IReadOnlyCollection<Type> GetStackableTypes() {
        throw new NotImplementedException();
    }
}