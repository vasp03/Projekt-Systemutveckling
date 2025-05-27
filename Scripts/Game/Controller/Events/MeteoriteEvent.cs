using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Controller.Events;

public class MeteoriteEvent : CardSpawnEvent {
    private const int METEORITE_CARD_SPAWN_COUNT = 1;
    private const string METEORITE_STRIKE_SFX = "Explosions/Short/meteoriteHit.wav";
    public override string EventName => "Meteorite Strike";

    public override int TicksUntilNextEvent =>
        Utilities.GameScaledTimeToTicks(days: 1); // Utilities.GameScaledTimeToTicks(days: 1);

    public override double Chance => 0.25d;

    public override int SpawnCardCount => METEORITE_CARD_SPAWN_COUNT;
    public override string SpawnCardSfx => METEORITE_STRIKE_SFX;

    public override Card CardInstance() {
        return new MaterialMeteorite();
    }

    public override void OnEvent(GameEventContext context) {
        base.OnEvent(context);
        context.GameController.CameraController.Shake(5f, Utilities.TimeToTicks(1d));
    }
}