using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Parents;

public abstract class CardBuilding : Card, ITickable {
    private int currentProduceTick;

    protected CardBuilding(string textureAddress, bool movable, int cost, int produceTimeInSeconds,
        CardNode cardNode) : base( 
        textureAddress, movable, cost, cardNode) {
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

    public virtual void preTick() {
    }

    public virtual void postTick() {
        currentProduceTick = (currentProduceTick + 1) % ProduceTimeInTicks;
    }

    /// <summary>
    ///     Invoked when a new card is going to be produced. Meant to be overriden
    ///     To supply a new Card Instance.
    /// </summary>
    /// <returns>New Card Instance</returns>
    public abstract string ProduceCard();
}