using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Interface;
using Godot;
using System;
using System.Collections.Generic;

namespace Goodot15.Scripts.Game.Controller;
public partial class EventManager : Node, ITickable {
    private List<GameEvent> events = new();
    public void preTick() {
        throw new NotImplementedException();
    }

    public void postTick() {
        throw new NotImplementedException();
    }
}