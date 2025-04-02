using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class ResolutionManager : Node{

	public void ChangeResolution(Vector2I selectedResolution) {
		GD.Print("current mode" + DisplayServer.WindowGetMode());
		GD.Print("option selected: " + selectedResolution);
		

		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen) {
			GD.Print("fullscreen mode");
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			DisplayServer.WindowSetSize(selectedResolution);
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			DisplayServer.WindowSetSize(selectedResolution);
		}
		else {
			DisplayServer.WindowSetSize(selectedResolution);
		}
	}

	public void ChangeDisplayMode(int mode) {
		switch (mode) {
			case 0:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				break;
			case 1:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
				break;
			case 2:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
				DisplayServer.WindowSetSize(DisplayServer.ScreenGetSize());
				DisplayServer.WindowSetPosition(Vector2I.Zero);
				break;
			
		}
	}
}