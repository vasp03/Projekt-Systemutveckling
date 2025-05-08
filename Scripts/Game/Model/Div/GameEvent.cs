using System;

namespace Goodot15.Scripts.Game.Model.Div;

public class GameEvent {
    public Action EventAction;
    public string EventName;

    public GameEvent(string eventName, Action eventAction) {
        EventName = eventName;
        EventAction = eventAction;
    }
}