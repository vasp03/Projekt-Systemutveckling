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
                GameController.SetSceneDarkness(0.5f);
                break;
            case DayTimeController.DayState.Morning:
                GameController.GetSoundController().PlayDayTimeSong("Morning");
                GameController.SetSceneDarkness(0.75f);
                break;
            case DayTimeController.DayState.Day:
                GameController.GetSoundController().PlayDayTimeSong("Day");
                GameController.SetSceneDarkness(1f);
                break;
            case DayTimeController.DayState.Evening:
                GameController.GetSoundController().PlayDayTimeSong("Evening");
                GameController.SetSceneDarkness(0.75f);
                break;
            default:
                GD.PrintErr("Invalid day state.");
                break;
        }

        OldDayState = dayState;
    }
}