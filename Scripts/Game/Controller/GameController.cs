using System.Collections.Generic;
using System.Text;
using System.Threading;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Vector2 = Godot.Vector2;

public partial class GameController : Node2D {
	private readonly List<int> numberList = new();
	private CardController cardController;
	private MenuController menuController;
	private MouseController mouseController;
	private SoundController soundController;
	private DayTimeController DayTimeController;
	private DayTimeEvent DayTimeEvent;

	public override void _Ready() {
		mouseController = new MouseController(this);
		cardController = new CardController(this, mouseController);
		DayTimeController = new DayTimeController();

		soundController = GetNode<SoundController>("/root/SoundController");
		soundController.PlayGameMusic();

		menuController = GetNode<MenuController>("/root/MenuController");
		menuController.SetNodeController(this);

		DayTimeEvent = new DayTimeEvent(this);
		DayTimeController.AddCallback(DayTimeEvent);
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
			for (int i = 0; i < numberList.Count; i++) {
				numbers.Append(numberList[i]);
			}

			// Create a new card with the numbers in the list
			cardController.CreateCard(numbers.ToString(), new Vector2(100, 100));

			numberList.Clear();
		}
	}

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

		GD.Print($"Scene darkness set to {darkness}");
	}

	public override void _PhysicsProcess(double delta) {
		DayTimeController.PreTick(delta);
	}

	public bool IsPaused() {
		return menuController.IsPaused();
	}
}
