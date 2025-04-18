using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Controller.Events;

namespace Goodot15.Scripts.Game.Model.Interface;

/// <summary>
/// Allows for cards to listen for new game events that occur.
/// </summary>
public interface IGameEventListener {
    /// <summary>
    /// Triggered when a game event occurs, fired by <see cref="GameEventManager"/>
    /// </summary>
    /// <param name="gameEventContext">Game Event being fired</param>
    public void GameEventFired(GameEventContext gameEventContext);
}