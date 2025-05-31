using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Controller.Events;

public abstract class CardSpawnEvent : GameEvent {
    /// <summary>
    ///     How many cards should be spawned when the event is triggered
    /// </summary>
    public abstract int SpawnCardCount { get; }

    /// <summary>
    ///     Which sound sfx (if any) should be used, supplying <b>null</b> plays no sound.
    /// </summary>
    public abstract string? SpawnCardSfx { get; }

    /// <summary>
    ///     Card instance to be used when spawning new cards for the event
    /// </summary>
    /// <returns></returns>
    public abstract Card CardInstance();

    public override void OnEvent(GameEventContext context) {
        if (SpawnCardSfx is not null) context.GameController.SoundController.PlaySound(SpawnCardSfx);

        for (int i = 0; i < SpawnCardCount; i++)
            context.GameController.CardController.CreateCard(CardInstance(),
                context.GameController.NextRandomPositionOnScreen());
    }
}