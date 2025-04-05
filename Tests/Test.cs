using System.Linq;
using GdUnit4;
using Godot;

namespace Goodot15.Tests;

[TestSuite]
public class Test {
	[TestCase]
	// Very basic Unit test
	public void Test_StackCard() {
		// Load a scene
		ISceneRunner runner = ISceneRunner.Load("res://Scenes/mainScene.tscn");
		// Simulate that we pressed space
		runner.SimulateKeyPress(Key.Space);
		// Fetch if ANY CardNode has appeared on the screen - which it has
		Assertions.AssertObject(runner.Scene().GetChildren().FirstOrDefault(e => e is CardNode)).IsNotNull();
	}
}