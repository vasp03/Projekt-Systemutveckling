using Godot;
using Goodot15.Scripts;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class DayTimeController {
    private static int DayDuration = Utilities.TICKS_PER_DAY;

    private int CurrentTimeOfDay = 0;

    private List<DayTimeCallback> Callbacks;

    private bool IsRunning = true;

    private Thread DayTickThread;

    public enum DayState {
        Night,
        Morning,
        Day,
        Evening,
        Invalid
    }

    public DayTimeController() {
        Callbacks = new List<DayTimeCallback>();

        DayTickThread = new Thread(DayTickThreadMethod);
        DayTickThread.Start();
    }

    public DayTimeCallback AddCallback(DayTimeCallback callback) {
        if (callback == null) {
            GD.Print("Callback is null.");
            return null;
        }

        if (Callbacks == null) {
            Callbacks = new List<DayTimeCallback>();
        }

        if (Callbacks.Contains(callback)) {
            GD.Print($"Callback {callback} already exists.");
            return callback;
        }
        GD.Print($"Adding callback: {callback}");
        Callbacks.Add(callback);
        return callback;
    }

    public DayTimeCallback RemoveCallback(DayTimeCallback callback) {
        Callbacks.Remove(callback);
        return callback;
    }

    /// <summary>
    ///   Get the current day state based on the current time of day.
    /// </summary>
    /// <param name="ticks">Current time of day in ticks.</param>
    /// <returns>Current day state.</returns>
    /// <remarks>
    /// Night: 0 - 1/10 of the day 
    /// Morning: 1/10 - 3/10 of the day 
    /// Day: 3/10 - 7/10 of the day 
    /// Evening: 7/10 - 9/10 of the day 
    /// Night: 9/10 - 1 of the day 
    /// </remarks>
    public static DayState GetCurrentDayState(int ticks) {
        if (ticks >= 0 && ticks < DayDurationRatio(DayDuration)) { // Night
            return DayState.Night;
        } else if (ticks >= DayDurationRatio(DayDuration) && ticks < DayDurationRatio(DayDuration) * 3) { // Morning
            return DayState.Morning;
        } else if (ticks >= DayDurationRatio(DayDuration) * 3 && ticks < DayDurationRatio(DayDuration) * 7) { // Day
            return DayState.Day;
        } else if (ticks >= DayDurationRatio(DayDuration) * 7 && ticks < DayDurationRatio(DayDuration) * 9) { // Evening
            return DayState.Evening;
        } else if (ticks >= DayDurationRatio(DayDuration) * 9 && ticks <= DayDuration) { // Night
            return DayState.Night;
        } else {
            return DayState.Invalid; // Invalid state
        }
    }

    private static int DayDurationRatio(int ticks) {
        return ticks / 10;
    }

    private void DayTickThreadMethod() {
        // Start the day tick thread
        while (IsRunning) {
            // Wait for the next tick
            Thread.Sleep(1000 / Utilities.TicksPerSecond()); // 60 ticks per second

            // Update the current time of day
            CurrentTimeOfDay += 1;

            // Check if the current time of day has reached the end of the day
            if (CurrentTimeOfDay > DayDuration) {
                CurrentTimeOfDay = 0; // Reset to the start of the day
            }

            foreach (DayTimeCallback callback in Callbacks) {
                callback.DayTimeChanged(GetCurrentDayState(CurrentTimeOfDay), CurrentTimeOfDay);
            }
        }
    }
}
