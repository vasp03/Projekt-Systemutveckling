namespace Goodot15.Scripts.Game.Model.Interface;

public interface ITickable {
	/// <summary>
	///     Invoked before any logic has been executed on an instance.
	/// </summary>
	public void preTick() {
	}

	/// <summary>
	///     Invoked after all logic has been executed on an instance
	/// </summary>
	public void postTick() {
	}
}