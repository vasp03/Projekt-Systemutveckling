using System.Collections.Generic;
using System.Linq;

namespace Goodot15.Scripts.Game.Model.Interface;

public interface IStackable {
	/// <summary>
	///     Determines if this may stack with the other object <paramref name="card"/>
	///		Defaults to true
	/// </summary>
	/// <param name="card">Other card to check if it can stack</param>
	/// <returns>True if capable of stacking, false otherwise</returns>
	public bool CanStackWith(Card card) {
		return true;
	}
}