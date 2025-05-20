namespace Goodot15.Scripts.Game.Model.Interface;

public interface ITickable {
    /// <summary>
    ///     Invoked before any logic has been executed on an instance.
    /// </summary>
    /// <param name="delta"></param>
    public void PreTick(double delta) {
    }

    /// <summary>
    ///     Invoked after all logic has been executed on an instance
    /// </summary>
    /// <param name="delta"></param>
    public void PostTick(double delta) {
    }
}