using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class Boulder(Vector2 direction) : Card("Boulder", false), ICardAnimateable, ITickable {
    private const float MOVEMENT_RATE = 100;
    private const float ROTATION_RATE = 360;
    private static readonly int TICKS_ALIVE = Utilities.TimeToTicks(15d);
    private int remainingTicksAlive = TICKS_ALIVE;

    private Vector2 travelDirection = direction.Normalized();

    public Vector2 TravelDirection {
        get => travelDirection;
        set => travelDirection = value.Normalized();
    }

    public override int Value => -1;
    public override bool AlwaysOnTop => true;

    public void Render(Sprite2D cardSprite, double delta) {
        cardSprite.RotationDegrees += ROTATION_RATE * (float)delta;
    }

    public void PostTick(double delta) {
        CardNode.Position += TravelDirection * MOVEMENT_RATE * (float)delta * TravelDirection.X;
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
        foreach (CardNode cardNode in cardsNoTrashYet)
            // TODO: SFX for "crushing" cards
            cardNode.CardType = new MaterialTrash();
    }
}