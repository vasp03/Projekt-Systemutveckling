using System.Threading.Tasks;
using GdUnit4;
using Godot;

namespace Goodot15.Tests;

[TestSuite]
public class PlayButtonTest {
	[TestCase]
	public async Task Test_StartGameWithPlayButton() {
		// Load Menu Scene
		ISceneRunner scene = ISceneRunner.Load("res://Scenes/MenuScenes/MainMenu.tscn", true);
		// Find button
		Button playButton = scene.FindChild("PlayButton") as Button;

		// Simulate button press 
		// scene.SetMousePos(playButton.GlobalPosition);
		// await ISceneRunner.SyncProcessFrame;
		// scene.SimulateMouseButtonPressed(MouseButton.Left);
		// await ISceneRunner.SyncProcessFrame;
		playButton.EmitSignal(Button.SignalName.Pressed);

		await scene.SimulateFrames(2);

		Assertions.AssertString(scene.Scene().GetTree().CurrentScene.SceneFilePath)
			.IsEqual("res://Scenes/mainScene.tscn");
	}

	// [AfterTest]
	// public async Task Test_ExitGame() {
	// 	(Engine.GetMainLoop() as SceneTree).Quit();
	// }
}