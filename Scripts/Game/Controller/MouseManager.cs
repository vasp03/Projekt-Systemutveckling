using Godot;
using Timer = Godot.Timer;

namespace Goodot15.Scripts.Game.Controller;

public class MouseManager : GameManagerBase {
    public enum MouseCursor {
        POINT,
        POINT_SMALL,
        HAND_OPEN,
        HAND_CLOSE,
        LOADING
    }

    private const string path = "res://Assets/MouseCursor/";
    private readonly Resource hand_close = ResourceLoader.Load(path + "hand_close.png");
    private readonly Resource hand_open = ResourceLoader.Load(path + "hand_open.png");

    private readonly Resource[] loadingResources = [
        ResourceLoader.Load(path + "loading_1.png"),
        ResourceLoader.Load(path + "loading_2.png"),
        ResourceLoader.Load(path + "loading_3.png"),
        ResourceLoader.Load(path + "loading_4.png"),
        ResourceLoader.Load(path + "loading_5.png"),
        ResourceLoader.Load(path + "loading_6.png"),
        ResourceLoader.Load(path + "loading_7.png"),
        ResourceLoader.Load(path + "loading_8.png")
    ];

    private readonly Timer loadingTimer = new();
    private readonly Vector2 offset = new(12, 12);

    private readonly Resource point = ResourceLoader.Load(path + "point.png");
    private readonly Resource point_small = ResourceLoader.Load(path + "point_small.png");
    private bool isLoading;
    private int loadingIndex;

    public MouseManager() {
        SetMouseCursor(MouseCursor.POINT_SMALL);
    }

    public bool SetMouseCursor(MouseCursor cursor) {
        if (cursor != MouseCursor.LOADING && isLoading) StopLoading();

        switch (cursor) {
            case MouseCursor.POINT:
                Input.SetCustomMouseCursor(point, Input.CursorShape.Arrow, offset);
                return true;
            case MouseCursor.POINT_SMALL:
                Input.SetCustomMouseCursor(point_small, Input.CursorShape.Arrow, offset);
                return true;
            case MouseCursor.HAND_OPEN:
                Input.SetCustomMouseCursor(hand_open, Input.CursorShape.Arrow, offset);
                return true;
            case MouseCursor.HAND_CLOSE:
                Input.SetCustomMouseCursor(hand_close, Input.CursorShape.Arrow, offset);
                return true;
            case MouseCursor.LOADING:
                if (!isLoading) StartLoading();
                return true;
            default:
                Input.SetCustomMouseCursor(point_small, Input.CursorShape.Arrow, offset);
                return false;
        }
    }

    private void StartLoading() {
        isLoading = true;
        loadingIndex = 0;
        if (loadingTimer == null) return;
        loadingTimer.Timeout += LoadingThread;
        // loadingTimer.Connect("timeout", new Callable(this, nameof(LoadingThread)));
        loadingTimer.WaitTime = 0.200;
        loadingTimer.Start();
    }

    private void StopLoading() {
        isLoading = false;
        loadingIndex = 0;
        loadingTimer.Stop();
        SetMouseCursor(MouseCursor.POINT);
    }

    private void LoadingThread() {
        loadingIndex = (loadingIndex + 1) % 8;
        Input.SetCustomMouseCursor(loadingResources[loadingIndex], Input.CursorShape.Arrow, offset);
    }
}