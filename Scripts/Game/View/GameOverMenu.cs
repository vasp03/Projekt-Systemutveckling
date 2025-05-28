using Godot;
using Goodot15.Scripts;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;

public partial class GameOverMenu : Control, IMenuAnimation {
    private const string LOSE_SOUND = "General Sounds/Negative Sounds/sfx_sounds_error9.wav";

    private Sprite2D background;
    private Button backToMenuButton;
    private Button exitGameButton;
    private Label gameOverLabel;

    private MenuController menuController;
    private SoundController soundController;
    // private Sprite2D foreground;
    private bool isAnimating = false;
    private const float ANIMATION_DURATION = 60.0f * 5.0f; // Duration of the animation in seconds
    private int animationTicks = 0;
    private double deltaTime = 0.0;

    private Sprite2D timeDarknessSprite;
    private float timeDarknessStart = 0.0f;

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

    public void Animate() {
        background.Visible = true;
        exitGameButton.Visible = true;
        backToMenuButton.Visible = true;
        gameOverLabel.Visible = true;

        if (!IsInstanceValid(timeDarknessSprite)) {
            timeDarknessSprite = GameController.Singleton.GetNode<CanvasLayer>("CanvasLayer").GetNode<Sprite2D>("Sprite2D");
        }

        timeDarknessStart = timeDarknessSprite.Modulate.A;

        background.Modulate = new Color(0, 0, 0, timeDarknessStart);
        timeDarknessSprite.Modulate = new Color(0, 0, 0, .0f);

        exitGameButton.Modulate = new Color(1, 1, 1, 0.0f);
        backToMenuButton.Modulate = new Color(1, 1, 1, 0.0f);
        gameOverLabel.Modulate = new Color(1, 1, 1, 0.0f);

        animationTicks = 0;

        isAnimating = true;
    }

    public override void _PhysicsProcess(double delta) {
        deltaTime += delta;

        if (deltaTime < 1 / Utilities.TICKS_PER_SECOND) {
            return;
        }

        deltaTime -= 1 / Utilities.TICKS_PER_SECOND;

        if (!isAnimating) return;
        animationTicks++;

        float t = Utilities.MapRange(0.0f, ANIMATION_DURATION, 0.0f, 1.0f, animationTicks);

        if (t >= 1.0f) {
            isAnimating = false;
        }

        float easedT = t * t * t;

        float mapped = Mathf.Lerp(timeDarknessStart, 1.0f, easedT);

        exitGameButton.Modulate = new Color(1, 1, 1, easedT);
        backToMenuButton.Modulate = new Color(1, 1, 1, easedT);
        gameOverLabel.Modulate = new Color(1, 1, 1, easedT);
        background.Modulate = new Color(0, 0, 0, mapped);
    }
}