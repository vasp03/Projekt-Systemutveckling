namespace Goodot15.Scripts.Game.Controller.Events;

/// <summary>
///     Any events will implement this interface; Includes basic properties that an event has to expose.
/// </summary>
public interface IGameEvent {
    /// <summary>
    ///     The name of the event. May be used for displaying the event in game through text or similar
    /// </summary>
    public string EventName { get; }

    /// <summary>
    ///     How many ticks must pass before the event can happen again.<br />
    ///     Returning -1 will make this event only able to be posted manually to the event bus through
    ///     <see cref="GameEventManager.PostEvent(Goodot15.Scripts.Game.Controller.Events.IGameEvent)" />
    /// </summary>
    public int TicksUntilNextEvent { get; }

    /// <summary>
    ///     Determines if the event should actually be ticking and be executed.<br />
    ///     If <see cref="TicksUntilNextEvent" /> is -1; This will automatically be <b>false</b>
    /// </summary>
    public virtual bool EventActive => TicksUntilNextEvent >= 0;

    /// <summary>
    ///     The chance for the event to actually trigger, executed every <see cref="TicksUntilNextEvent" /> ticks.<br />
    ///     The value ranges from <b>0-1</b> inclusive. 1 meaning 100% and 0 meaning 0%
    ///     <remarks>When manually posting it to the <see cref="GameEventManager" /> event bus; This property is ignored.</remarks>
    /// </summary>
    public double Chance { get; }

    /// <summary>
    ///     Called when the event is triggered; Happens either automatically by the <see cref="GameEventManager" /> or manually
    ///     when posted through the eventbus with
    ///     <see cref="GameEventManager.PostEvent(Goodot15.Scripts.Game.Controller.Events.IGameEvent)" />
    /// </summary>
    /// <param name="context">Context that the event was fired in; Supplies basic parameters</param>
    public void OnEvent(GameEventContext context);
}