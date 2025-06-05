using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Camera controller responsible for camera-related functions
/// </summary>
public class CameraController : ITickable {
    private readonly static int END_GAME_ANIMATION_TICKS = Utilities.TimeToTicks(3); // 3 seconds
    private Sprite2D darknessSprite;
    private bool isPlayingEndGameAnimation;
    private int remainingEndGameAnimationTicks;
    private Camera2D Camera2DInstance => GameController.Singleton?.GetNodeOrNull<Camera2D>("CameraCenter");

    #region Animation handling

    public void PostTick(double delta) {
        // Guard clause - if the camera controller happens to execute at the end of the frame and the camera is deleted
        // Do nothing
        if (Camera2DInstance is null)
            return;

        if (isPlayingEndGameAnimation)
            EndGameAnimation();
        else if (RemainingScreenShakeTicks > 0)
            ShakeAnimation();
        else
            Camera2DInstance.GlobalPosition = CAMERA_ORIGIN;
    }

    private void ShakeAnimation() {
        RemainingScreenShakeTicks--;

        Camera2DInstance.GlobalPosition = new Vector2(
            Mathf.Sin(RemainingScreenShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 * X_SHAKE_FREQUENCY),
            Mathf.Sin(RemainingScreenShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 * Y_SHAKE_FREQUENCY)
        ) * LastScreenShakeIntensity + CAMERA_ORIGIN;
    }

    public void PlayEndGameAnimation() {
        if (isPlayingEndGameAnimation) return;

        darknessSprite = GameController.Singleton?.GetNode<CanvasLayer>("HUD")?.GetNode<Sprite2D>("EndGameOverlay");

        darknessSprite.Modulate = new Color(0, 0, 0, 0.0f);
        darknessSprite.Visible = true;

        isPlayingEndGameAnimation = true;
    }

    private void EndGameAnimation() {
        if (remainingEndGameAnimationTicks >= END_GAME_ANIMATION_TICKS) {
            Camera2DInstance.GlobalPosition = CAMERA_ORIGIN;
            isPlayingEndGameAnimation = false;
            MenuController.Singleton.OpenMainMenu();
            return;
        }

        remainingEndGameAnimationTicks++;

        float t = (END_GAME_ANIMATION_TICKS - remainingEndGameAnimationTicks) / (float)END_GAME_ANIMATION_TICKS;
        const float startZoom = 1.0f;
        const float endZoom = 0.75f;

        float invertedT = 1 - Mathf.Clamp(t, 0, 1);
        float zoomFactor = Mathf.Log(1 + 9 * invertedT);

        Camera2DInstance.Zoom = new Vector2(
            startZoom + (endZoom - startZoom) * zoomFactor,
            startZoom + (endZoom - startZoom) * zoomFactor
        );

        darknessSprite.Visible = true;
        darknessSprite.Modulate = new Color(0, 0, 0, Mathf.Clamp(invertedT, 0.0f, 1.0f));

        RemainingScreenShakeTicks = 0; // Reset shake ticks during end game animation
    }

    #endregion Animation handling

    #region Static values

    /// <summary>
    ///     X-direction shake frequency
    /// </summary>
    private const float X_SHAKE_FREQUENCY = 200f;

    /// <summary>
    ///     Y-direction shake frequency
    /// </summary>
    private const float Y_SHAKE_FREQUENCY = 290f;

    /// <summary>
    ///     Camera origin position
    /// </summary>
    private static Vector2 CAMERA_ORIGIN => GameController.Singleton.GetViewportRect().Size / 2;

    #endregion Static values

    #region Shaking properties

    /// <summary>
    ///     Makes the game camera shake for the specified amount of <see cref="ticks" /> and with intensity of
    ///     <see cref="LastScreenShakeIntensity" />
    /// </summary>
    /// <param name="intensity">Intensity oscillation in pixels</param>
    /// <param name="ticks">
    ///     The time the shake event lasts for, in ticks. Use <see cref="Utilities" /> for converting time and
    ///     ticks
    /// </param>
    public void Shake(float intensity, int ticks) {
        RemainingScreenShakeTicks = ticks;
        LastScreenShakeIntensity = intensity;
    }

    /// <summary>
    ///     Screen shake event intensity
    /// </summary>
    public float LastScreenShakeIntensity { get; private set; }

    /// <summary>
    ///     Screen shake duration remaining in ticks
    /// </summary>
    public int RemainingScreenShakeTicks { get; private set; }

    #endregion Shaking properties
}