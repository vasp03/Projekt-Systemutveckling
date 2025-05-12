using Godot;
using Goodot15.Scripts.Game.Model.Enums;

namespace Goodot15.Scripts.Game.Controller.Events;

public class DayTimeEvent : IPausable, IGameEvent {
    private readonly GameController gameController;
    private bool isPaused;
    private DayStateEnum dayState;
    private DayStateEnum oldDayState;
    private float oldSceneDarkness;
    public string EventName => "Day Time Event";
    public int TicksUntilNextEvent => 1;
    public double Chance => 1.0d;
    private int ticks = 0;
    private readonly Label timeLabel;
    private CanvasLayer canvasLayer;
    private Sprite2D sprite;

    /// <summary>
    ///     An event to handle when the day changes and its time.
    /// </summary>
    public DayTimeEvent(GameController gameController) {
        oldDayState = DayStateEnum.Invalid;
        this.gameController = gameController;
        canvasLayer = gameController.GetNode<CanvasLayer>("CanvasLayer");
        timeLabel = canvasLayer.GetNode<Label>("DayTimeLabel");
        sprite = canvasLayer.GetNode<Sprite2D>("Sprite2D");
    }

    public void OnEvent(GameEventContext context) {
        if (isPaused) return;

        ticks++;

        if (ticks > Utilities.TICKS_PER_DAY) {
            ticks = 0;
        }

        SetSceneDarkness(ticks);
        timeLabel.SetText(Utilities.GetTimeOfDay(ticks));
        dayState = Utilities.GetCurrentDayState(ticks);

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
    ///    Sets the paused state of the event.
    /// </summary>
    /// <param name="isPaused">True if the event should be paused, false otherwise.</param>
    public void SetPaused(bool isPaused) {
        if (gameController is null || !GodotObject.IsInstanceValid(gameController) ||
            !gameController.IsInsideTree()) {
            return;
        }

        this.isPaused = isPaused;

        if (isPaused) {
            SetSceneDarkness(1.0f);
        } else {
            SetSceneDarkness(oldSceneDarkness);
        }
    }

    /// <summary>
    ///   Sets the darkness of the scene.
    /// </summary>
    private void SetSceneDarkness(float darkness) {
        darkness = Mathf.Clamp(darkness, 0, 1);

        if (canvasLayer is null || sprite is null) {
            return;
        }

        sprite.Modulate = new Color(0, 0, 0, 1 - darkness);
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

        if (timeOfDay == oldSceneDarkness) return;

        oldSceneDarkness = timeOfDay;

        SetSceneDarkness(timeOfDay);
    }
}