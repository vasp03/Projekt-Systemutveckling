using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Parents;

public abstract class CardBuilding : Card, ITickable {
    private int CurrentProduceTick;

    protected CardBuilding(string textureAddress, bool movable, int produceTimeInSeconds) : base(textureAddress, movable) {
        ProduceTimeInSeconds = produceTimeInSeconds;
    }

    /// <summary>
    ///     Produce time in ticks
    ///     1 tick = 1/60th of a second.
    /// </summary>
    public int ProduceTimeInTicks { get; set; }

    /// <summary>
    ///     Produce time in seconds
    /// </summary>
    public int ProduceTimeInSeconds {
        get => ProduceTimeInTicks / 60;
        set => ProduceTimeInTicks = value * 60;
    }

    public virtual void PreTick() {
    }

    public virtual void PostTick() {
        CurrentProduceTick = (CurrentProduceTick + 1) % ProduceTimeInTicks;
    }
}