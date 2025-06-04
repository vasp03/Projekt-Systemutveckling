using System.Collections.Generic;
using Godot;
using Goodot15.Scripts;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.View;

public partial class GameOverMenu : Control, IMenuAnimation {
    private const string LOSE_SFX = "General Sounds/Negative Sounds/sfx_sounds_error9.wav";

    private static readonly float ANIMATION_DURATION = Utilities.TimeToTicks(5);

    private int animationTicks;

    private Sprite2D background;
    private Button backToMenuButton;
    private double deltaTime;
    private Button exitGameButton;
    private Label gameOverLabel;
    private HUD hud;

    private ICollection<Node> hudChildren = [];

    // private Sprite2D foreground;
    private bool isAnimating;

    private MenuController menuController;
    private SoundController soundController;
    private float timeDarknessStart;

    private Sprite2D TimeDarknessSprite => GameController.Singleton.GameEventManager.EventInstance<DayTimeEvent>()
        .DarknessLayer.GetNode<Sprite2D>("SceneDarkness");

    public void Animate() {
        background.Visible = true;
        exitGameButton.Visible = true;
        backToMenuButton.Visible = true;
        gameOverLabel.Visible = true;

        timeDarknessStart = TimeDarknessSprite.Modulate.A;

        background.Modulate = new Color(0, 0, 0, timeDarknessStart);
        TimeDarknessSprite.Modulate = new Color(0, 0, 0, .0f);

        hud = GameController.Singleton.HUD;
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

        SoundController.Singleton.PlaySound(LOSE_SFX);
    }

    private void OnExitGameButtonPressed() {
        GetTree().Quit();
    }

    private void OnBackToMenuButtonPressed() {
        menuController.CloseMenus();
        menuController.OpenMainMenu();
        menuController.GetTree().Paused = false;
        soundController.MusicMuted = false;
        soundController.StopAllAmbiance();
        soundController.ForceStopSong();
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

        foreach (Node child in hudChildren)
            if (child is Control controlChild)
                controlChild.Modulate = new Color(1, 1, 1, 1 - easedT);
    }
}