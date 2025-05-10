namespace Goodot15.Scripts.Game.Controller.Events;

public readonly struct GameEventContext(IGameEvent gameEvent, GameController gameController, int globalTicks) {
    public IGameEvent GameEventFired => gameEvent;
    public GameController GameController => gameController;
    public int CurrentGlobalTicks => globalTicks;
}