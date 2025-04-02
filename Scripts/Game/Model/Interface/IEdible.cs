using System;

namespace Goodot15.Scripts.Game.Model.Interface;

public interface IEdible {
	public int RemainingFood { get; protected set; }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="foodAmount"></param>
	/// <returns></returns>
	public int ConsumeFood(int foodAmount) {
		int clampedRemainingFood = Math.Max(RemainingFood - foodAmount, 0);
		int consumedFood = RemainingFood - clampedRemainingFood;
		
		
		this.RemainingFood -= consumedFood;
		
		return consumedFood;
	}
}