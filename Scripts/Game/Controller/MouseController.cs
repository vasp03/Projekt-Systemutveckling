using Godot;
using Timer = Godot.Timer;

public partial class MouseController : Node {
	public enum MouseCursor {
		point,
		point_small,
		hand_open,
		hand_close,
		loading
	}

	private static readonly string path = "res://Assets/MouseCursor/";
	private Resource hand_close = ResourceLoader.Load(path + "hand_close.png");
	private Resource hand_open = ResourceLoader.Load(path + "hand_open.png");

	private bool isLoading;
	private int loadingIndex;

	private Resource[] loadingResources = [
		ResourceLoader.Load(path + "loading_1.png"),
		ResourceLoader.Load(path + "loading_2.png"),
		ResourceLoader.Load(path + "loading_3.png"),
		ResourceLoader.Load(path + "loading_4.png"),
		ResourceLoader.Load(path + "loading_5.png"),
		ResourceLoader.Load(path + "loading_6.png"),
		ResourceLoader.Load(path + "loading_7.png"),
		ResourceLoader.Load(path + "loading_8.png")
	];

	private Timer loadingTimer = new();

	private Resource point = ResourceLoader.Load(path + "point.png");
	private Resource point_small = ResourceLoader.Load(path + "point_small.png");

	public MouseController() {
		SetMouseCursor(MouseCursor.point_small);
	}

	public bool SetMouseCursor(MouseCursor cursor) {
		if (cursor != MouseCursor.loading && isLoading) stopLoading();

		switch (cursor) {
			case MouseCursor.point:
				Input.SetCustomMouseCursor(point);
				return true;
			case MouseCursor.point_small:
				Input.SetCustomMouseCursor(point_small);
				return true;
			case MouseCursor.hand_open:
				Input.SetCustomMouseCursor(hand_open);
				return true;
			case MouseCursor.hand_close:
				Input.SetCustomMouseCursor(hand_close);
				return true;
			case MouseCursor.loading:
				if (!isLoading) startLoading();
				return true;
			default:
				SetMouseCursor(MouseCursor.point_small);
				return false;
		}
	}

	private void startLoading() {
		isLoading = true;
		loadingIndex = 0;
		if (loadingTimer == null) return;
		loadingTimer.Connect("timeout", new Callable(this, nameof(LoadingThread)));
		loadingTimer.WaitTime = 0.200;
		loadingTimer.Start();
	}

	private void stopLoading() {
		isLoading = false;
		loadingIndex = 0;
		loadingTimer.Stop();
		SetMouseCursor(MouseCursor.point);
	}

	private void LoadingThread() {
		loadingIndex = (loadingIndex + 1) % 8;
		Input.SetCustomMouseCursor(loadingResources[loadingIndex]);
	}
}