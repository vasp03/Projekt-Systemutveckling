using System.Collections.Generic;
using System.Text;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller;

public partial class GameController : Node2D {
    private readonly List<int> numberList = new();

    /// <summary>
    ///     Gets the single GameController of the game (if the correct scene is loaded); Null if the game has no GameController
    ///     at the given point of execution
    /// </summary>
    public static GameController? Singleton => (Engine.GetMainLoop() as SceneTree).CurrentScene as GameController;

    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey eventKey && eventKey.Pressed) {
            switch (eventKey.Keycode) {
                case Key.Escape:
                    MenuController.OpenPauseMenu();

                    if (GameEventManager.EventInstance<DayTimeEvent>() is IPausable pausable) pausable.SetPaused(true);

                    SoundController.MusicMuted = true;
                    HideHUD();
                    Visible = false;
                    break;
                case Key.Space:
                    CardController.CreateCard("Random", Vector2.One * 100);
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
                CardController.LeftMouseButtonPressed();
            else
                CardController.LeftMouseButtonReleased();
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
            CardController.CreateCard(numbers.ToString(), new Vector2(100, 100));

            numberList.Clear();
        }
    }


    public override void _PhysicsProcess(double delta) {
        GameEventManager.PostTick();
        CameraController.PostTick();
    }

    public bool IsPaused() {
        return GetTree().Paused;
    }

    internal Vector2 GetRandomPositionWithinScreen() {
        // Get the size of the screen
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;

        // Generate a random position within the screen bounds
        float x = (float)GD.RandRange(0, screenSize.X);
        float y = (float)GD.RandRange(0, screenSize.Y);

        return new Vector2(x, y);
    }

    #region Controller references

    public CardController CardController { get; private set; }
    public GameEventManager GameEventManager { get; private set; }
    public MenuController MenuController { get; private set; }
    public MouseController MouseController { get; private set; }
    public SoundController SoundController { get; private set; }
    public CameraController CameraController { get; private set; }
    public HUD HUD { get; private set; }

    #endregion

    #region Sell Mode

    public bool SellModeActive { get; private set; }

    public void ToggleSellMode() {
        SellModeActive = !SellModeActive;
        GD.Print($"Sell mode is now {(SellModeActive ? "ON" : "OFF")}");
    }

    public void SetSellMode(bool active) {
        SellModeActive = active;
        GD.Print($"Sell mode set to {(SellModeActive ? "ON" : "OFF")}");
    }

    #region HUD visibility

    public void HideHUD() {
        HUD.Visible = false;
    }

    public void ShowHUD() {
        HUD.Visible = true;
    }

    #endregion

    #endregion

    #region Initialization

    public override void _Ready() {
        ConfigureControllers();
        SetupControllers();
        SetupUI();
    }

    private void SetupControllers() {
        MouseController = new MouseController(this);
        CardController = new CardController(this, MouseController);
        GameEventManager = new GameEventManager(this);
        CameraController = new CameraController();
    }

    private void SetupUI() {
        HUD = GetNode<HUD>("HUD");
    }

    private void ConfigureControllers() {
        SoundController = GetNode<SoundController>("/root/SoundController");
        SoundController.PlayGameMusic();

        MenuController = GetNode<MenuController>("/root/MenuController");
        MenuController.SetGameController(this);
    }

    #endregion Initialization
    #region Callbacks related

    private readonly IList<IPausable> pausedCallbacks = [];

    public void CallPausedCallbacks(bool isPaused) {
        if (pausedCallbacks is null) return;

        foreach (IPausable callback in pausedCallbacks) callback.SetPaused(isPaused);
    }

    public void AddPauseCallback(IPausable callback) {
        pausedCallbacks.Add(callback);
    }

    public void RemovePauseCallback(IPausable callback) {
        pausedCallbacks.Remove(callback);
    }

    #endregion Callbacks related
}