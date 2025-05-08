using Godot;

namespace Goodot15.Scripts.Game.Controller;

public class GameManagerBase {
    /// <summary>
    ///     Constructs a GameManager with a GameController
    /// </summary>
    /// <param name="coreGameController"></param>
    protected GameManagerBase(GameController gameController) {
        GameController = gameController;
    }

    public GameController GameController { get; }

    public Node CurrentScene => GameController.GetTree().CurrentScene;
}