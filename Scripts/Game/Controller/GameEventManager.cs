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

    public int GlobalTicks { get; private set; }

    public GameEventManager(GameController gameController) : base(gameController) {
        RegisterDefaultEvents();
    }

    public void PreTick() {
    }

    public void PostTick() {
        foreach (IGameEvent registeredEvent in registeredEvents) {
            ITickable? tickableGameEvent = registeredEvent as ITickable;
            tickableGameEvent?.PreTick();
            if (registeredEvent.TicksUntilNextEvent <= eventTicks[registeredEvent]) {
                eventTicks[registeredEvent] = 0;
                if (registeredEvent.Chance >= GD.Randf()) {
                    GameEventContext gameEventContext = new(registeredEvent, GameController, GlobalTicks);
                    PostEvent(gameEventContext);
                }
            } else {
                eventTicks[registeredEvent]++;
            }

            GlobalTicks++;
            tickableGameEvent?.PostTick();
        }
    }

    public void RegisterDefaultEvents() {
            RegisterEvent(new MeteoriteEvent());
            RegisterEvent(new NatureResourceEvent());
            RegisterEvent(new FireEvent());
            RegisterEvent(new TimeEvent());
        }

        public void RegisterEvent(IGameEvent gameEvent) {
            eventTicks.TryAdd(gameEvent, 0);
            registeredEvents.Add(gameEvent);
        }

        public void RegisterGlobalEventListener(IGameEventListener gameEventListener) {
            registeredEventListeners.Add(gameEventListener);
        }
    
        public void PostEvent(GameEventContext gameEventContext) {
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

    public override void Ready() {
        foreach (IGameEvent registeredEvent in registeredEvents)
        {
            registeredEvent.OnInit(this);
        }
    }
}