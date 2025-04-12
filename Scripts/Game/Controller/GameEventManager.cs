using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Interface;
using Godot;
using System;

using System.Collections.Generic;


namespace Goodot15.Scripts.Game.Controller;
public partial class GameEventManager : Node, ITickable {
    private List<GameEvent> events = new();
    private PackedScene meteoriteCardScene;
    private Node cardParent;
    private int tickCounter = 0;
    private int tickInterval = 60;
    private Random random = new();

    public override void _Ready() {
        meteoriteCardScene = GD.Load<PackedScene>("res://MeteoriteCard.tscn");
        cardParent = GetNode("");
        events.Add(new GameEvent("Meteorite Strike", SpawnMeteoriteStrike));
    }
    public void preTick() {
        throw new NotImplementedException();
    }

    public void postTick() {
        throw new NotImplementedException();
    }

    public void SpawnMeteoriteStrike() {
        if (meteoriteCardScene == null || cardParent == null) {
            GD.PrintErr("Cannot spawn meteorite card, scene or parent is null");
            return;
        }

        Node2D meteorite = (Node2D)meteoriteCardScene.Instantiate();

        RandomNumberGenerator randomize = new();
        randomize.Randomize();
        meteorite.Position = new Vector2(randomize.RandfRange(100, 800), randomize.RandfRange(100, 500));
        
        cardParent.AddChild(meteorite);
    }
}