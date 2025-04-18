namespace Goodot15.Scripts.Game.Controller.Events;

public interface IGameEvent {
    /// <summary>
    /// The name of the event. May be used for displaying the event in game through text or similar
    /// </summary>
    public string EventName { get; }
    
    /// <summary>
    /// How many ticks must pass before the event can happen again.
    /// </summary>
    public int TicksUntilNextEvent { get; }
    public double Chance { get; }

    public void OnEvent(GameEventContext context);
}