using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class GameEventManager : GameManagerBase, ITickable {
    private readonly IDictionary<IGameEvent, int> eventTicks = new Dictionary<IGameEvent, int>();
    private readonly IList<IGameEvent> registedEvents = [];

    private Random random = new();

    public GameEventManager() {
        RegisterDefaultEvents();
    }

    public void PreTick() {
    }

    public void PostTick() {
        foreach (IGameEvent registeredEvent in registedEvents)
            if (registeredEvent.TicksUntilNextEvent <= eventTicks[registeredEvent]) {
                eventTicks[registeredEvent] = 0;
                if (registeredEvent.Chance >= GD.Randf()) {
                    GameEventContext gameEventContext = new(registeredEvent, GameController);
                    PostEvent(gameEventContext);
                }
            } else {
                eventTicks[registeredEvent]++;
            }
        // GD.Print("t");
        // GD.Print(registeredEvent.GetType().FullName + ": " + eventTicks[registeredEvent]);
    }

    public void RegisterDefaultEvents() {
        RegisterEvent(new MeteoriteEvent());
    }

    public void RegisterEvent(IGameEvent gameEvent) {
        eventTicks.TryAdd(gameEvent, 0);
        registedEvents.Add(gameEvent);
    }

    private void PostEvent(GameEventContext gameEventContext) {
        gameEventContext.GameEventFired.OnEvent(gameEventContext);
        // Fires the event to all cards as well for those cards that are listening to any game events
        GameController.GetManager<CardManager>().AllCards.ToList().ForEach(e => {
            if (e is IGameEventListener cardEventListener) cardEventListener.GameEventFired(gameEventContext);
        });
    }
}