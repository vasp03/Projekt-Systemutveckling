using Godot;
using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller.Events;

/// <summary>
///     Class that handles the time of day and the temperature
/// </summary>
public class DayTimeEvent : GameEvent, IPausable {
    /// <summary>
    ///     An event to handle when the day changes and it's time.
    /// </summary>
    public DayTimeEvent() {
        oldDayPhaseState = DayPhaseState.INVALID;
        sprite = CanvasLayer.GetNode<Sprite2D>("Sprite2D");
        GameController.Singleton.AddPauseCallback(this);
    }

    public override string EventName => "Day Time Event";
    public override int TicksUntilNextEvent => 1;
    public override double Chance => 1.0d;

    public float CurrentTemperature { get; set; }
    public bool temperatureLocked { get; set; } = false;

    /// <summary>
    ///     Sets the paused state of the event.
    /// </summary>
    /// <param name="isPaused">True if the event should be paused, false otherwise.</param>
    public void SetPaused(bool isPaused, bool hideDarknessOverlay = true) {
        GameController gameController = GameController.Singleton;

        this.isPaused = isPaused;

        if (isPaused) {
            // ShowAndHideTimeLabel(false);
            if (hideDarknessOverlay) SetSceneDarkness(1.0f);
        } else {
            ShowAndHideTimeLabel(true);
            SetSceneDarkness(oldSceneDarkness);
        }
    }

    public override void OnEvent(GameEventContext context) {
        if (isPaused) return;

        GameController gameController = GameController.Singleton;

        dayTicks++;

        if (dayTicks > Utilities.TICKS_PER_DAY) dayTicks = 0;

        UpdateTemperature(dayTicks);
        SetSceneDarkness(dayTicks);
        TimeLabel.SetText(Utilities.GetTimeOfDay(dayTicks));
        dayPhaseState = Utilities.GetCurrentDayState(dayTicks);

        if (dayPhaseState == oldDayPhaseState) return;

        switch (dayPhaseState) {
            case DayPhaseState.NIGHT:
                gameController.SoundController.PlayDayTimeSong("Night");
                break;
            case DayPhaseState.MORNING:
                gameController.SoundController.PlayDayTimeSong("Morning");
                break;
            case DayPhaseState.DAY:
                gameController.SoundController.PlayDayTimeSong("Day");
                break;
            case DayPhaseState.EVENING:
                gameController.SoundController.PlayDayTimeSong("Evening");
                break;
            case DayPhaseState.INVALID:
            case DayPhaseState.PAUSED:
            default:
                gameController.SoundController.ToggleMusicMuted();
                break;
        }

        oldDayPhaseState = dayPhaseState;
    }

    /// <summary>
    ///     Sets the darkness of the scene.
    /// </summary>
    private void SetSceneDarkness(float darkness) {
        darkness = Mathf.Clamp(darkness, 0, 1);

        if (CanvasLayer is null || !GodotObject.IsInstanceValid(CanvasLayer)) return;

        if (sprite is null || !GodotObject.IsInstanceValid(sprite)) sprite = CanvasLayer.GetNode<Sprite2D>("SceneDarkness");

        sprite.Modulate = new Color(0, 0, 0, 1 - darkness);
    }

    private void ShowAndHideTimeLabel(bool show) {
        if (TimeLabel is null || !GodotObject.IsInstanceValid(TimeLabel)) return;
        if (show)
            TimeLabel.Show();
        else
            TimeLabel.Hide();
    }

    private void UpdateTemperature(int ticks) {
        const int midDay = Utilities.TICKS_PER_DAY / 2;

        if (ticks < midDay) {
            CurrentTemperature = Utilities.MapRange(0, midDay, 10f, 30f, ticks);
        } else {
            if (!temperatureLocked)
                CurrentTemperature = Utilities.MapRange(midDay, Utilities.TICKS_PER_DAY, 30f, 10f, ticks);
        }
    }


    /// <summary>
    ///     Sets the darkness of the scene based on the time of day
    /// </summary>
    /// <param name="ticks">The current time of day in ticks</param>
    private void SetSceneDarkness(int ticks) {
        // Turn ticks per day into a float between 0.5 and 1.0 with 1 being when ticks per day is half way
        // through the day and 0.5 being when ticks per day is at the start and end of the day

        const int midDay = Utilities.TICKS_PER_DAY / 2;

        float timeOfDay;

        if (ticks < midDay)
            timeOfDay = Utilities.MapRange(0, midDay, 0.5f, 1f, ticks);
        else
            timeOfDay = Utilities.MapRange(midDay, Utilities.TICKS_PER_DAY, 1f, 0.5f, ticks);

        timeOfDay = Mathf.Round(timeOfDay * 1000) / 1000;

        if (timeOfDay == oldSceneDarkness) return;

        oldSceneDarkness = timeOfDay;

        SetSceneDarkness(timeOfDay);
    }

    #region Game object references

    private CanvasLayer CanvasLayer => GameController.Singleton.GetNode<CanvasLayer>("CanvasLayer");
    private Sprite2D sprite;
    private Label TimeLabel => CanvasLayer.GetNode<Label>("DayTimeLabel");

    #endregion

    #region Event state data

    private DayPhaseState dayPhaseState;
    private bool isPaused;
    private DayPhaseState oldDayPhaseState;
    private float oldSceneDarkness;
    public int dayTicks { get; set; }

    #endregion
}