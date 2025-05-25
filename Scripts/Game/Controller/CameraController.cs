using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

/// <summary>
///     Camera controller responsible for camera-related functions
/// </summary>
public class CameraController : ITickable {
    /// <summary>
    ///     Camera2D reference being used by the game and the controller.
    /// </summary>
    public Camera2D Camera2D => GameController.Singleton.GetNode<Camera2D>("Camera2D");

    public void PostTick(double delta) {
        // Shake feature
        ShakeScreen();
    }

    private void ShakeScreen() {
        if (RemainingScreenShakeTicks > 0) {
            RemainingScreenShakeTicks--;
            Camera2D.GlobalPosition = new Vector2(
                Mathf.Sin(RemainingScreenShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 *
                          X_SHAKE_FREQUENCY),
                Mathf.Sin(RemainingScreenShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 *
                          Y_SHAKE_FREQUENCY)
            ) * LastScreenShakeIntensity + CAMERA_ORIGIN;
        } else {
            Camera2D.GlobalPosition = CAMERA_ORIGIN;
        }
    }


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