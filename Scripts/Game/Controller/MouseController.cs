using Godot;
using Goodot15.Scripts.Game.Model.Enums;
using Timer = Godot.Timer;

namespace Goodot15.Scripts.Game.Controller;

public class MouseController {
    private const string PATH = "res://Assets/MouseCursor/";

    private static readonly Timer LoadingTimer = new();
    private static readonly Vector2 Offset = new(12, 12);

    private bool isLoading;
    private int loadingIndex;

    public MouseController() {
        SetMouseCursor(MouseCursorEnum.point_small);
    }

    public bool SetMouseCursor(MouseCursorEnum cursor) {
        if (cursor != MouseCursorEnum.loading && isLoading) StopLoading();

        switch (cursor) {
            case MouseCursorEnum.point:
                Input.SetCustomMouseCursor(Point, Input.CursorShape.Arrow, Offset);
                return true;
            case MouseCursorEnum.point_small:
                Input.SetCustomMouseCursor(Point_small, Input.CursorShape.Arrow, Offset);
                return true;
            case MouseCursorEnum.hand_open:
                Input.SetCustomMouseCursor(Hand_open, Input.CursorShape.Arrow, Offset);
                return true;
            case MouseCursorEnum.hand_close:
                Input.SetCustomMouseCursor(Hand_close, Input.CursorShape.Arrow, Offset);
                return true;
            case MouseCursorEnum.loading:
                if (!isLoading) StartLoading();
                return true;
            default:
                Input.SetCustomMouseCursor(Point_small, Input.CursorShape.Arrow, Offset);
                return false;
        }
    }

    private void StartLoading() {
        isLoading = true;
        loadingIndex = 0;

        LoadingTimer.Timeout += LoadingThread;
        LoadingTimer.WaitTime = 0.200;
        LoadingTimer.Start();
    }

    private void StopLoading() {
        isLoading = false;
        loadingIndex = 0;
        LoadingTimer.Stop();
        SetMouseCursor(MouseCursorEnum.point);
    }

    private void LoadingThread() {
        loadingIndex = (loadingIndex + 1) % 8;
        Input.SetCustomMouseCursor(loadingResources[loadingIndex], Input.CursorShape.Arrow, Offset);
    }

    #region Static resources

    private static readonly Resource Hand_close = ResourceLoader.Load(PATH + "hand_close.png");
    private static readonly Resource Hand_open = ResourceLoader.Load(PATH + "hand_open.png");

    private static readonly Resource[] loadingResources = [
        ResourceLoader.Load(PATH + "loading_1.png"),
        ResourceLoader.Load(PATH + "loading_2.png"),
        ResourceLoader.Load(PATH + "loading_3.png"),
        ResourceLoader.Load(PATH + "loading_4.png"),
        ResourceLoader.Load(PATH + "loading_5.png"),
        ResourceLoader.Load(PATH + "loading_6.png"),
        ResourceLoader.Load(PATH + "loading_7.png"),
        ResourceLoader.Load(PATH + "loading_8.png")
    ];

    private readonly Resource Point = ResourceLoader.Load(PATH + "point.png");
    private readonly Resource Point_small = ResourceLoader.Load(PATH + "point_small.png");

    #endregion Static resources
}