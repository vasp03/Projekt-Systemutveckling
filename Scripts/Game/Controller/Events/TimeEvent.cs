using System;

namespace Goodot15.Scripts.Game.Controller.Events;

public class TimeEvent : GameEventBase {
    public override string EventName => "Time event";
    public override int TicksUntilNextEvent => Utilities.TICKS_PER_DAY;
    public override double Chance => 1d;

    public DayPhase CurrentPhase { get; private set; }

    public override void OnEvent(GameEventContext context) {
        CurrentPhase = (DayPhase)(((int)CurrentPhase + 1) % Enum.GetValues(typeof(DayPhase)).Length);
        
        switch (CurrentPhase) {
            case DayPhase.Night:
                context.GameController.GetSoundController().PlayDayTimeSong("Night");
                break;
            case DayPhase.Morning:
                context.GameController.GetSoundController().PlayDayTimeSong("Morning");
                break;
            case DayPhase.Day:
                context.GameController.GetSoundController().PlayDayTimeSong("Day");
                break;
            case DayPhase.Evening:
                context.GameController.GetSoundController().PlayDayTimeSong("Evening");
                break;
        }
    }

    public enum DayPhase {
        Night = 0,
        Morning = 1,
        Day = 2,
        Evening = 3
    }
}