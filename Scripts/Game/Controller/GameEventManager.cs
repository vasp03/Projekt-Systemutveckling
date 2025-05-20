using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class GameEventManager : GameManagerBase, ITickable {
    /// <summary>
    /// Keeps tracks of event instances ticks
    /// </summary>
    private readonly IDictionary<IGameEvent, int> eventTicks = new Dictionary<IGameEvent, int>();
    
    /// <summary>
    /// Keeps track of registered events
    /// </summary>
    private readonly IList<IGameEvent> registeredEvents = [];
    
    public GameEventManager(GameController gameController) : base(gameController) {
        RegisterDefaultEvents();
    }

    public void PreTick(double delta) {
    }

    public void PostTick(double delta) {
        foreach (IGameEvent registeredEvent in registeredEvents)
            if (registeredEvent.TicksUntilNextEvent <= eventTicks[registeredEvent]) {
                eventTicks[registeredEvent] = 0;
                if (registeredEvent.Chance >= GD.Randf()) {
                    PostEvent(registeredEvent);
                }
            } else {
                eventTicks[registeredEvent]++;
            }
    }

    /// <summary>
    /// Registers the default events for the game, called from the constructor
    /// </summary>
    private void RegisterDefaultEvents() {
        RegisterEvent(new MeteoriteEvent());
        RegisterEvent(new NatureResourceEvent());
        RegisterEvent(new FireEvent());
        RegisterEvent(new DayTimeEvent(GameController));
        RegisterEvent(new BoulderEvent());
    }

    /// <summary>
    /// Registers any new <see cref="IGameEvent"/> event instances.
    /// </summary>
    /// <param name="gameEvent">Game Event Instance to be registered</param>
    public void RegisterEvent(IGameEvent gameEvent) {
        eventTicks.TryAdd(gameEvent, 0);
        registeredEvents.Add(gameEvent);
    }

    /// <summary>
    ///     Gets a registered event instance from the Game Event Manager
    /// </summary>
    /// <typeparam name="T">The Event instance type</typeparam>
    /// <returns>Event instance registered, null if not registered</returns>
    public T? EventInstance<T>() where T : IGameEvent {
        return (T)registeredEvents.FirstOrDefault(e => e.GetType() == typeof(T));
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