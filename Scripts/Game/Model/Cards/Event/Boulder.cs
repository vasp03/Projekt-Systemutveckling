﻿using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class Boulder : Card, ICardAnimateable, ITickable {
    private const string BOULDER_CRUSH_SFX = "Explosions/Shortest/sfx_exp_shortest_hard4.wav";

    private const float MOVEMENT_RATE = 100;
    private const float ROTATION_RATE = 360;
    private readonly static int TICKS_ALIVE = Utilities.TimeToTicks(15d);
    private int remainingTicksAlive = TICKS_ALIVE;

    private Vector2 travelDirection;

    public Boulder(Vector2 direction) : base("Boulder", false) {
        TravelDirection = direction;
    }

    public Vector2 TravelDirection {
        get => travelDirection;
        set => travelDirection = value.Normalized();
    }

    public override int Value => -1;
    public override bool AlwaysOnTop => true;

    public void Render(Sprite2D cardSprite, double delta) {
        cardSprite.RotationDegrees += ROTATION_RATE * (float)delta * travelDirection.X;
    }

    public void PostTick(double delta) {
        CardNode.Position += TravelDirection * MOVEMENT_RATE * (float)delta;
        if (remainingTicksAlive-- <= 0) {
            CardNode.Destroy();
            return;
        }

        ConvertOverlappingCardsToTrash();
    }

    public override bool CanStackBelow(Card cardBelow) {
        return false;
    }

    public override bool CanStackAbove(Card cardAbove) {
        return false;
    }

    private void ConvertOverlappingCardsToTrash() {
        ICollection<CardNode> cardsNoTrashYet = CardNode.OverlappingCards
            .Where(e => e.CardType is not MaterialTrash && e.CardType is not Boulder).ToArray();
        foreach (CardNode cardNode in cardsNoTrashYet) {
            GameController.Singleton.SoundController.PlaySound(BOULDER_CRUSH_SFX);
            cardNode.CardType = new MaterialTrash();
        }
    }
}