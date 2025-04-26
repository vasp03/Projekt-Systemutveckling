using System;
using System.Reflection.Metadata;

namespace Goodot15.Scripts;

public static class Utilities {
    /// <summary>
    ///     How many ticks is a single day.
    /// </summary>
    public const int TICKS_PER_DAY = 36000;

    public const double TICKS_PER_SECOND = 60;

    /// <summary>
    ///     Converts the specified time in to ticks, keep in mind the time values are in real life units.
    /// </summary>
    /// <param name="seconds">Real-life Seconds</param>
    /// <param name="minutes">Real-life Minutes</param>
    /// <param name="hours">Real-life Hours</param>
    /// <param name="days">Real-life Days</param>
    /// <returns>Real-life time units converted to ticks (60 ticks = 1 second)</returns>
    public static int TimeToTicks(double seconds = 0, double minutes = 0, double hours = 0, double days = 0) {
        return (int)Math.Floor(
            seconds * TICKS_PER_SECOND + // Seconds to ticks
            minutes * 60 * TICKS_PER_SECOND + // Minutes to seconds to ticks
            hours * 60 * 60 * TICKS_PER_SECOND + // Hours to seconds to ticks
            days * 24 * 60 * 60 * TICKS_PER_SECOND // Days to seconds to ticks
        );
    }

    /// <summary>
    ///     Converts the specified time in to ticks, keep in mind the time values are in-game time units.
    /// </summary>
    /// <param name="seconds">Game Seconds</param>
    /// <param name="minutes">Game Minutes</param>
    /// <param name="hours">Game Hours</param>
    /// <param name="days">Game Days</param>
    /// <returns>Game time units converted to ticks (60 ticks = 1 second)</returns>
    public static int GameScaledTimeToTicks(double seconds = 0, double minutes = 0, double hours = 0, double days = 0) {
        int gameTicks = TimeToTicks(seconds, minutes, hours, days);

        // How many real ticks per in-game tick (scaling factor)
        double factorScaledToDays = TICKS_PER_DAY / (double)TimeToTicks(days: 1);

        // Scale the game ticks to real time
        return (int)Math.Floor(gameTicks * factorScaledToDays);
    }

    /// <summary>
    ///    Converts the specified ticks to time
    /// </summary>
    /// <param name="ticks">Ticks to convert</param>
    /// <returns>Seconds (60 ticks = 1 second)</returns>
    public static int TicksToTime(int ticks) {
        return (int)Math.Floor(ticks / (float)TICKS_PER_SECOND);
    }
}