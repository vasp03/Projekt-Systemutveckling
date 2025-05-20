using System.Numerics;
using Godot;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller.Events;

public class BoulderEvent : GameEvent {
    private const int BOULDER_COUNT = 1;
    
    public override string EventName => "Random boulders";

    public override int TicksUntilNextEvent => Utilities.GameScaledTimeToTicks(days: 0.5d);

    public override double Chance => 0.2d;

    public override void OnEvent(GameEventContext context) {
        bool fromLeft = GD.Randf() > .5d;
        Vector2 randomPositition = context.GameController.NextRandomPositionOnScreen() * Vector2.Down;

        for (int i = 0; i < BOULDER_COUNT; i++) {
            context.GameController.CardController.CreateCard(new BoulderCard(), randomPositition + (fromLeft
                ? Vector2.Left
                : Vector2.Right) * 2);
        }
    }
}