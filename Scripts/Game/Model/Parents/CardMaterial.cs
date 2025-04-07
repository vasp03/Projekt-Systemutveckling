using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model;

public abstract class CardMaterial(string textureAddress, int cost, CardNode cardNode)
	: Card(textureAddress, true, cost, cardNode), IStackable {
	public IStackable NeighbourAbove { get; set; }
	public IStackable NeighbourBelow { get; set; }
}