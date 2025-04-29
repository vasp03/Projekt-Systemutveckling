using System;
using Godot;

public class DayTimeEvent : IDayTimeCallback {
    private DayTimeController.DAY_STATE OldDayState;

    private GameController GameController;

    private DateTime LastTickTime = DateTime.Now;

    public DayTimeEvent(GameController gameController) {
        OldDayState = DayTimeController.DAY_STATE.Invalid;
        GameController = gameController;
    }

    public void DayTimeChanged(DayTimeController.DAY_STATE dayState, int ticks) {
        if (dayState == OldDayState) {
            return;
        }

        GD.Print($"Day time changed: {dayState} ({ticks})");

        GD.Print($"Last tick time: {LastTickTime}");
        GD.Print($"Current time of day: {DateTime.Now}");
        GD.Print($"Time since last tick: {(DateTime.Now - LastTickTime).Seconds} s");

        switch (dayState) {
            case DayTimeController.DAY_STATE.Night:
                GameController.GetSoundController().PlayDayTimeSong("Night");
                GameController.SetSceneDarkness(0.5f);
                break;
            case DayTimeController.DAY_STATE.Morning:
                GameController.GetSoundController().PlayDayTimeSong("Morning");
                GameController.SetSceneDarkness(0.75f);
                break;
            case DayTimeController.DAY_STATE.Day:
                GameController.GetSoundController().PlayDayTimeSong("Day");
                GameController.SetSceneDarkness(1.0f);
                break;
            case DayTimeController.DAY_STATE.Evening:
                GameController.GetSoundController().PlayDayTimeSong("Evening");
                GameController.SetSceneDarkness(0.75f);
                break;
            case DayTimeController.DAY_STATE.Invalid:
            case DayTimeController.DAY_STATE.Paused:
            default:
                GameController.GetSoundController().StopMusic();
                GameController.SetSceneDarkness(1.0f);
                break;
        }

        OldDayState = dayState;
    }
}