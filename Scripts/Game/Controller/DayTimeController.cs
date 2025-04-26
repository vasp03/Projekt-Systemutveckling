using Godot;
using Goodot15.Scripts;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class DayTimeController : Node {
    private int DayDuration = Utilities.IN_GAME_DAY_TICKS_FACTOR;

    private int CurrentTimeOfDay = 0;

    private List<DayTimeCallback> Callbacks;

    private bool IsRunning = true;

    public enum DayState {
        Night,
        Morning,
        Day,
        Evening,
        Invalid
    }

    public override void _Ready() {
        Callbacks = new List<DayTimeCallback>();

        // Start the day tick thread
        Thread dayTickThread = new Thread(DayTickThread);
        dayTickThread.Start();
    }

    public void AddCallback(DayTimeCallback callback) {
        Callbacks.Add(callback);
    }

    public void RemoveCallback(DayTimeCallback callback) {
        Callbacks.Remove(callback);
    }

    public static DayState GetCurrentDayState(int ticks) {
        if (ticks >= 0 && ticks < 2000) { // Night
            return DayState.Night;
        } else if (ticks >= 2000 && ticks < 10000) { // Morning
            return DayState.Morning;
        } else if (ticks >= 10000 && ticks < 24000) { // Day
            return DayState.Day;
        } else if (ticks >= 24000 && ticks < 32000) { // Evening
            return DayState.Evening;
        } else if (ticks >= 32000 && ticks < 36000) { // Night
            return DayState.Night;
        } else {
            return DayState.Invalid; // Invalid state
        }
    }

    private void DayTickThread() {
        // Start the day tick thread
        while (IsRunning) {
            // Wait for the next tick
            Thread.Sleep(Utilities.TicksToTime(DayDuration));

            // Update the current time of day
            CurrentTimeOfDay += 1;

            // Check if the current time of day has reached the end of the day
            if (GetCurrentDayState(CurrentTimeOfDay) == DayState.Invalid) {
                CurrentTimeOfDay = 0; // Reset to the start of the day
            }

            foreach (DayTimeCallback callback in Callbacks) {
                callback.DayTimeChanged(GetCurrentDayState(CurrentTimeOfDay), CurrentTimeOfDay);
            }
        }
    }
}
