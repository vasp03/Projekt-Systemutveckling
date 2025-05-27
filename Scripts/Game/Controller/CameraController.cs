using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class CameraController : ITickable {
    private Camera2D Camera2D => GameController.Singleton.GetNode<Camera2D>("Camera2D");

    public void PostTick(double delta) {
        if (remainingShakeTicks > 0) {
            remainingShakeTicks--;
            Camera2D.GlobalPosition = new Vector2(
                Mathf.Sin(remainingShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 * X_SHAKE_FREQUENCY),
                Mathf.Sin(remainingShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 * Y_SHAKE_FREQUENCY)
            ) * intensity + CAMERA_ORIGIN;
        } else {
            Camera2D.GlobalPosition = CAMERA_ORIGIN;
        }
    }

    public void Shake(float intensity, int ticks) {
        remainingShakeTicks = ticks;
        this.intensity = intensity;
    }

    #region Static values

    private const float X_SHAKE_FREQUENCY = 200f;
    private const float Y_SHAKE_FREQUENCY = 290f;
    private readonly static Vector2 CAMERA_ORIGIN = new(1280 / 2, 720 / 2);

    #endregion Static values

    #region Shaking properties

    private float intensity;
    private int remainingShakeTicks;

    #endregion Shaking properties
}