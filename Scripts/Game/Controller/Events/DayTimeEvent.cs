using System;
using Godot;

namespace Goodot15.Scripts.Game.Controller.Events;

public class DayTimeEvent : IDayTimeCallback {
    private readonly DateTime LastTickTime = DateTime.Now;
    private DayTimeManager.DAY_STATE OldDayState = DayTimeManager.DAY_STATE.Invalid;

    public void DayTimeChanged(DayTimeManager.DAY_STATE dayState, int ticks) {
        if (dayState == OldDayState) return;

        GD.Print($"Day time changed: {dayState} ({ticks})");

        GD.Print($"Last tick time: {LastTickTime}");
        GD.Print($"Current time of day: {DateTime.Now}");
        GD.Print($"Time since last tick: {(DateTime.Now - LastTickTime).Seconds} s");


        GameController gameController = IGameManager.GameControllerSingleton;
        SoundManager soundManager = gameController.GetManager<SoundManager>();

        switch (dayState) {
            case DayTimeManager.DAY_STATE.Night:
                soundManager.PlayDayTimeSong("Night");
                gameController.SetSceneDarkness(0.5f);
                break;
            case DayTimeManager.DAY_STATE.Morning:
                soundManager.PlayDayTimeSong("Morning");
                gameController.SetSceneDarkness(0.75f);
                break;
            case DayTimeManager.DAY_STATE.Day:
                soundManager.PlayDayTimeSong("Day");
                gameController.SetSceneDarkness(1.0f);
                break;
            case DayTimeManager.DAY_STATE.Evening:
                soundManager.PlayDayTimeSong("Evening");
                gameController.SetSceneDarkness(0.75f);
                break;
            case DayTimeManager.DAY_STATE.Invalid:
            case DayTimeManager.DAY_STATE.Paused:
            default:
                soundManager.StopMusic();
                gameController.SetSceneDarkness(1.0f);
                break;
        }

        OldDayState = dayState;
    }
}