using Godot;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class Boulder(Vector2 travelDirection) : Card("",false), ICardAnimateable, ITickable {
    private const double ROTATION_RATE = 360;
    
    public Vector2 TravelDirection { get; private set; } = travelDirection;
    public override int Value => -1;
    public override bool CanStackBelow(Card cardBelow) {
        return false;
    }

    public override bool CanStackAbove(Card cardAbove) {
        return false;
    }

    public void Render(Sprite2D cardSprite, double delta) {
        cardSprite.RotationDegrees += (float)ROTATION_RATE * (float)delta;
    }

    public void PostTick() {
        throw new System.NotImplementedException();
    }
}