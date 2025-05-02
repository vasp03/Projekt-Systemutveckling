using System;

namespace Goodot15.Scripts.Game.Model.Div;

public class GameEvent {
    public Action eventAction;
    public string eventName;

    public GameEvent(string eventName, Action eventAction) {
        this.eventName = eventName;
        this.eventAction = eventAction;
    }
}