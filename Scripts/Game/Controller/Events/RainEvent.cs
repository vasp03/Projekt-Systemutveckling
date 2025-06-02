using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Controller.Events;

public class RainEvent : CardSpawnEvent {
    private const string RAIN_SFX = "General Sounds/Weird Sounds/sfx_sound_noise.wav";
    private const int WATER_CARD_SPAWN_COUNT = 3;

    public override string EventName => "It's raining";
    public override int TicksUntilNextEvent => Utilities.GameScaledTimeToTicks(days: 1);
    public override double Chance => 0.5d;
    public override int SpawnCardCount => WATER_CARD_SPAWN_COUNT;
    public override string SpawnCardSfx => RAIN_SFX;

    public override Card CardInstance() {
        return new MaterialWater();
    }

    public override void OnEvent(GameEventContext context) {
        context.GameController.SoundController.PlayAmbianceType(AmbianceSoundType.Rain);
        base.OnEvent(context);
    }
}