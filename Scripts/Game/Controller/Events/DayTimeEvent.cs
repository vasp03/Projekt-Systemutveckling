using Godot;
using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller.Events;

/// <summary>
///     Class that handles the time of day and the temperature
/// </summary>
public class DayTimeEvent : GameEvent, IPausable {
    public DayTimeEvent() {
        InitializeReferences();

        oldDayPhaseState = DayPhaseState.INVALID;
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

        DayTicks++;

        if (DayTicks > Utilities.TICKS_PER_HALF_DAY) DayTicks = 0;

        UpdateTemperature(DayTicks);
        SetSceneDarkness(DayTicks);
        TimeLabel.SetText(Utilities.GetTimeOfDay(DayTicks));
        DayPhaseState = Utilities.GetCurrentDayState(DayTicks);

        if (DayPhaseState == oldDayPhaseState) return;

        // SoundController.Singleton.StopAmbianceType([AmbianceTypeEnum.Wind, AmbianceTypeEnum.Forest]);

        switch (DayPhaseState) {
            case DayPhaseState.NIGHT:
                SoundController.Singleton.PlayDayTimeSong("Night");
                SoundController.Singleton.PlayAmbianceType(AmbianceSoundType.Wind);
                break;
            case DayPhaseState.MORNING:
                SoundController.Singleton.PlayDayTimeSong("Morning");
                SoundController.Singleton.PlayAmbianceType(AmbianceSoundType.Forest);
                break;
            case DayPhaseState.DAY:
                SoundController.Singleton.PlayDayTimeSong("Day");
                SoundController.Singleton.PlayAmbianceType(AmbianceSoundType.Forest);
                break;
            case DayPhaseState.EVENING:
                SoundController.Singleton.PlayDayTimeSong("Evening");
                SoundController.Singleton.PlayAmbianceType(AmbianceSoundType.Wind);
                break;
            case DayPhaseState.INVALID:
            case DayPhaseState.PAUSED:
            default:
                SoundController.Singleton.ToggleMusicMuted();
                break;
        }

        GameController.Singleton.GetNode<HUD>("HUD").UpdateThermometerUI();
        oldDayPhaseState = DayPhaseState;
    }

    /// <summary>
    ///     Sets the darkness of the scene.
    /// </summary>
    private void SetSceneDarkness(float darkness) {
        darkness = Mathf.Clamp(darkness, 0, 1);
        Sprite.Modulate = new Color(0, 0, 0, 1 - darkness);
    }

    private void ShowAndHideTimeLabel(bool show) {
        if (show)
            TimeLabel.Show();
        else
            TimeLabel.Hide();
    }

    private void UpdateTemperature(int ticks) {
        int midDay = Utilities.TICKS_PER_HALF_DAY / 2;

        if (ticks < midDay) {
            CurrentTemperature = Utilities.MapRange(0, midDay, 10f, 30f, ticks);
        } else {
            if (!temperatureLocked)
                CurrentTemperature = Utilities.MapRange(midDay, Utilities.TICKS_PER_HALF_DAY, 30f, 10f, ticks);
        }
    }


    /// <summary>
    ///     Sets the darkness of the scene based on the time of day
    /// </summary>
    /// <param name="ticks">The current time of day in ticks</param>
    private void SetSceneDarkness(int ticks) {
        // Turn ticks per day into a float between 0.5 and 1.0 with 1 being when ticks per day is half way
        // through the day and 0.5 being when ticks per day is at the start and end of the day

        int midDay = Utilities.TICKS_PER_HALF_DAY / 2;

        float timeOfDay;

        if (ticks < midDay)
            timeOfDay = Utilities.MapRange(0, midDay, 0.5f, 1f, ticks);
        else
            timeOfDay = Utilities.MapRange(midDay, Utilities.TICKS_PER_HALF_DAY, 1f, 0.5f, ticks);

        timeOfDay = Mathf.Round(timeOfDay * 1000) / 1000;

        if (timeOfDay == oldSceneDarkness) return;

        oldSceneDarkness = timeOfDay;

        SetSceneDarkness(timeOfDay);
    }

    #region Game object references

    private void InitializeReferences() {
        DarknessLayer = GameController.Singleton!.GetNode<CanvasLayer>("SceneDarknessCanvas");
        Sprite = DarknessLayer.GetNode<Sprite2D>("SceneDarkness");
        TimeLabel = GameController.Singleton!.GetNode<Label>("HUD/DayTimeLabel");
    }

    private Sprite2D Sprite { get; set; }
    public CanvasLayer DarknessLayer { get; set; }
    private Label TimeLabel { get; set; }

    #endregion

    #region Event state data

    public DayPhaseState DayPhaseState { get; private set; }
    private bool isPaused;
    private DayPhaseState oldDayPhaseState;
    private float oldSceneDarkness;
    public int DayTicks { get; set; }

    #endregion
}