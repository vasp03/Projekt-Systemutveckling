using Godot;
using Goodot15.Scripts.Game.Model.Enums;
using Timer = Godot.Timer;

namespace Goodot15.Scripts.Game.Controller;

public class MouseController {
    private const string PATH = "res://Assets/MouseCursor/";

    private readonly static Timer LoadingTimer = new();
    private readonly static Vector2 Offset = new(12, 12);

    private bool isLoading;
    private int loadingIndex;
    private bool sellModeActive;

    public MouseController() {
        SetMouseCursor(MouseCursorEnum.point_small);
    }

    public bool SetMouseCursor(MouseCursorEnum cursor) {
        if (cursor != MouseCursorEnum.loading && isLoading) StopLoading();

        switch (cursor) {
            case MouseCursorEnum.point:
            case MouseCursorEnum.point_small:
                if (sellModeActive)
                    Input.SetCustomMouseCursor(Sell_point, Input.CursorShape.Arrow, Offset);
                else
                    Input.SetCustomMouseCursor(Point_small, Input.CursorShape.Arrow, Offset);

                return true;
            case MouseCursorEnum.hand_open:
                if (sellModeActive)
                    Input.SetCustomMouseCursor(Sell_open, Input.CursorShape.Arrow, Offset);
                else
                    Input.SetCustomMouseCursor(Hand_open, Input.CursorShape.Arrow, Offset);

                return true;
            case MouseCursorEnum.hand_close:
                if (sellModeActive)
                    Input.SetCustomMouseCursor(Sell_close, Input.CursorShape.Arrow, Offset);
                else
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

    public void SetSellMode(bool sellModeActive) {
        this.sellModeActive = sellModeActive;

        SetMouseCursor(MouseCursorEnum.point_small);
    }

    #region Static resources

    private readonly static Resource Hand_close = ResourceLoader.Load(PATH + "hand_close.png");
    private readonly static Resource Hand_open = ResourceLoader.Load(PATH + "hand_open.png");

    private readonly static Resource[] loadingResources = [
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

    private readonly Resource Sell_open = ResourceLoader.Load(PATH + "sell_open.png");
    private readonly Resource Sell_close = ResourceLoader.Load(PATH + "sell_close.png");
    private readonly Resource Sell_point = ResourceLoader.Load(PATH + "sell_point.png");

    #endregion Static resources
}