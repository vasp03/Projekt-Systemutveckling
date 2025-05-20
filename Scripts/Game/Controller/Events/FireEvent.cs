using Goodot15.Scripts.Game.Model.Material_Cards;

namespace Goodot15.Scripts.Game.Controller.Events;

public class FireEvent : GameEvent {
    private const int FIRE_CARD_SPAWN_COUNT = 3;

    public override string EventName => "Fire event";
    public override int TicksUntilNextEvent => Utilities.GameScaledTimeToTicks(days: 0.5d);
    public override double Chance => 0.25d;

    public override void OnEvent(GameEventContext context) {
        for (int i = 0; i < FIRE_CARD_SPAWN_COUNT; i++)
            context.GameController.CardController.CreateCard(new MaterialFire(),
                context.GameController.NextRandomPositionOnScreen());
    }
}