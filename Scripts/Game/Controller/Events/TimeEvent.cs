using System;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller.Events;

public class TimeEvent : GameEventBase, ITickable {
    private GameEventManager gameEventManager;
    
    public override string EventName => "Time event";
    public override int TicksUntilNextEvent => Utilities.TICKS_PER_DAY / 5;
    public override double Chance => 1d;

    public DayPhase CurrentPhase { get; private set; }

    public override void OnEvent(GameEventContext context) {
        switch (CurrentPhase) {
            case DayPhase.Night:
                context.GameController.GetSoundController().PlayDayTimeSong("Night");
                context.GameController.SetSceneDarkness(0.5f);
                break;
            case DayPhase.Morning:
                context.GameController.GetSoundController().PlayDayTimeSong("Morning");
                context.GameController.SetSceneDarkness(0.75f);
                break;
            case DayPhase.Day:
                context.GameController.GetSoundController().PlayDayTimeSong("Day");
                context.GameController.SetSceneDarkness(1f);
                break;
            case DayPhase.Evening:
                context.GameController.GetSoundController().PlayDayTimeSong("Evening");
                context.GameController.SetSceneDarkness(0.75f);
                break;
        }
        
        CurrentPhase = (DayPhase)(((int)CurrentPhase + 1) % Enum.GetValues(typeof(DayPhase)).Length);
        
        GD.Print($"new phase: {CurrentPhase}, Global ticks: {context.CurrentGlobalTicks}");
        
        //SetSceneDarkness(context.CurrentGlobalTicks);
    }

    public enum DayPhase {
        Night = 0,
        Morning = 1,
        Day = 2,
        Evening = 3
    }

    public void PostTick() {
        GameController.Singleton.TimeLabel.Text = GetTimeOfDay(gameEventManager.GlobalTicks);
    }

    /// <summary>
    ///     Sets the darkness of the scene based on the time of day
    /// </summary>
    /// <param name="ticks">The current time of day in ticks</param>
    // private void SetSceneDarkness(int ticks) {
    //     ticks %= TicksUntilNextEvent;
    //     // Turn ticks per day into a float between 0.5 and 1.0 with 1 being when ticks per day is half way
    //     // through the day and 0.5 being when ticks per day is at the start and end of the day
// 
    //     int midDay = TicksUntilNextEvent / 2;
// 
    //     float timeOfDay = 0f;
// 
    //     if (ticks < midDay)
    //         timeOfDay = Utilities.MapRange(0, midDay, 0.5f, 1f, ticks);
    //     else
    //         timeOfDay = Utilities.MapRange(midDay, TicksUntilNextEvent, 1f, 0.5f, ticks);
// 
    //     timeOfDay = Mathf.Round(timeOfDay * 1000) / 1000;
// 
    //     GameController.Singleton.SetSceneDarkness(timeOfDay);
    //     GameController.Singleton.TimeLabel.Text = GetTimeOfDay(ticks);
    // }
    
    private string GetTimeOfDay(int ticks) {
        int hours = ticks / (TicksUntilNextEvent / 24);
        int minutes = ticks % (TicksUntilNextEvent / 24) * 60 / (TicksUntilNextEvent / 24);

        // Round minutes to the nearest 10 minutes
        minutes = (int)Math.Round(minutes / 10.0) * 10;

        return $"{hours:D2}:{minutes:D2}";
    }

    public override void OnInit(GameEventManager eventManager) {
        this.gameEventManager = eventManager;
        
        eventManager.PostEvent(new GameEventContext(this, GameController.Singleton, 0));
    }
}