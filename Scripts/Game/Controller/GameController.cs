using System.Collections.Generic;
using System.Text;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller;

public partial class GameController : Node2D {
	private readonly List<int> numberList = new();
    #region Controller references
	public CardController CardController { get; private set; }
	public DayTimeController DayTimeController { get; private set; }
	public DayTimeEvent DayTimeEvent { get; private set; }
	public GameEventManager GameEventManager { get; private set; }
	public MenuController MenuController { get; private set; }
	public MouseController MouseController { get; private set; }
	public SoundController SoundController { get; private set; }
    public CameraController CameraController { get; private set; }
    #endregion
	public Label TimeLabel { get; private set; }

    /// <summary>
    /// Gets the single GameController of the game (if the correct scene is loaded); Null if the game has no GameController at the given point of execution
    /// </summary>
	public static GameController? Singleton => (Engine.GetMainLoop() as SceneTree).CurrentScene as GameController;

    #region Initialization
	public override void _Ready() {
		SetupControllers();
        ConfigureControllers();
        
		TimeLabel = GetNode<Label>("CanvasLayer/DayTimeLabel");
	}

    private void SetupControllers() {
        MouseController = new MouseController(this);
        CardController = new CardController(this, MouseController);
        DayTimeController = new DayTimeController(this);
        GameEventManager = new GameEventManager(this);
        CameraController = new CameraController();
    }

    private void ConfigureControllers() {
        SoundController = GetNode<SoundController>("/root/SoundController");
        SoundController.PlayGameMusic();

        MenuController = GetNode<MenuController>("/root/MenuController");
        MenuController.SetGameController(this);



        DayTimeEvent = new DayTimeEvent(this);
        DayTimeController.AddDayTimeCallback(DayTimeEvent);
    }
    #endregion Initialization

	public override void _Input(InputEvent @event) {
		if (@event is InputEventKey eventKey && eventKey.Pressed) {
			switch (eventKey.Keycode) {
				case Key.Escape:
					MenuController.OpenPauseMenu();
					DayTimeController.SetPaused(true);
					SoundController.MusicMuted = true;
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
		DayTimeController.PreTick(delta);
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
}
