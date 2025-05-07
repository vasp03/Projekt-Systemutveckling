using Godot;
using Goodot15.Scripts.Game.Model.Enums;
using Timer = Godot.Timer;

namespace Goodot15.Scripts.Game.Controller;

public class MouseController : GameManagerBase {
    private const string PATH = "res://Assets/MouseCursor/";

    private readonly static Timer LoadingTimer = new();
    private readonly static Vector2 Offset = new(12, 12);
    private readonly Resource Hand_close = ResourceLoader.Load(PATH + "hand_close.png");
    private readonly Resource Hand_open = ResourceLoader.Load(PATH + "hand_open.png");

    private readonly Resource[] loadingResources = [
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
    private bool IsLoading;
    private int LoadingIndex;

    public MouseController(GameController gameController) : base(gameController) {
        SetMouseCursor(MouseCursorEnum.point_small);
    }

    public bool SetMouseCursor(MouseCursorEnum cursor) {
        if (cursor != MouseCursorEnum.loading && IsLoading) {
            stopLoading();
        }

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
                if (!IsLoading) startLoading();
                return true;
            default:
                Input.SetCustomMouseCursor(Point_small, Input.CursorShape.Arrow, Offset);
                return false;
        }
    }

    private void startLoading() {
        IsLoading = true;
        LoadingIndex = 0;

        if (LoadingTimer == null) {
            return;
        }

        LoadingTimer.Timeout += LoadingThread;
        LoadingTimer.WaitTime = 0.200;
        LoadingTimer.Start();
    }

    private void stopLoading() {
        IsLoading = false;
        LoadingIndex = 0;
        LoadingTimer.Stop();
        SetMouseCursor(MouseCursorEnum.point);
    }

    private void LoadingThread() {
        LoadingIndex = (LoadingIndex + 1) % 8;
        Input.SetCustomMouseCursor(loadingResources[LoadingIndex], Input.CursorShape.Arrow, Offset);
    }
}