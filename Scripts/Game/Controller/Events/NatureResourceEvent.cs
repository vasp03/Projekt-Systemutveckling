using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Enums;

namespace Goodot15.Scripts.Game.Controller.Events;

public class NatureResourceEvent : IGameEvent {
    public string Description => "A nature resource event has occurred. A stone has been created.";
    public string EventName => "Nature Resource Event";
    public int TicksUntilNextEvent => Utilities.TimeToTicks(minutes: 1);
    public double Chance => .75d;

    public void OnEvent(GameEventContext context) {
        GameController gameController = context.GameController;
        CardController cardController = gameController.CardController;
        CardCreationHelper cardCreationHelper = cardController.CardCreationHelper;

        List<string> pack = cardCreationHelper.GetCardTypePacks(CardPackEnum.Nature);
        string randomCardType = pack[GD.RandRange(0, pack.Count - 1)];

        cardController.CreateCard(randomCardType, context.GameController.GetRandomPositionWithinScreen());
    }
}