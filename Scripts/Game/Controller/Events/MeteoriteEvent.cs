using Goodot15.Scripts.Game.Model.Material_Cards;

namespace Goodot15.Scripts.Game.Controller.Events;

public class MeteoriteEvent : GameEventBase {
    public override string EventName => "Meteorite Strike";

    public override int TicksUntilNextEvent =>
        Utilities.GameScaledTimeToTicks(days: 1); // Utilities.GameScaledTimeToTicks(days: 1);

    public override double Chance => 0.25d;


    public override void OnEvent(GameEventContext context) {
        context.GameController.GetCardController()
            .CreateCard(new MaterialMeteorite(), context.GameController.GetRandomPositionWithinScreen());
        context.GameController.GetSoundController().PlaySound("Explosions/Short/meteoriteHit.wav");
        context.GameController.CameraController.Shake(5f, Utilities.TimeToTicks(1d));
    }
}