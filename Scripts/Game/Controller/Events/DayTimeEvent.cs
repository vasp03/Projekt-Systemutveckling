using System;
using Godot;

namespace Goodot15.Scripts.Game.Controller.Events;

public class DayTimeEvent : IDayTimeCallback {
    private readonly DateTime LastTickTime = DateTime.Now;
    private DayTimeController.DAY_STATE OldDayState = DayTimeController.DAY_STATE.Invalid;

    public void DayTimeChanged(DayTimeController.DAY_STATE dayState, int ticks) {
        if (dayState == OldDayState) return;

        GD.Print($"Day time changed: {dayState} ({ticks})");

        GD.Print($"Last tick time: {LastTickTime}");
        GD.Print($"Current time of day: {DateTime.Now}");
        GD.Print($"Time since last tick: {(DateTime.Now - LastTickTime).Seconds} s");


        GameController gameController = IGameManager.GameControllerSingleton;
        SoundController soundController = gameController.GetManager<SoundController>();

        switch (dayState) {
            case DayTimeController.DAY_STATE.Night:
                soundController.PlayDayTimeSong("Night");
                gameController.SetSceneDarkness(0.5f);
                break;
            case DayTimeController.DAY_STATE.Morning:
                soundController.PlayDayTimeSong("Morning");
                gameController.SetSceneDarkness(0.75f);
                break;
            case DayTimeController.DAY_STATE.Day:
                soundController.PlayDayTimeSong("Day");
                gameController.SetSceneDarkness(1.0f);
                break;
            case DayTimeController.DAY_STATE.Evening:
                soundController.PlayDayTimeSong("Evening");
                gameController.SetSceneDarkness(0.75f);
                break;
            case DayTimeController.DAY_STATE.Invalid:
            case DayTimeController.DAY_STATE.Paused:
            default:
                soundController.StopMusic();
                gameController.SetSceneDarkness(1.0f);
                break;
        }

        OldDayState = dayState;
    }
}