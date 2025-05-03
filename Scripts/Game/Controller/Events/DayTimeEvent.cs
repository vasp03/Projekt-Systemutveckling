using System;
using Godot;
using Utilities = Goodot15.Scripts.Utilities;

public class DayTimeEvent : IDayTimeCallback {
    private DayTimeController.DAY_STATE OldDayState;

    private Goodot15.Scripts.Game.Controller.GameController GameController;

    private DateTime LastTickTime = DateTime.Now;

    private float OldSceneDarkness = 0f;

    /// <summary>
    ///  An event to handle when the day changes and its time. 
    /// </summary>
    public DayTimeEvent(Goodot15.Scripts.Game.Controller.GameController gameController) {
        OldDayState = DayTimeController.DAY_STATE.Invalid;
        GameController = gameController;
    }

    /// <summary>
    ///   CCalled each tick with the current time of day and the current day state
    /// </summary>
    /// <param name="dayState"></param>
    /// <param name="ticks"></param>
    public void DayTimeChanged(DayTimeController.DAY_STATE dayState, int ticks) {
        SetSceneDarness(ticks);

        if (dayState == OldDayState) {
            return;
        }

        switch (dayState) {
            case DayTimeController.DAY_STATE.Night:
                GameController.GetSoundController().PlayDayTimeSong("Night");
                break;
            case DayTimeController.DAY_STATE.Morning:
                GameController.GetSoundController().PlayDayTimeSong("Morning");
                break;
            case DayTimeController.DAY_STATE.Day:
                GameController.GetSoundController().PlayDayTimeSong("Day");
                break;
            case DayTimeController.DAY_STATE.Evening:
                GameController.GetSoundController().PlayDayTimeSong("Evening");
                break;
            case DayTimeController.DAY_STATE.Invalid:
            case DayTimeController.DAY_STATE.Paused:
            default:
                GameController.GetSoundController().StopMusic();
                break;
        }

        OldDayState = dayState;
    }

    /// <summary>
    ///  Sets the darkness of the scene based on the time of day
    /// </summary>
    /// <param name="ticks">The current time of day in ticks</param>
    private void SetSceneDarness(int ticks) {
        // Turn ticks per day into a float between 0.5 and 1.0 with 1 being when ticks per day is half way
        // through the day and 0.5 being when ticks per day is at the start and end of the day

        int midDay = Utilities.TICKS_PER_DAY / 2;

        float timeOfDay = 0f;

        if (ticks < midDay) {
            timeOfDay = Utilities.MapRange(0, midDay, 0.5f, 1f, ticks);
        } else {
            timeOfDay = Utilities.MapRange(midDay, Utilities.TICKS_PER_DAY, 1f, 0.5f, ticks);
        }

        timeOfDay = Mathf.Round(timeOfDay * 1000) / 1000;

        if (timeOfDay == OldSceneDarkness) {
            return;
        }

        OldSceneDarkness = timeOfDay;

        GameController.SetSceneDarkness(timeOfDay);
    }
}