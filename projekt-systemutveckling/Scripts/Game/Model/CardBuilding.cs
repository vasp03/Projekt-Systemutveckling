using System;
using Goodot15.Scripts.Game.Model.Interface;

public abstract class CardBuilding : Card, ITickable
{
    private int currentProduceTick;

    private int rawProduceTime;
    /// <summary>
    /// Produce time in ticks
    /// 1 tick = 1/60th of a second.
    /// </summary>
    public int ProduceTimeInTicks { get => rawProduceTime; set => rawProduceTime = value; }

    /// <summary>
    /// Produce time in seconds
    /// </summary>
    public int ProduceTimeInSeconds
    {
        get => rawProduceTime / 60;
        set => rawProduceTime = (int)(value * 60);
    }

    /// <summary>
    /// Invoked when a new card is going to be produced. Meant to be overriden
    /// To supply a new Card Instance.
    /// </summary>
    /// <returns>New Card Instance</returns>
    public abstract Card ProduceCard();

    public CardBuilding(string name, string textureAddress, bool movable, int cost, int produceTimeInSeconds) : base(name, textureAddress, movable, cost)
    {
        this.ProduceTimeInSeconds = produceTimeInSeconds;
    }

    public virtual void preTick()
    {
    }

    public virtual void postTick()
    {
        this.currentProduceTick = (this.currentProduceTick + 1) % this.rawProduceTime;
    }
}
