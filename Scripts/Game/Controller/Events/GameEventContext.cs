namespace Goodot15.Scripts.Game.Controller.Events;

public struct GameEventContext {
    private readonly GameController gameController;
    private readonly IGameEvent gameEvent;
    public IGameEvent GameEventFired => gameEvent;
    public GameController GameController => gameController;

    public GameEventContext(IGameEvent gameEvent, GameController gameController) {
        this.gameEvent = gameEvent;
        this.gameController = gameController;
    }
}