using Godot;
using Goodot15.Scripts.Game.Model.Enums;

namespace Goodot15.Scripts.Game.Controller.Events;

public class DayTimeEvent : GameEventBase, IPausable {
    /// <summary>
    ///     An event to handle when the day changes and it's time.
    /// </summary>
    public DayTimeEvent(GameController gameController) {
        oldDayState = DayStateEnum.Invalid;
        canvasLayer = gameController.GetNode<CanvasLayer>("CanvasLayer");
        timeLabel = canvasLayer.GetNode<Label>("DayTimeLabel");
        sprite = canvasLayer.GetNode<Sprite2D>("Sprite2D");
        gameController.MenuController.AddPauseCallback(this);
    }

    public override string EventName => "Day Time Event";
    public override int TicksUntilNextEvent => 1;
    public override double Chance => 1.0d;

    /// <summary>
    ///     Sets the paused state of the event.
    /// </summary>
    /// <param name="isPaused">True if the event should be paused, false otherwise.</param>
    public void SetPaused(bool isPaused) {
        GameController gameController = GameController.Singleton;

        if (gameController is null || !GodotObject.IsInstanceValid(gameController) ||
            !gameController.IsInsideTree())
            return;

        if (canvasLayer is null || !GodotObject.IsInstanceValid(canvasLayer) ||
            !canvasLayer.IsInsideTree())
            canvasLayer = gameController.GetNode<CanvasLayer>("CanvasLayer");

        if (!GodotObject.IsInstanceValid(timeLabel)) timeLabel = canvasLayer.GetNode<Label>("DayTimeLabel");

        this.isPaused = isPaused;

        if (isPaused) {
            ShowAndHideTimeLabel(false);
            SetSceneDarkness(1.0f);
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

        if (sprite is null || !GodotObject.IsInstanceValid(sprite)) sprite = canvasLayer.GetNode<Sprite2D>("Sprite2D");

        sprite.Modulate = new Color(0, 0, 0, 1 - darkness);
    }

    private void ShowAndHideTimeLabel(bool show) {
        if (timeLabel is null || !GodotObject.IsInstanceValid(timeLabel)) return;
        if (show)
            timeLabel.Show();
        else
            timeLabel.Hide();
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
    private int dayTicks;

    #endregion
}