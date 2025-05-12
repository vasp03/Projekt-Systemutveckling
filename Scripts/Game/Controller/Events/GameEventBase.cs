namespace Goodot15.Scripts.Game.Controller.Events;

public abstract class GameEventBase : IGameEvent {
    public abstract string EventName { get; }
    public abstract int TicksUntilNextEvent { get; }
    public abstract double Chance { get; }
    public abstract void OnEvent(GameEventContext context);
}