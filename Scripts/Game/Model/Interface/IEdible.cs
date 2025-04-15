using Math = System.Math;

namespace Goodot15.Scripts.Game.Model.Interface;

public interface IEdible {
	public int RemainingFood { get; protected set; }

	/// <summary>
	/// </summary>
	/// <param name="foodAmount"></param>
	/// <returns></returns>
	public int ConsumeFood(int foodAmount) {
		int consumedAmount = Math.Min(foodAmount, RemainingFood);

		RemainingFood -= consumedAmount;

		return consumedAmount;
	}
}