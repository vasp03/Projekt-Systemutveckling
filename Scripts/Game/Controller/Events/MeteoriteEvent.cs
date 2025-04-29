namespace Goodot15.Scripts.Game.Controller.Events;

public class MeteoriteEvent : IGameEvent {
    public string EventName => "Meteorite Strike";
    public int TicksUntilNextEvent => Utilities.GameScaledTimeToTicks(days: 5);
    public double Chance => 0.5d;

    public void OnEvent(GameEventContext context) {
        // if (meteoriteCardScene == null || cardParent == null) {
        //     GD.PrintErr("Cannot spawn meteorite card, scene or parent is null");
        //     return;
        // }
// 
        // Node2D meteorite = (Node2D)meteoriteCardScene.Instantiate();
// 
        // RandomNumberGenerator randomize = new();
        // randomize.Randomize();
        // meteorite.Position = new Vector2(randomize.RandfRange(100, 800), randomize.RandfRange(100, 500));
        // 
        // cardParent.MenuControllerNode.AddChild(meteorite);
    }
}