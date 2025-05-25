using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Controller.Events;

public class NatureResourceEvent : CardSpawnEvent {
    public override string EventName => "Nature Resource Event";
    public override int TicksUntilNextEvent => Utilities.TimeToTicks(minutes: 1);
    public override double Chance => .75d;

    public override int SpawnCardCount => 1;
    public override string SpawnCardSfx => null;

    public override Card CardInstance() {
        IReadOnlyList<string> pack =
            GameController.Singleton.CardController.CardCreationHelper.GetCardTypePacks(CardPackEnum.Nature);
        string randomCardType = pack[GD.RandRange(0, pack.Count - 1)];
        return GameController.Singleton.CardController.CardCreationHelper.GetCreatedInstanceOfCard(randomCardType);
    }
}