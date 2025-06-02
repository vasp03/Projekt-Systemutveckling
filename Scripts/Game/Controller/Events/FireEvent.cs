using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Controller.Events;

public class FireEvent : CardSpawnEvent {
    private const int FIRE_CARD_SPAWN_COUNT = 3;

    public override string EventName => "Fire event";
    public override int TicksUntilNextEvent => Utilities.GameScaledTimeToTicks(days: 1d);
    public override double Chance => 0.25d;

    public override int SpawnCardCount => FIRE_CARD_SPAWN_COUNT;
    public override string SpawnCardSfx => null;

    public override Card CardInstance() {
        return new MaterialFire();
    }

    public override void OnEvent(GameEventContext context) {
        context.GameController.SoundController.PlayAmbianceType(AmbianceSoundType.Fire);
        base.OnEvent(context);
    }
}