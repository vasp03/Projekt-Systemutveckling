using Godot;
using Goodot15.Scripts.Game.Model.Enums;

namespace Goodot15.Scripts.Game.Controller.Events;

public class DayTimeEvent : IDayTimeCallback, IPauseCallback {
    private readonly GameController gameController;
    private DayStateEnum oldDayState;
    private float oldSceneDarkness;

    private bool isPaused;

    /// <summary>
    ///     An event to handle when the day changes and its time.
    /// </summary>
    public DayTimeEvent(GameController gameController) {
        oldDayState = DayStateEnum.Invalid;
        this.gameController = gameController;
        this.gameController.MenuController.AddPauseCallback(this);
    }

    /// <summary>
    ///     Called each tick with the current time of day and the current day state
    /// </summary>
    /// <param name="dayState"></param>
    /// <param name="ticks"></param>
    public void DayTimeChanged(DayStateEnum dayState, int ticks) {
        SetSceneDarkness(ticks);

        if (dayState == oldDayState || isPaused) return;

        switch (dayState) {
            case DayStateEnum.Night:
                gameController.SoundController.PlayDayTimeSong("Night");
                break;
            case DayStateEnum.Morning:
                gameController.SoundController.PlayDayTimeSong("Morning");
                break;
            case DayStateEnum.Day:
                gameController.SoundController.PlayDayTimeSong("Day");
                break;
            case DayStateEnum.Evening:
                gameController.SoundController.PlayDayTimeSong("Evening");
                break;
            case DayStateEnum.Invalid:
            case DayStateEnum.Paused:
            default:
                gameController.SoundController.ToggleMusicMuted();
                break;
        }

        oldDayState = dayState;
    }

    public void PauseToggle(bool isPaused) {
        if (gameController == null || !Godot.GodotObject.IsInstanceValid(gameController) || !gameController.IsInsideTree()) return;

        this.isPaused = isPaused;

        if (isPaused)
            gameController.SetSceneDarkness(1.0f);
        else
            gameController.SetSceneDarkness(oldSceneDarkness);
    }


    /// <summary>
    ///     Sets the darkness of the scene based on the time of day
    /// </summary>
    /// <param name="ticks">The current time of day in ticks</param>
    private void SetSceneDarkness(int ticks) {
        // Turn ticks per day into a float between 0.5 and 1.0 with 1 being when ticks per day is half way
        // through the day and 0.5 being when ticks per day is at the start and end of the day

        int midDay = Utilities.TICKS_PER_DAY / 2;

        float timeOfDay = 0f;

        if (ticks < midDay)
            timeOfDay = Utilities.MapRange(0, midDay, 0.5f, 1f, ticks);
        else
            timeOfDay = Utilities.MapRange(midDay, Utilities.TICKS_PER_DAY, 1f, 0.5f, ticks);

        timeOfDay = Mathf.Round(timeOfDay * 1000) / 1000;

        if (timeOfDay == oldSceneDarkness) return;

        oldSceneDarkness = timeOfDay;

        gameController.SetSceneDarkness(timeOfDay);
    }
}