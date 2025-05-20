using Godot;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class Boulder(Vector2 travelDirection) : Card("",false), ICardAnimateable, ITickable {
    private const float MOVEMENT_RATE = 100;
    private const float ROTATION_RATE = 360;
    private static readonly int TICKS_ALIVE = Utilities.TimeToTicks(seconds: 15d);
    private int remainingTicksAlive = TICKS_ALIVE;
    
    public Vector2 TravelDirection { get; set; } = travelDirection;
    public override int Value => -1;
    public override bool CanStackBelow(Card cardBelow) {
        return false;
    }

    public override bool CanStackAbove(Card cardAbove) {
        return false;
    }

    public void Render(Sprite2D cardSprite, double delta) {
        cardSprite.RotationDegrees += ROTATION_RATE * (float)delta;
    }

    public void PostTick(double delta) {
        CardNode.Position += TravelDirection * MOVEMENT_RATE * (float)delta * TravelDirection.X;
        if (remainingTicksAlive-- <= 0) {
            this.CardNode.Destroy();
        }
    }
}