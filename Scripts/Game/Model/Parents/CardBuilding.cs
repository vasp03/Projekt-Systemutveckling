using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Parents;

public abstract class CardBuilding : Card, ITickable {
    private int currentProduceTick;

    protected CardBuilding(string textureAddress, bool movable) : base(textureAddress,
        movable) {
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
        currentProduceTick = (currentProduceTick + 1) % ProduceTimeInTicks;
    }
}