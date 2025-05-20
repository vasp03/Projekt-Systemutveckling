using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Enums;

namespace Goodot15.Scripts.Game.Controller.Events;

public class ColdNightEvent : GameEventBase {
    private int dayTicks;
    public override string EventName => "Cold Night";
    public override int TicksUntilNextEvent => (Utilities.GameScaledTimeToTicks(days: 1.0d));
    public override double Chance => 1.0f;

    public override void OnEvent(GameEventContext context) {
        GameController gameController = context.GameController;
        
        DayTimeEvent dayTimeEvent = gameController.GameEventManager.EventInstance<DayTimeEvent>();
        if (dayTimeEvent is null || Utilities.GetCurrentDayState(dayTimeEvent.dayTicks) is not DayStateEnum.Night) return;
        
        GD.Print("Cold Night Event Triggered");
        dayTimeEvent.CurrentTemperature = -10f;

        CardNode.CardController.AllCards.ToList().ForEach(e => {
            if (e.CardType is not CardLiving cardLiving) return;
            cardLiving.Health -= 1;
            cardLiving.Saturation -= 2;
        });
    }
}