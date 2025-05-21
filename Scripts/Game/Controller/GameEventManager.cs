using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Responsible for handling game events, ticking logic and timing.<br />
///     Acts as a register for events; Events must always be registered so they can be ticked and executed through
///     <see cref="RegisterEvent" />.
/// </summary>
public class GameEventManager : GameManagerBase, ITickable {
    /// <summary>
    ///     Keeps tracks of event instances timing ticks
    /// </summary>
    private readonly IDictionary<IGameEvent, int> eventTicks = new Dictionary<IGameEvent, int>();

    /// <summary>
    ///     Keeps track of registered events
    /// </summary>
    private readonly IList<IGameEvent> registeredEvents = [];

    /// <summary>
    ///     Constructs a new Game Event Manager
    /// </summary>
    /// <param name="gameController">Game Controller instance to be used</param>
    public GameEventManager(GameController gameController) : base(gameController) {
        RegisterDefaultEvents();
    }

    /// <summary>
    ///     Executes Pre ticking logic, this includes for every event.
    /// </summary>
    public void PreTick() {
        foreach (IGameEvent gameEvent in registeredEvents) {
            ITickable? tickableGameEvent = gameEvent as ITickable;
            tickableGameEvent?.PreTick();
        }
    }

    /// <summary>
    ///     Executes Post ticking logic, this includes incrementing tick counters for timing the events.
    /// </summary>
    public void PostTick() {
        foreach (IGameEvent registeredEvent in registeredEvents) {
            ITickable? tickableGameEvent = registeredEvent as ITickable;
            if (registeredEvent.EventActive && registeredEvent.TicksUntilNextEvent <= eventTicks[registeredEvent]) {
                eventTicks[registeredEvent] = 0;
                if (registeredEvent.Chance >= GD.Randf()) PostEvent(registeredEvent);
            } else {
                eventTicks[registeredEvent]++;
            }

            tickableGameEvent?.PostTick();
        }
    }

    /// <summary>
    ///     Registers the default events for the game, called from the constructor
    /// </summary>
    private void RegisterDefaultEvents() {
        RegisterEvent(new MeteoriteEvent());
        RegisterEvent(new NatureResourceEvent());
        RegisterEvent(new FireEvent());
        RegisterEvent(new DayTimeEvent());
        RegisterEvent(new ColdNightEvent());
    }

    /// <summary>
    ///     Registers any new <see cref="IGameEvent" /> event instances.
    /// </summary>
    /// <param name="gameEvent">Game Event Instance to be registered</param>
    /// <remarks>
    ///     Instance pass may implement the interface <see cref="ITickable" /> to allow an event to execute logic each
    ///     tick
    /// </remarks>
    public void RegisterEvent(IGameEvent gameEvent) {
        if (gameEvent.TicksUntilNextEvent >= 0) eventTicks.TryAdd(gameEvent, 0);

        registeredEvents.Add(gameEvent);
    }

    /// <summary>
    ///     Gets an already registered event instance from the Game Event Manager
    /// </summary>
    /// <typeparam name="T">The Event instance type</typeparam>
    /// <returns>Event instance registered, null if not registered</returns>
    public T? EventInstance<T>() where T : class, IGameEvent {
        return registeredEvents.FirstOrDefault(e => e.GetType() == typeof(T)) as T;
    }

    /// <summary>
    ///     Posts an event to the game system
    /// </summary>
    /// <param name="gameEvent">Event instance to be posted</param>
    public void PostEvent(IGameEvent gameEvent) {
        PostEvent(new GameEventContext(gameEvent, GameController));
    }

    /// <summary>
    ///     Posts an event to the game system
    /// </summary>
    /// <param name="gameEventContext">Game event context posted</param>
    public void PostEvent(GameEventContext gameEventContext) {
        gameEventContext.GameEventFired.OnEvent(gameEventContext);
        // Fires the event to all cards as well for those cards that are listening to any game events
        GameController.CardController.AllCards.ToList().ForEach(e => {
            if (e is IGameEventListener cardEventListener) cardEventListener.GameEventFired(gameEventContext);
        });
    }
}