namespace Goodot15.Scripts.Game.Controller.Events;

/// <summary>
///     <inheritdoc cref="IGameEvent" /><br />
/// </summary>
public abstract class GameEvent : IGameEvent {
    public abstract string EventName { get; }
    public abstract int TicksUntilNextEvent { get; }
    public abstract double Chance { get; }
    public abstract void OnEvent(GameEventContext context);
}