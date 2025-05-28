namespace Goodot15.Scripts.Game.Model.Interface;

/// <summary>
///     Represents a callback that can listen for pause or unpause events
/// </summary>
public interface IPausable {
    public void SetPaused(bool isPaused);
}