namespace Goodot15.Scripts.Game.Controller.Events;

public readonly struct GameEventContext(IGameEvent gameEvent, GameController gameController) {
    public IGameEvent GameEventFired => gameEvent;
    public GameController GameController => gameController;
}