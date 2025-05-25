using Godot;
using Goodot15.Scripts.Game.Model.Enums;
using Timer = Godot.Timer;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Mouse controller is responsible for managing the cursor icon
/// </summary>
public class MouseController {
    private static readonly Timer LOADING_TIMER = new();
    private static readonly Vector2 CURSOR_OFFSET = new(12, 12);

    private bool isLoading;
    private int loadingIndex;
    private bool sellModeActive;

    /// <summary>
    ///     Constructs a new instance of Mouse Controller, defaults to <see cref="MouseCursorIcon.POINT_SMALL" />
    /// </summary>
    public MouseController() {
        SetMouseCursor(MouseCursorIcon.POINT_SMALL);
    }

    /// <summary>
    ///     Sets a new Mouse cursor icon
    /// </summary>
    /// <param name="cursor"></param>
    /// <returns></returns>
    public bool SetMouseCursor(MouseCursorIcon cursor) {
        if (cursor != MouseCursorIcon.LOADING && isLoading) StopLoading();

        switch (cursor) {
            case MouseCursorIcon.POINT:
            case MouseCursorIcon.POINT_SMALL:
                if (sellModeActive)
                    Input.SetCustomMouseCursor(ICON_SELL_POINT, Input.CursorShape.Arrow, CURSOR_OFFSET);
                else
                    Input.SetCustomMouseCursor(ICON_POINT_SMALL, Input.CursorShape.Arrow, CURSOR_OFFSET);

                return true;
            case MouseCursorIcon.HAND_OPEN:
                if (sellModeActive)
                    Input.SetCustomMouseCursor(ICON_SELL_OPEN, Input.CursorShape.Arrow, CURSOR_OFFSET);
                else
                    Input.SetCustomMouseCursor(ICON_HAND_OPEN, Input.CursorShape.Arrow, CURSOR_OFFSET);

                return true;
            case MouseCursorIcon.HAND_CLOSE:
                if (sellModeActive)
                    Input.SetCustomMouseCursor(ICON_SELL_CLOSE, Input.CursorShape.Arrow, CURSOR_OFFSET);
                else
                    Input.SetCustomMouseCursor(ICON_HAND_CLOSE, Input.CursorShape.Arrow, CURSOR_OFFSET);

                return true;
            case MouseCursorIcon.LOADING:
                if (!isLoading) StartLoading();

                return true;
            default:
                Input.SetCustomMouseCursor(ICON_POINT_SMALL, Input.CursorShape.Arrow, CURSOR_OFFSET);

                return false;
        }
    }

    private void StartLoading() {
        isLoading = true;
        loadingIndex = 0;

        LOADING_TIMER.Timeout += LoadingThread;
        LOADING_TIMER.WaitTime = 0.200;
        LOADING_TIMER.Start();
    }

    private void StopLoading() {
        isLoading = false;
        loadingIndex = 0;
        LOADING_TIMER.Stop();
        SetMouseCursor(MouseCursorIcon.POINT);
    }

    private void LoadingThread() {
        loadingIndex = (loadingIndex + 1) % 8;
        Input.SetCustomMouseCursor(ICON_LOADING_SPRITES[loadingIndex], Input.CursorShape.Arrow, CURSOR_OFFSET);
    }

    public void SetSellMode(bool sellModeActive) {
        this.sellModeActive = sellModeActive;

        SetMouseCursor(MouseCursorIcon.POINT_SMALL);
    }

    #region Static resources

    private static string IconResource(string iconName) {
        return $"{PATH}/{iconName}";
    }

    /// <summary>
    ///     Base path of mouse cursor assets
    /// </summary>
    private const string PATH = "res://Assets/MouseCursor/";

    private static readonly Resource ICON_HAND_CLOSE = ResourceLoader.Load(IconResource("hand_close.png"));
    private static readonly Resource ICON_HAND_OPEN = ResourceLoader.Load(IconResource("hand_open.png"));

    private static readonly Resource[] ICON_LOADING_SPRITES = [
        ResourceLoader.Load(IconResource("loading_1.png")),
        ResourceLoader.Load(IconResource("loading_2.png")),
        ResourceLoader.Load(IconResource("loading_3.png")),
        ResourceLoader.Load(IconResource("loading_4.png")),
        ResourceLoader.Load(IconResource("loading_5.png")),
        ResourceLoader.Load(IconResource("loading_6.png")),
        ResourceLoader.Load(IconResource("loading_7.png")),
        ResourceLoader.Load(IconResource("loading_8.png"))
    ];

    private readonly Resource ICON_POINT = ResourceLoader.Load(IconResource("point.png"));
    private readonly Resource ICON_POINT_SMALL = ResourceLoader.Load(IconResource("point_small.png"));

    private readonly Resource ICON_SELL_OPEN = ResourceLoader.Load(IconResource("sell_open.png"));
    private readonly Resource ICON_SELL_CLOSE = ResourceLoader.Load(IconResource("sell_close.png"));
    private readonly Resource ICON_SELL_POINT = ResourceLoader.Load(IconResource("sell_point.png"));

    #endregion Static resources
}