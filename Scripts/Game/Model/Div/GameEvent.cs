using System;

namespace Goodot15.Scripts.Game.Model.Div;

public class GameEvent {
    public Action EventAction;
    public string EventName;

    public GameEvent(string eventName, Action eventAction) {
        this.EventName = eventName;
        this.EventAction = eventAction;
    }
}