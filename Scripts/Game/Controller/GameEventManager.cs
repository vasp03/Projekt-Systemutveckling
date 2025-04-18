using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Interface;
using Godot;
using System;

using System.Collections.Generic;
using Goodot15.Scripts.Game.Controller.Events;


namespace Goodot15.Scripts.Game.Controller;
public partial class GameEventManager : GameManagerBase, ITickable {
    private IList<IGameEvent> registedEvents = [];
    private IDictionary<IGameEvent, int> eventTicks = new Dictionary<IGameEvent, int>();
    
    private Random random = new();

    public GameEventManager(GameController gameController) : base(gameController) {
        this.RegisterDefaultEvents();
    }

    public void RegisterDefaultEvents() {
        RegisterEvent(new MeteoriteEvent());
    }

    public void RegisterEvent(IGameEvent gameEvent) {
        eventTicks.TryAdd(gameEvent, 0);
        registedEvents.Add(gameEvent);
    }
    public void PreTick() {
    }

    public void PostTick() {
        foreach (IGameEvent registeredEvent in registedEvents) {
            if (registeredEvent.TicksUntilNextEvent <= eventTicks[registeredEvent]) {
                eventTicks[registeredEvent] = 0;
                if (registeredEvent.Chance >= GD.Randf()) {
                    registeredEvent.OnEvent(new GameEventContext(registeredEvent, CoreGameController));
                }
            } else {
                eventTicks[registeredEvent]++;
            }
            GD.Print(registeredEvent.GetType().FullName + ": " + eventTicks[registeredEvent]);
        }
    }
}