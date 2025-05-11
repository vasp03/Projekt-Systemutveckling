using System.Collections.Generic;
using System.Text;
using Godot;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller;

public partial class GameController : Node2D {
    // TODO: Organisera, sort and use regions to split each variable purpose
    // TODO: camelCase
    private readonly List<int> numberList = new();
    private CardController cardController;
    private DayTimeController DayTimeController;
    private DayTimeEvent DayTimeEvent;
    private GameEventManager GameEventManager;
    private MenuController menuController;
    private MouseController mouseController;
    private SoundController soundController;
    
    // TODO: Move to DayTimeController?
    public Label TimeLabel { get; private set; }

    // TODO: Annotate with `?` (GameController?)
    public static GameController Singleton => (Engine.GetMainLoop() as SceneTree).CurrentScene as GameController;

    public CameraController CameraController { get; private set; }

    public override void _Ready() {
        // TODO: Split constructors and initialization into 2 separate methods
        mouseController = new MouseController(this);
        cardController = new CardController(this, mouseController);
        DayTimeController = new DayTimeController(this);
        GameEventManager = new GameEventManager(this);


        soundController = GetNode<SoundController>("/root/SoundController");
        soundController.PlayGameMusic();

        menuController = GetNode<MenuController>("/root/MenuController");
        menuController.SetNodeController(this);

        CameraController = new CameraController();

        DayTimeEvent = new DayTimeEvent(this);
        DayTimeController.AddCallback(DayTimeEvent);

        TimeLabel = GetNode<Label>("CanvasLayer/DayTimeLabel");
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey eventKey && eventKey.Pressed) {
            switch (eventKey.Keycode) {
                case Key.Escape:
                    menuController.OpenPauseMenu();
                    DayTimeController.SetPaused(true);
                    soundController.MusicMuted = true;
                    Visible = false;
                    break;
                case Key.Space:
                    cardController.CreateCard("Random", Vector2.One * 100);
                    break;
                case Key.Key0:
                case Key.Key1:
                case Key.Key2:
                case Key.Key3:
                case Key.Key4:
                case Key.Key5:
                case Key.Key6:
                case Key.Key7:
                case Key.Key8:
                case Key.Key9:
                    MultipleNumberInput((int)eventKey.Keycode - (int)Key.Key0);
                    break;
            }
        } else if (@event is InputEventMouseButton mouseButton) {
            if (mouseButton.Pressed)
                cardController.LeftMouseButtonPressed();
            else
                cardController.LeftMouseButtonReleased();
        }
    }

    public Vector2 GetMousePosition() {
        return GetGlobalMousePosition();
    }

    public void MultipleNumberInput(int number) {
        numberList.Add(number);

        if (numberList.Count >= 2) {
            StringBuilder numbers = new();
            for (int i = 0; i < numberList.Count; i++) numbers.Append(numberList[i]);

            // Create a new card with the numbers in the list
            cardController.CreateCard(numbers.ToString(), new Vector2(100, 100));

            numberList.Clear();
        }
    }

    // Set the scene darknes
    public void SetSceneDarkness(float darkness) {
        // Clamp darkness between 0 (bright) and 1 (completely dark)
        darkness = Mathf.Clamp(darkness, 0, 1);

        // Get Canvaslayer and sprite2d child
        CanvasLayer canvasLayer = GetNode<CanvasLayer>("CanvasLayer");

        if (canvasLayer == null) {
            GD.PrintErr("CanvasLayer not found.");
            return;
        }

        Sprite2D sprite = canvasLayer.GetNode<Sprite2D>("Sprite2D");

        if (sprite == null) {
            GD.PrintErr("Darkness sprite not found.");
            return;
        }

        sprite.Modulate = new Color(0, 0, 0, 1 - darkness); // Set the color to black with the specified alpha value
    }

    public override void _PhysicsProcess(double delta) {
        // TODO: Use polymorphism to remove the need of manually calling methods; Iterate through list and calling the interface instead
        DayTimeController.PreTick(delta);
        GameEventManager.PostTick();
        CameraController.PostTick();
    }

    
    // TODO: Confusing placement? Isn't the game controller the one that should hold if the game is paused
    // TODO: convert to property instead of method
    public bool IsPaused() {
        return menuController.IsPaused();
    }

    internal Vector2 GetRandomPositionWithinScreen() {
        // Get the size of the screen
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;

        // Generate a random position within the screen bounds
        float x = (float)GD.RandRange(0, screenSize.X);
        float y = (float)GD.RandRange(0, screenSize.Y);

        return new Vector2(x, y);
    }

    #region Getters
    
    // TODO: Make use of properties instead of methods
    public CardController GetCardController() {
        return cardController;
    }

    public MenuController GetMenuController() {
        return menuController;
    }

    public MouseController GetMouseController() {
        return mouseController;
    }

    public SoundController GetSoundController() {
        return soundController;
    }

    public DayTimeController GetDayTimeController() {
        return DayTimeController;
    }

    #endregion Getters
}