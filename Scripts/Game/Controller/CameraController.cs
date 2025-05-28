using System;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

public class CameraController : ITickable {
    private Camera2D Camera2D => GameController.Singleton.GetNode<Camera2D>("CameraCenter");
    private bool isPlayinhgEndGameAnimation = false;
    private const int END_GAME_ANIMATION_TICKS = 60 * 3; // 3 seconds
    private int remainingEndGameAnimationTicks = 0;
    private Sprite2D darknessSprite;
    private Sprite2D timeDarknessSprite;
    private float timeDarknessStart;

    public void PostTick(double delta) {
        if (isPlayinhgEndGameAnimation) {
            EndGameAnimation();
        } else if (remainingShakeTicks > 0) {
            ShakeAnimation();
        } else {
            Camera2D.GlobalPosition = CAMERA_ORIGIN;
        }
    }

    public void Shake(float intensity, int ticks) {
        remainingShakeTicks = ticks;
        this.intensity = intensity;
    }

    private void ShakeAnimation() {
        remainingShakeTicks--;
        Camera2D.GlobalPosition = new Vector2(
            Mathf.Sin(remainingShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 * X_SHAKE_FREQUENCY),
            Mathf.Sin(remainingShakeTicks / (float)Utilities.TICKS_PER_SECOND * Mathf.Pi * 2 * Y_SHAKE_FREQUENCY)
        ) * intensity + CAMERA_ORIGIN;
    }

    public void PlayEndGameAnimation() {
        if (isPlayinhgEndGameAnimation) return;

        darknessSprite = GameController.Singleton?.GetNode<CanvasLayer>("HUD")?.GetNode<Sprite2D>("EndGameOverlay");
        timeDarknessSprite = GameController.Singleton?.GetNode<CanvasLayer>("CanvasLayer")?.GetNode<Sprite2D>("Sprite2D");

        timeDarknessStart = timeDarknessSprite.Modulate.A;

        darknessSprite.Modulate = new Color(0, 0, 0, 0.0f);
        darknessSprite.Visible = true;

        isPlayinhgEndGameAnimation = true;
    }

    private void EndGameAnimation() {
        if (remainingEndGameAnimationTicks >= END_GAME_ANIMATION_TICKS) {
            Camera2D.GlobalPosition = CAMERA_ORIGIN;
            isPlayinhgEndGameAnimation = false;
            MenuController.Singleton.OpenMainMenu();
            Camera2D.Zoom = new Vector2(1, 1);
            return;
        }

        remainingEndGameAnimationTicks++;

        float t = (END_GAME_ANIMATION_TICKS - remainingEndGameAnimationTicks) / (float)END_GAME_ANIMATION_TICKS;
        float startZoom = 1.0f;
        float endZoom = 0.75f;

        GD.Print($"End game animation tick: {remainingEndGameAnimationTicks}, t: {t}");

        float invertedT = 1 - Mathf.Clamp(t, 0, 1);
        float zoomFactor = Mathf.Log(1 + 9 * invertedT);

        Camera2D.Zoom = new Vector2(
            startZoom + (endZoom - startZoom) * zoomFactor,
            startZoom + (endZoom - startZoom) * zoomFactor
        );

        darknessSprite.Visible = true;
        darknessSprite.Modulate = new Color(0, 0, 0, Mathf.Clamp(invertedT, 0.0f, 1.0f));

        timeDarknessSprite.Modulate = new Color(1, 1, 1, Mathf.Clamp(invertedT, 0.0f, 1.0f) * timeDarknessStart);

        remainingShakeTicks = 0; // Reset shake ticks during end game animation
    }

    #region Static values

    private const float X_SHAKE_FREQUENCY = 200f;
    private const float Y_SHAKE_FREQUENCY = 290f;
    private static readonly Vector2 CAMERA_ORIGIN = new(1280 / 2, 720 / 2);

    #endregion Static values

    #region Shaking properties

    private float intensity;
    private int remainingShakeTicks;

    #endregion Shaking properties
}