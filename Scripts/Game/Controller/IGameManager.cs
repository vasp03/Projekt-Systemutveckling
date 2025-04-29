using Godot;

namespace Goodot15.Scripts.Game.Controller;

public interface IGameManager {
	private static GameController gameControllerSingletonInstance;

	public static GameController GameControllerSingleton {
		get {
			if (gameControllerSingletonInstance is null) {
				gameControllerSingletonInstance =
					(Engine.GetMainLoop() as SceneTree).CurrentScene.GetNode<GameController>("/root/GameController");
			}

			return gameControllerSingletonInstance;
		}
	}

	public GameController GameController => IGameManager.GameControllerSingleton;

	public virtual void OnReady() {}

	public virtual void OnUnload() {
	}
}
