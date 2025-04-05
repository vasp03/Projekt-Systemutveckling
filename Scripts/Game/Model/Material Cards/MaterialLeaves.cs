using System;
using System.Collections.Generic;
using Goodot15.Scripts.Game.Model;

public class MaterialLeaves(string textureAddress, int cost, CardNode cardNode) : CardMaterial(textureAddress, cost, cardNode) {
	public override IReadOnlyCollection<Type> GetStackableTypes() {
		throw new NotImplementedException();
	}
}