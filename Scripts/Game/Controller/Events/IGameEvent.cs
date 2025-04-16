namespace Goodot15.Scripts.Game.Controller.Events;

public interface IGameEvent {
    public string EventName { get; }
    public int TicksUntilNextEvent { get; }
    public double Chance { get; }

    public void OnEvent(GameEventContext context);
}