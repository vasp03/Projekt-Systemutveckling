using System;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Living;

public abstract class LivingAnimal(string textureAddress, bool movable, int cardValue)
    : CardLiving(textureAddress, movable, cardValue), ICardProducer {
    private int _produceTimer;
    public virtual int? TicksUntilProducedCard => Utilities.TimeToTicks(days: 0.5d);

    public int ProduceTickProgress {
        get => _produceTimer;
        set => _produceTimer = Math.Max(0, value);
    }

    public override int TicksUntilFullyStarved => Utilities.TimeToTicks(days: 3d);
    public override int TicksUntilSaturationDecrease => Utilities.TimeToTicks(days: 1d);

    public abstract Card ProduceCard();

    public override void PostTick() {
        base.PostTick();
        if (TicksUntilProducedCard is not null && Saturation > 0) ProduceTickProgress++;
    }

    protected override void ExecuteTickLogic() {
        base.ExecuteTickLogic();
        if (ProduceTickProgress >= TicksUntilProducedCard) {
            ProduceTickProgress = 0;
            CardNode.CardController.CreateCard(ProduceCard(), CardNode.Position + Vector2.Down * 15f);
        }
    }
}