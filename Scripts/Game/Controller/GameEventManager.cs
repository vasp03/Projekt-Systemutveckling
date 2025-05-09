using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class GameEventManager : GameManagerBase, ITickable {
    private readonly IDictionary<IGameEvent, int> eventTicks = new Dictionary<IGameEvent, int>();
    private readonly IList<IGameEvent> registeredEvents = [];
    private readonly IList<IGameEventListener> registeredEventListeners = [];

    public GameEventManager(GameController gameController) : base(gameController) {
        RegisterDefaultEvents();
    }

    public void PreTick() {
    }

    public void PostTick() {
        foreach (IGameEvent registeredEvent in registeredEvents)
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
    }

    public void RegisterEvent(IGameEvent gameEvent) {
        eventTicks.TryAdd(gameEvent, 0);
        registeredEvents.Add(gameEvent);
    }

    public void RegisterGlobalEventListener(IGameEventListener gameEventListener) {
        registeredEventListeners.Add(gameEventListener);
    }
    
    private void PostEvent(GameEventContext gameEventContext) {
        gameEventContext.GameEventFired.OnEvent(gameEventContext);
        foreach (IGameEventListener registeredEventListener in registeredEventListeners)
        {
            registeredEventListener.GameEventFired(gameEventContext);
        }
        // Fires the event to all cards as well for those cards that are listening to any game events
        GameController.GetCardController().AllCards.ToList().ForEach(e => {
            if (e is IGameEventListener cardEventListener) cardEventListener.GameEventFired(gameEventContext);
        });
    }
}