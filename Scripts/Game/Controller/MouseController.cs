using Godot;
using System;
using System.Threading;
using Timer = Godot.Timer;

public partial class MouseController : Node {
    private static string path = "res://Assets/MouseCursor/";

    private Resource point = ResourceLoader.Load(path + "point.png");
    private Resource point_small = ResourceLoader.Load(path + "point_small.png");
    private Resource hand_open = ResourceLoader.Load(path + "hand_open.png");
    private Resource hand_close = ResourceLoader.Load(path + "hand_close.png");
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
    private Vector2 offset = new Vector2(12, 12);
    private bool isLoading = false;
    private int loadingIndex = 0;

    private Timer loadingTimer = new Timer();

    public MouseController() {
        SetMouseCursor(MouseCursor.point_small);
    }

    public enum MouseCursor {
        point,
        point_small,
        hand_open,
        hand_close,
        loading
    }

    public bool SetMouseCursor(MouseCursor cursor) {
        if (cursor != MouseCursor.loading && isLoading) {
            stopLoading();
        }

        switch (cursor) {
            case MouseCursor.point:
                Input.SetCustomMouseCursor(point, Input.CursorShape.Arrow, offset);
                return true;
            case MouseCursor.point_small:
                Input.SetCustomMouseCursor(point_small, Input.CursorShape.Arrow, offset);
                return true;
            case MouseCursor.hand_open:
                Input.SetCustomMouseCursor(hand_open, Input.CursorShape.Arrow, offset);
                return true;
            case MouseCursor.hand_close:
                Input.SetCustomMouseCursor(hand_close, Input.CursorShape.Arrow, offset);
                return true;
            case MouseCursor.loading:
                if (!isLoading) {
                    startLoading();
                }
                return true;
            default:
                Input.SetCustomMouseCursor(point_small, Input.CursorShape.Arrow, offset);
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
        Input.SetCustomMouseCursor(loadingResources[loadingIndex], Input.CursorShape.Arrow, offset);
    }
}
