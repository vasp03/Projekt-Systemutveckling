using Godot;

namespace Goodot15.Scripts.Game.Controller;

public class GameManagerBase : IGameManager {
    /// <summary>
    ///     Constructs a GameManager with a GameController
    /// </summary>
    /// <param name="coreGameController"></param>
    protected GameManagerBase(GameController coreGameController) {
        CoreGameController = coreGameController;
    }

    /// <summary>
    ///     Constructs a GameManager with no GameController supplied - may be used if there is no use for a GameController
    /// </summary>
    protected GameManagerBase() : this(null) {
    }

    public GameController CoreGameController { get; }
    public Node CurrentScene => CoreGameController.GetTree().CurrentScene;
}