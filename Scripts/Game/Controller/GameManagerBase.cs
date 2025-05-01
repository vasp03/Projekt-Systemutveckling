using Godot;

namespace Goodot15.Scripts.Game.Controller;

public class GameManagerBase : IGameManager {
    public Node CurrentScene => IGameManager.GameControllerSingleton.GetTree().CurrentScene;

    public GameController GameController => IGameManager.GameControllerSingleton;

    public virtual void OnReady() {
    }

    public virtual void OnUnload() {
    }

    public virtual void OnReset() {
    }
}