using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Enums;

namespace Goodot15.Scripts.Game.Controller.Events;

public class NatureResourceEvent : IGameEvent {
    public string EventName => "Nature Resource Event";
    public string Description => "A nature resource event has occurred. A stone has been created.";
    public int TicksUntilNextEvent => Utilities.TimeToTicks(seconds: 1);
    public double Chance => .1d;

    public void OnEvent(GameEventContext context) {
        GameController gameController = context.GameController;
        CardController cardController = gameController.GetCardController();
        CardCreationHelper cardCreationHelper = cardController.CardCreationHelper;

        List<string> pack = cardCreationHelper.GetCardTypePacks(CardPackEnum.Nature);
        string randomCardType = pack[(int)GD.RandRange(0, pack.Count)];

        cardController.CreateCard(randomCardType, context.GameController.GetRandomPositionWithinScreen());
    }
}