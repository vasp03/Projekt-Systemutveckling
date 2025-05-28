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
        oldDayState = DayStateEnum.Invalid;
        canvasLayer = GameController.Singleton.GetNode<CanvasLayer>("SceneDarknessCanvas");
        timeLabel = GameController.Singleton.GetNode<HUD>("HUD")?.GetNode<Label>("DayTimeLabel");
        sprite = canvasLayer.GetNode<Sprite2D>("SceneDarkness");
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

        if (gameController is null || !GodotObject.IsInstanceValid(gameController) ||
            !gameController.IsInsideTree())
            return;

        if (canvasLayer is null || !GodotObject.IsInstanceValid(canvasLayer) ||
            !canvasLayer.IsInsideTree())
            canvasLayer = gameController.GetNode<CanvasLayer>("CanvasLayer");

        if (!GodotObject.IsInstanceValid(timeLabel)) timeLabel = gameController.GetNode<HUD>("HUD")?.GetNode<Label>("DayTimeLabel");

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
        timeLabel.SetText(Utilities.GetTimeOfDay(dayTicks));
        dayState = Utilities.GetCurrentDayState(dayTicks);

        if (dayState == oldDayState) return;

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

    /// <summary>
    ///     Sets the darkness of the scene.
    /// </summary>
    private void SetSceneDarkness(float darkness) {
        darkness = Mathf.Clamp(darkness, 0, 1);

        if (canvasLayer is null || !GodotObject.IsInstanceValid(canvasLayer)) return;

        if (sprite is null || !GodotObject.IsInstanceValid(sprite)) sprite = canvasLayer.GetNode<Sprite2D>("SceneDarkness");

        sprite.Modulate = new Color(0, 0, 0, 1 - darkness);
    }

    private void ShowAndHideTimeLabel(bool show) {
        if (timeLabel is null || !GodotObject.IsInstanceValid(timeLabel)) return;
        if (show)
            timeLabel.Show();
        else
            timeLabel.Hide();
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

    private CanvasLayer canvasLayer;
    private Sprite2D sprite;
    private Label timeLabel;

    #endregion

    #region Event state data

    private DayStateEnum dayState;
    private bool isPaused;
    private DayStateEnum oldDayState;
    private float oldSceneDarkness;
    public int dayTicks { get; set; }

    #endregion
}