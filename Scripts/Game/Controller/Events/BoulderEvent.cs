using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller.Events;

public class BoulderEvent : GameEvent {
    private const int BOULDER_START_OFFSET = 100;
    private const int BOULDER_COUNT = 2;

    public override string EventName => "Random boulders";

    public override int TicksUntilNextEvent =>
        Utilities.TimeToTicks(days: 1d); //Utilities.GameScaledTimeToTicks(days: 0.5d);

    public override double Chance => 0.15d; // 0.2d;

    public override void OnEvent(GameEventContext context) {
        for (int i = 0; i < BOULDER_COUNT; i++) {
            bool fromLeft = GD.RandRange(1, 2) is 1;
            int randomYPosition = (int)context.GameController.NextRandomPositionOnScreen().Y;

            int edgePosition =
                fromLeft
                    ? -BOULDER_START_OFFSET
                    : (int)context.GameController.GetViewportRect().Size.X + BOULDER_START_OFFSET;


            context.GameController.CardController.CreateCard(
                new Boulder(fromLeft
                    ? Vector2.Right
                    : Vector2.Left), new Vector2(edgePosition, randomYPosition)
            );
        }
    }
}