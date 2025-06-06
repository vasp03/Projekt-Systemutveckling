﻿using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Buildings;
using Goodot15.Scripts.Game.Model.Enums;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller.Events;

/// <summary>
///     Represents a cold night event in the game, which triggers during the night and depletes the health and saturation
///     of living cards.
/// </summary>
public class ColdNightEvent : GameEvent, ITickable {
    private readonly GameController gameController;

    /// <summary>
    ///     Indicates whether the event has been triggered.
    /// </summary>
    private bool hasTriggered;

    /// <summary>
    ///     Tracks the last tick when damage was applied to living cards.
    /// </summary>
    private int lastDamageTick;

    private static DayTimeEvent DayTimeEvent => GameController.Singleton.GameEventManager.EventInstance<DayTimeEvent>();

    /// <inheritdoc cref="IGameEvent" />
    /// />
    public override string EventName => "Cold Night";

    public override int TicksUntilNextEvent => Utilities.GameScaledTimeToTicks(hours: 22);
    public override double Chance => 0.15f;


    /// <summary>
    ///     Determines if the event should actually be ticking and be executed
    /// </summary>
    public override bool EventActive {
        get {
            DayPhaseState currentPhaseState = DayTimeEvent.GetCurrentDayState(DayTimeEvent.DayTicks);
            if (!hasTriggered && currentPhaseState is DayPhaseState.NIGHT) return true;

            if (currentPhaseState is DayPhaseState.NIGHT) return false;
            hasTriggered = false;
            DayTimeEvent.TemperatureLocked = false;

            return false;
        }
    }

    /// <summary>
    ///     Executes the damage logic when the event is fired. Applies damage every real time second to all living cards that
    ///     are not stacked on a campfire.
    ///     A campfire can only protect up to 3 living cards from damage
    /// </summary>
    public void PostTick(double delta) {
        if (!hasTriggered) return;

        if (lastDamageTick++ < Utilities.TimeToTicks(1)) return;

        lastDamageTick = 0;

        CardNode.CardController.AllCards.ToList().ForEach(card => {
            bool isStackedOnCampfire = CardNode.CardController.AllCards.Any(otherCard =>
                otherCard.CardType is BuildingCampfire &&
                otherCard.StackAboveWithItself.Contains(card) &&
                otherCard.StackAboveWithItself.Where(c => c.CardType is CardLiving).Take(3).Contains(card));

            if (card.CardType is not CardLiving cardLiving || isStackedOnCampfire) return;
            cardLiving.Health -= 2;
            cardLiving.Saturation -= 3;
        });
    }

    /// <summary>
    ///     Executes the event and sets the temperature to -15 degrees Celsius.
    /// </summary>
    /// <param name="context">not used</param>
    public override void OnEvent(GameEventContext context) {
        DayTimeEvent.TemperatureLocked = true;
        GD.Print("Cold Night Event Triggered");
        DayTimeEvent.CurrentTemperature = -15f;
        GD.Print("Current Temperature: " + DayTimeEvent.CurrentTemperature + "C");

        GameController.Singleton.GetNode<HUD>("HUD").UpdateThermometerUI();
        hasTriggered = true;
    }
}