using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model;

public abstract class CardMaterial(string textureAddress)
	: Card(textureAddress, true), IStackable {
	public IStackable NeighbourAbove { get; set; }
	public IStackable NeighbourBelow { get; set; }
}