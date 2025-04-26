using Godot;

public class DayTimeEvent : DayTimeCallback {
    private DayTimeController.DayState OldDayState;

    private GameController GameController;

    public DayTimeEvent(GameController gameController) {
        OldDayState = DayTimeController.DayState.Invalid;
        GameController = gameController;
    }

    public override void DayTimeChanged(DayTimeController.DayState dayState, int ticks) {
        if (dayState == OldDayState) {
            return;
        }

        GD.Print($"Day time changed: {dayState} ({ticks})");

        switch (dayState) {
            case DayTimeController.DayState.Night:
                GameController.GetSoundController().PlayDayTimeSong("Night");
                break;
            case DayTimeController.DayState.Morning:
                GameController.GetSoundController().PlayDayTimeSong("Morning");
                break;
            case DayTimeController.DayState.Day:
                GameController.GetSoundController().PlayDayTimeSong("Day");
                break;
            case DayTimeController.DayState.Evening:
                GameController.GetSoundController().PlayDayTimeSong("Evening");
                break;
            default:
                GD.PrintErr("Invalid day state.");
                break;
        }

        OldDayState = dayState;
    }
}