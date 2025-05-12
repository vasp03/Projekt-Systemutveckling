using System;
using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class DayTimeController : ITickable {
    private const int DAY_DURATION = Utilities.TICKS_PER_DAY;

    public GameController GameController { get; private set; }

    private int currentTimeOfDay;

    private bool hasWarnedAboutLabel;

    private bool isPaused;

    private IList<IDayTimeCallback> registeredPausedCallbacks = [];

    private double timeCountingToOneTick;

    public DayTimeController(GameController gameController) {
        GameController = gameController;
    }

    public void PostTick() {
        // This method is not in use right now
    }

    /// <summary>
    ///     Ticks the day time controller
    /// </summary>
    /// <param name="delta">How long time it has been between frames</param>
    public void PreTick(double delta) {
        if (isPaused) return;

        timeCountingToOneTick += delta;
        if (timeCountingToOneTick < 1 / Utilities.TICKS_PER_SECOND) return;

        timeCountingToOneTick -= 1 / Utilities.TICKS_PER_SECOND;

        // Update the current time of day
        currentTimeOfDay += 1;

        // Check if the current time of day has reached the end of the day
        if (currentTimeOfDay > DAY_DURATION) currentTimeOfDay = 0; // Reset to the start of the day

        foreach (IDayTimeCallback callback in registeredPausedCallbacks)
            callback.DayTimeChanged(GetCurrentDayState(currentTimeOfDay), currentTimeOfDay);

        if (GameController is not null && GameController.TimeLabel is not null) {
            GameController.TimeLabel.SetText(GetTimeOfDay(currentTimeOfDay));

            if (hasWarnedAboutLabel) {
            }
        } else {
            GD.PrintErr("GameController or TimeLabel is null. Cannot update time label.");
            GD.PrintErr("Check Node2D if Time Label is set to a label");
            hasWarnedAboutLabel = true;
        }
    }

    // Method that converts a range of 0 to DayDuration to normal clock time
    public string GetTimeOfDay(int ticks) {
        int hours = ticks / (DAY_DURATION / 24);
        int minutes = ticks % (DAY_DURATION / 24) * 60 / (DAY_DURATION / 24);

        // Round minutes to the nearest 10 minutes
        minutes = (int)Math.Round(minutes / 10.0) * 10;

        return $"{hours:D2}:{minutes:D2}";
    }

    public IDayTimeCallback AddDayTimeCallback(IDayTimeCallback callback) {
        if (callback is null) throw new ArgumentNullException(nameof(callback), "Callback cannot be null.");

        if (registeredPausedCallbacks.Contains(callback)) return callback;

        registeredPausedCallbacks.Add(callback);

        return callback;
    }

    public IDayTimeCallback RemoveDayTimeCallback(IDayTimeCallback callback) {
        registeredPausedCallbacks.Remove(callback);
        return callback;
    }

    /// <summary>
    ///     Get the current day state based on the current time of day.
    /// </summary>
    /// <param name="ticks">Current time of day in ticks.</param>
    /// <returns>Current day state.</returns>
    /// <remarks>
    ///     Night: 0 - 1/10 of the day
    ///     Morning: 1/10 - 3/10 of the day
    ///     Day: 3/10 - 7/10 of the day
    ///     Evening: 7/10 - 9/10 of the day
    ///     Night: 9/10 - 1 of the day
    /// </remarks>
    public static DayStateEnum GetCurrentDayState(int ticks) {
        if (ticks >= 0 && ticks < DayDurationRatio(DAY_DURATION))
            // Night
            return DayStateEnum.Night;

        if (ticks >= DayDurationRatio(DAY_DURATION) && ticks < DayDurationRatio(DAY_DURATION) * 3)
            // Morning
            return DayStateEnum.Morning;

        if (ticks >= DayDurationRatio(DAY_DURATION) * 3 && ticks < DayDurationRatio(DAY_DURATION) * 7)
            // Day
            return DayStateEnum.Day;

        if (ticks >= DayDurationRatio(DAY_DURATION) * 7 && ticks < DayDurationRatio(DAY_DURATION) * 9)
            // Evening
            return DayStateEnum.Evening;

        if (ticks >= DayDurationRatio(DAY_DURATION) * 9 && ticks <= DAY_DURATION)
            // Night
            return DayStateEnum.Night;

        return DayStateEnum.Invalid; // Invalid state
    }

    private static int DayDurationRatio(int ticks) {
        return ticks / 10;
    }

    public void SetPaused(bool paused) {
        isPaused = paused;

        if (isPaused)
            foreach (IDayTimeCallback callback in registeredPausedCallbacks) {
                callback.DayTimeChanged(DayStateEnum.Paused, currentTimeOfDay);
                GameController.TimeLabel.Visible = false;
            }
        else
            foreach (IDayTimeCallback callback in registeredPausedCallbacks) {
                callback.DayTimeChanged(GetCurrentDayState(currentTimeOfDay), currentTimeOfDay);
                GameController.TimeLabel.Visible = true;
            }
    }
}