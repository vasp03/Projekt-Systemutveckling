using Godot;
using Goodot15.Scripts;
using Goodot15.Scripts.Game.Model.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using Goodot15.Scripts.Game.Controller;

public partial class DayTimeController : ITickable {
	private const int DayDuration = Utilities.TICKS_PER_DAY;
	private const double TicksPerSecond = Utilities.TICKS_PER_SECOND;

	private int CurrentTimeOfDay = 0;

	private List<IDayTimeCallback> Callbacks = [];

	private double TimeCountingToOneTick = 0;

	private bool IsPaused = false;

	private GameController GameController;

	private Label Label;

	private bool HasWarnedAboutLabel = false;

	public DayTimeController(GameController gameController) {
		GameController = gameController;
	}

	public enum DAY_STATE {
		Night,
		Morning,
		Day,
		Evening,
		Invalid,
		Paused
	}

	/// <summary>
	///   Ticks the day time controller
	/// </summary>
	/// <param name="delta">How long time it has been between frames</param>
	public void PreTick(double delta) {
		if (IsPaused) {
			return;
		}

		TimeCountingToOneTick += delta;
		if (TimeCountingToOneTick < (1 / TicksPerSecond)) {
			return;
		} else {
			TimeCountingToOneTick -= 1 / TicksPerSecond;
		}

		// Update the current time of day
		CurrentTimeOfDay += 1;

		// Check if the current time of day has reached the end of the day
		if (CurrentTimeOfDay > DayDuration) {
			CurrentTimeOfDay = 0; // Reset to the start of the day
		}

		foreach (IDayTimeCallback callback in Callbacks) {
			callback.DayTimeChanged(GetCurrentDayState(CurrentTimeOfDay), CurrentTimeOfDay);
		}


		if (HasWarnedAboutLabel) {
			return;
		}

		if (GameController != null && GameController.TimeLabel != null) {
			GameController.TimeLabel.SetText(GetTimeOfDay(CurrentTimeOfDay));
		} else {
			GD.PrintErr("GameController or TimeLabel is null. Cannot update time label.");
			GD.PrintErr("Check Node2D if Time Label is set to a label");
			HasWarnedAboutLabel = true;
		}
	}

	public void PostTick() {
		// This method is not in use right now
	}

	// Method that converts a range of 0 to DayDuration to normal clock time
	public string GetTimeOfDay(int ticks) {
		int hours = ticks / (DayDuration / 24);
		int minutes = ticks % (DayDuration / 24) * 60 / (DayDuration / 24);

		// Round minutes to the nearest 10 minutes
		minutes = (int)Math.Round(minutes / 10.0) * 10;

		return $"{hours:D2}:{minutes:D2}";
	}

	public IDayTimeCallback AddCallback(IDayTimeCallback callback) {
		if (callback == null) {
			throw new ArgumentNullException(nameof(callback), "Callback cannot be null.");
		}

		if (Callbacks == null) {
			Callbacks = [];
		}

		if (Callbacks.Contains(callback)) {
			return callback;
		}

		Callbacks.Add(callback);

		return callback;
	}

	public IDayTimeCallback RemoveCallback(IDayTimeCallback callback) {
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
	public static DAY_STATE GetCurrentDayState(int ticks) {
		if (ticks >= 0 && ticks < DayDurationRatio(DayDuration)) { // Night
			return DAY_STATE.Night;
		} else if (ticks >= DayDurationRatio(DayDuration) && ticks < DayDurationRatio(DayDuration) * 3) { // Morning
			return DAY_STATE.Morning;
		} else if (ticks >= DayDurationRatio(DayDuration) * 3 && ticks < DayDurationRatio(DayDuration) * 7) { // Day
			return DAY_STATE.Day;
		} else if (ticks >= DayDurationRatio(DayDuration) * 7 && ticks < DayDurationRatio(DayDuration) * 9) { // Evening
			return DAY_STATE.Evening;
		} else if (ticks >= DayDurationRatio(DayDuration) * 9 && ticks <= DayDuration) { // Night
			return DAY_STATE.Night;
		} else {
			return DAY_STATE.Invalid; // Invalid state
		}
	}

	private static int DayDurationRatio(int ticks) {
		return ticks / 10;
	}

	public void SetPaused(bool paused) {
		IsPaused = paused;

		if (IsPaused) {
			foreach (IDayTimeCallback callback in Callbacks) {
				callback.DayTimeChanged(DAY_STATE.Paused, CurrentTimeOfDay);
			}
		} else {
			foreach (IDayTimeCallback callback in Callbacks) {
				callback.DayTimeChanged(GetCurrentDayState(CurrentTimeOfDay), CurrentTimeOfDay);
			}
		}
	}
}
