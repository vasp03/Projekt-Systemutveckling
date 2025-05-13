public interface IDayTimeCallback {
    /// <summary>
    ///     Called when the day time changes.
    /// </summary>
    /// <param name="dayState">The current day state.</param>
    /// <param name="ticks">The current time in ticks.</param>
    /// <remarks>Called every tick.</remarks>
    public void DayTimeChanged(DayTimeController.DAY_STATE dayState, int ticks);
}