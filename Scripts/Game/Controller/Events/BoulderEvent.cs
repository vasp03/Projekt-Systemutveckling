using System.Numerics;
using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller.Events;

public class BoulderEvent : GameEvent {
    private const int BOULDER_START_OFFSET = 250;
    private const int BOULDER_COUNT = 1;
    
    public override string EventName => "Random boulders";

    public override int TicksUntilNextEvent => Utilities.TimeToTicks(seconds: 5);//Utilities.GameScaledTimeToTicks(days: 0.5d);

    public override double Chance => 1;// 0.2d;

    public override void OnEvent(GameEventContext context) {
        bool fromLeft = GD.Randf() > .5d;
        Vector2 randomYPosition = context.GameController.NextRandomPositionOnScreen() * new Vector2(0,1);

        int edgePosition = 
            fromLeft 
                ? -BOULDER_START_OFFSET 
                : (int)context.GameController.GetViewportRect().Size.X + BOULDER_START_OFFSET;
        
        for (int i = 0; i < BOULDER_COUNT; i++) {
            context.GameController.CardController.CreateCard(
                new Boulder(fromLeft
                    ? Vector2.Left
                    : Vector2.Right), randomYPosition + Vector2.Right * edgePosition
            );
        }
    }
}