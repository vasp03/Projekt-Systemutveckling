using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class CameraController : ITickable {
    private static readonly Vector2 CAMERA_ORIGIN = new Vector2(1280 / 2, 720 / 2);
    
    
    private const float xFrequency = 200f;
    private const float yFrequency = 290f;
    private Camera2D Camera2D => GameController.Singleton.GetNode<Camera2D>("Camera2D");

    private int remainingShakeTicks = 0;
    private float intensity = 0;
    public void Shake(float intensity, int ticks) {
        remainingShakeTicks = ticks;
        this.intensity = intensity;
    }

    public void PostTick() {
        if (remainingShakeTicks > 0) {
            remainingShakeTicks--;
            Camera2D.GlobalPosition = new Vector2(
                Mathf.Sin((float)remainingShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 * xFrequency),
                Mathf.Sin((float)remainingShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 * yFrequency)
            ) * intensity + CAMERA_ORIGIN;
        } else {
            Camera2D.GlobalPosition = CAMERA_ORIGIN;
        }
    }
}