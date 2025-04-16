using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Interface;
using Godot;
using System;

using System.Collections.Generic;
using Goodot15.Scripts.Game.Controller.Events;


namespace Goodot15.Scripts.Game.Controller;
public partial class GameEventManager : Node, ITickable {
    private IList<IGameEvent> registedEvents = [];
    private PackedScene meteoriteCardScene;
    // private Node cardParent;
    private int tickCounter = 0;
    private int tickInterval = 60;
    private Random random = new();

    public override void _Ready() {
        meteoriteCardScene = GD.Load<PackedScene>("res://MeteoriteCard.tscn");
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
    }
}