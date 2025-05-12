using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Enums;
using Utilities = Goodot15.Scripts.Utilities;

public class DayTimeEvent : IDayTimeCallback, IPauseCallback {
    private readonly GameController GameController;
    private DayStateEnum OldDayState;
    private float OldSceneDarkness;

    private bool IsPaused;

    /// <summary>
    ///     An event to handle when the day changes and its time.
    /// </summary>
    public DayTimeEvent(GameController gameController) {
        OldDayState = DayStateEnum.Invalid;
        GameController = gameController;
        GameController.GetMenuController().AddPauseCallback(this);
    }

    /// <summary>
    ///     Called each tick with the current time of day and the current day state
    /// </summary>
    /// <param name="dayState"></param>
    /// <param name="ticks"></param>
    public void DayTimeChanged(DayStateEnum dayState, int ticks) {
        SetSceneDarkness(ticks);

        if (dayState == OldDayState || IsPaused) return;

        switch (dayState) {
            case DayStateEnum.Night:
                GameController.GetSoundController().PlayDayTimeSong("Night");
                break;
            case DayStateEnum.Morning:
                GameController.GetSoundController().PlayDayTimeSong("Morning");
                break;
            case DayStateEnum.Day:
                GameController.GetSoundController().PlayDayTimeSong("Day");
                break;
            case DayStateEnum.Evening:
                GameController.GetSoundController().PlayDayTimeSong("Evening");
                break;
            case DayStateEnum.Invalid:
            case DayStateEnum.Paused:
            default:
                GameController.GetSoundController().ToggleMusicMuted();
                break;
        }

        OldDayState = dayState;
    }

    public void PauseToggle(bool isPaused) {
        if (GameController == null || !Godot.GodotObject.IsInstanceValid(GameController) || !GameController.IsInsideTree()) return;

        IsPaused = isPaused;

        if (isPaused)
            GameController.SetSceneDarkness(1.0f);
        else
            GameController.SetSceneDarkness(OldSceneDarkness);
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

        if (timeOfDay == OldSceneDarkness) return;

        OldSceneDarkness = timeOfDay;

        GameController.SetSceneDarkness(timeOfDay);
    }
}