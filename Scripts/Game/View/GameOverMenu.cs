using Godot;
using Godot.Collections;
using Goodot15.Scripts;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.View;

public partial class GameOverMenu : Control, IMenuAnimation {
    private const string LOSE_SOUND = "General Sounds/Negative Sounds/sfx_sounds_error9.wav";

    private const float
        ANIMATION_DURATION = (float)(Utilities.TICKS_PER_SECOND * 5.0f); // Duration of the animation in seconds

    private int animationTicks;

    private Sprite2D background;
    private Button backToMenuButton;
    private double deltaTime;
    private Button exitGameButton;
    private Label gameOverLabel;
    private HUD hud;

    private Array<Node> hudChildren = new();

    // private Sprite2D foreground;
    private bool isAnimating;

    private MenuController menuController;
    private SoundController soundController;

    private Sprite2D timeDarknessSprite;
    private float timeDarknessStart;

    public void Animate() {
        background.Visible = true;
        exitGameButton.Visible = true;
        backToMenuButton.Visible = true;
        gameOverLabel.Visible = true;

        if (!IsInstanceValid(timeDarknessSprite))
            timeDarknessSprite = GameController.Singleton.GetNode<CanvasLayer>("SceneDarknessCanvas")
                .GetNode<Sprite2D>("SceneDarkness");

        timeDarknessStart = timeDarknessSprite.Modulate.A;

        background.Modulate = new Color(0, 0, 0, timeDarknessStart);
        timeDarknessSprite.Modulate = new Color(0, 0, 0, .0f);

        hud = GameController.Singleton.GetNodeOrNull<HUD>("HUD");
        hudChildren = hud.GetChildren();

        exitGameButton.Modulate = new Color(1, 1, 1, 0.0f);
        backToMenuButton.Modulate = new Color(1, 1, 1, 0.0f);
        gameOverLabel.Modulate = new Color(1, 1, 1, 0.0f);

        animationTicks = 0;

        isAnimating = true;
    }

    public override void _Ready() {
        background = GetNode<Sprite2D>("Background");
        exitGameButton = GetNode<Button>("ExitGame");
        backToMenuButton = GetNode<Button>("BackToMenu");
        gameOverLabel = GetNode<Label>("GameOver");

        menuController = GetNode<MenuController>("/root/MenuController");
        soundController = GetNode<SoundController>("/root/SoundController");

        exitGameButton.Pressed += OnExitGameButtonPressed;
        backToMenuButton.Pressed += OnBackToMenuButtonPressed;

        SoundController.Singleton.PlaySound(LOSE_SOUND);
    }

    private void OnExitGameButtonPressed() {
        GetTree().Quit();
    }

    private void OnBackToMenuButtonPressed() {
        menuController.CloseMenus();
        menuController.OpenMainMenu();
        menuController.GetTree().Paused = false;
        soundController.MusicMuted = false;
        isAnimating = false;
    }

    public override void _PhysicsProcess(double delta) {
        deltaTime += delta;

        if (deltaTime < 1 / Utilities.TICKS_PER_SECOND) return;

        deltaTime -= 1 / Utilities.TICKS_PER_SECOND;

        if (!isAnimating) return;
        animationTicks++;

        float t = Utilities.MapRange(0.0f, ANIMATION_DURATION, 0.0f, 1.0f, animationTicks);

        if (t >= 1.0f) isAnimating = false;

        float easedT = t * t * t;

        float mapped = Mathf.Lerp(timeDarknessStart, 1.0f, easedT);

        exitGameButton.Modulate = new Color(1, 1, 1, easedT);
        backToMenuButton.Modulate = new Color(1, 1, 1, easedT);
        gameOverLabel.Modulate = new Color(1, 1, 1, easedT);
        background.Modulate = new Color(0, 0, 0, mapped);

        if (hud != null)
            foreach (Node child in hudChildren)
                if (child is Control controlChild)
                    controlChild.Modulate = new Color(1, 1, 1, 1 - easedT);
    }
}