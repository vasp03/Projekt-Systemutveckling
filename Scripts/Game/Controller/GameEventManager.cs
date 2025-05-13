using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class GameEventManager : GameManagerBase, ITickable {
    private readonly IDictionary<IGameEvent, int> eventTicks = new Dictionary<IGameEvent, int>();
    private readonly IList<IGameEvent> RegisteredEvents = [];

    public GameEventManager(GameController gameController) : base(gameController) {
        RegisterDefaultEvents();
    }

    public void PreTick() {
    }

    public void PostTick() {
        foreach (IGameEvent registeredEvent in RegisteredEvents)
            if (registeredEvent.TicksUntilNextEvent <= eventTicks[registeredEvent]) {
                eventTicks[registeredEvent] = 0;
                if (registeredEvent.Chance >= GD.Randf()) {
                    GameEventContext gameEventContext = new(registeredEvent, GameController);
                    PostEvent(gameEventContext);
                }
            } else {
                eventTicks[registeredEvent]++;
            }
    }

    public void RegisterDefaultEvents() {
        RegisterEvent(new MeteoriteEvent());
        RegisterEvent(new NatureResourceEvent());
        RegisterEvent(new FireEvent());
        RegisterEvent(new DayTimeEvent(GameController));
    }

    public void RegisterEvent(IGameEvent gameEvent) {
        eventTicks.TryAdd(gameEvent, 0);
        RegisteredEvents.Add(gameEvent);
    }

    /// <summary>
    ///     Gets a registered event instance from the Game Event Manager
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Event instance registered, null if not registered</returns>
    public T? EventInstance<T>() where T : IGameEvent {
        return (T)RegisteredEvents.FirstOrDefault(e => e.GetType() == typeof(T));
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