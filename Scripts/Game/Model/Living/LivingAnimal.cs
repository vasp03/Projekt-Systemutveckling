using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Interface;

public class LivingAnimal(string textureAddress, bool movable, int cost, int health, CardNode cardNode)
	: CardLiving(textureAddress, movable, cost, health, cardNode), IStackable, ITickable {
	private int produceTimer;

	public void preTick() {
		tickTimer();
	}

	public void postTick() {
	}

	public int tickTimer() {
		return produceTimer--;
	}
}