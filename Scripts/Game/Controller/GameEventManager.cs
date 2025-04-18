using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Interface;
using Godot;
using System;

using System.Collections.Generic;
using Goodot15.Scripts.Game.Controller.Events;


namespace Goodot15.Scripts.Game.Controller;
public partial class GameEventManager : Node, ITickable {
    private IList<IGameEvent> registedEvents = [];
    private IDictionary<IGameEvent, int> eventTicks = new Dictionary<IGameEvent, int>();
    
    private Random random = new();

    public override void _Ready() {
        RegisterDefaultEvents();
        //cardParent = GetNode("");
    }

    public void RegisterDefaultEvents() {
        RegisterEvent(new MeteoriteEvent());
    }

    public void RegisterEvent(IGameEvent gameEvent) {
        registedEvents.Add(gameEvent);
    }
    public void PreTick() {
    }

    public void PostTick() {
        foreach (IGameEvent registedEvent in registedEvents)
        {
            if (registedEvent.TicksUntilNextEvent <= eventTicks[registedEvent]) {
                eventTicks[registedEvent] = 0;
                
                registedEvent.OnEvent(new GameEventContext());
            }
        }
    }
}