using System;

namespace Goodot15.Scripts.Game.Model.Div;

public class GameEvent {
    public string  eventName;
    public Action  eventAction;

    public GameEvent(string eventName, string eventDescription, Action eventAction) {
        this.eventName = eventName;
        this.eventAction = eventAction;
    }
}