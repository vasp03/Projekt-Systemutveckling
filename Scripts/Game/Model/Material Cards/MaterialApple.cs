using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialApple(string textureAddress, int cost, CardNode cardNode) : CardMaterial(textureAddress, cost, cardNode), IEdible {
	public int FoodAmount { get; set; }

	public override IReadOnlyCollection<Type> GetStackableTypes() {
		throw new NotImplementedException();
	}
}