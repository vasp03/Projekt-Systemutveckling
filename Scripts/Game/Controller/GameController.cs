using System.Collections.Generic;
using System.Text;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller;

public partial class GameController : Node2D {
	private const string SELL_MODE_ACTIVE_SFX = "General Sounds/Buttons/sfx_sounds_button7.wav";
	private const string SELL_MODE_DEACTIVE_SFX = "General Sounds/Buttons/sfx_sounds_button7_rev.wav";

	private readonly List<int> numberList = new();

	/// <summary>
	///     Gets the single GameController of the game (if the correct scene is loaded); Null if the game has no GameController
	///     at the given point of execution
	/// </summary>
	public static GameController? Singleton => (Engine.GetMainLoop() as SceneTree).CurrentScene as GameController;


	public override void _Input(InputEvent @event) {
		if (!IsInstanceValid(MenuController)) MenuController = GetNode<MenuController>("/root/MenuController");

		if (MenuController.Singleton.IsGameOverMenuActive) return;

		switch (@event) {
			case InputEventKey eventKey when eventKey.Pressed:
				switch (eventKey.Keycode) {
					case Key.Escape:
						MenuController.OpenPauseMenu();

						if (GameEventManager.EventInstance<DayTimeEvent>() is IPausable pausable)
							pausable.SetPaused(true);

						SoundController.MusicMuted = true;
						HideHUD();
						Visible = false;
						break;
					case Key.O:
						MenuController.OpenGameOverMenu();
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
						if (SettingsManager.Singleton.CheatMode)
							MultipleNumberInput((int)eventKey.Keycode - (int)Key.Key0);
						break;
					case Key.S:
						ToggleSellMode();
						break;
					case Key.Q:
						MenuController.QuickOpenGuideMenu();

						if (GameEventManager.EventInstance<DayTimeEvent>() is IPausable pausable3)
							pausable3.SetPaused(true);

						SoundController.Singleton.MusicMuted = true;
						HideHUD();
						Visible = false;
						break;
					case Key.F:
						for (int i = 0; i < 100; i++) CardController.CreateCard("Fire", new Vector2(200, 200));
						break;
					case Key.D:
						SoundController.LogAllAmbiancePlaying();
						break;
					case Key.F1:
						if (SettingsManager.Singleton.CheatMode)
							GameEventManager.PostEvent(GameEventManager.EventInstance<BoulderEvent>());
						break;
					case Key.F2:
						if (SettingsManager.Singleton.CheatMode)
							GameEventManager.PostEvent(GameEventManager.EventInstance<ColdNightEvent>());
						break;
					case Key.F3:
						if (SettingsManager.Singleton.CheatMode)
							GameEventManager.PostEvent(GameEventManager.EventInstance<FireEvent>());
						break;
					case Key.F4:
						if (SettingsManager.Singleton.CheatMode)
							GameEventManager.PostEvent(GameEventManager.EventInstance<MeteoriteEvent>());
						break;
					case Key.F5:
						if (SettingsManager.Singleton.CheatMode)
							GameEventManager.PostEvent(GameEventManager.EventInstance<RainEvent>());
						break;
					case Key.F6:
						if (SettingsManager.Singleton.CheatMode)
							GameEventManager.PostEvent(GameEventManager.EventInstance<NatureResourceEvent>());
						break;
					case Key.F7:
						if (SettingsManager.Singleton.CheatMode) Global.Singleton.AddMoney(500);
						break;
				}

				break;
			case InputEventMouseButton mouseButton when mouseButton.Pressed:
				CardController.LeftMouseButtonPressed();
				break;
			case InputEventMouseButton mouseButton:
				CardController.LeftMouseButtonReleased();
				break;
		}
	}

	public Vector2 GetMousePosition() {
		return GetGlobalMousePosition();
	}

	private void MultipleNumberInput(int number) {
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
		GameEventManager.PostTick(delta);
		CameraController.PostTick(delta);
	}

	public bool IsPaused() {
		return GetTree().Paused;
	}

	internal Vector2 NextRandomPositionOnScreen() {
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
	public MouseController MouseController { get; } = Global.MouseController;
	public SoundController SoundController { get; private set; }
	public CameraController CameraController { get; private set; }

	#endregion

	#region Sell Mode

	private TextureRect SellModeLabel => GetNode<TextureRect>("HUD/HUDRoot/SellModeLabel");

	private bool sellModeActive;

	/// <summary>
	///     Determines if the sell mode is active or not.
	/// </summary>
	public bool SellModeActive {
		get => sellModeActive;
		set {
			sellModeActive = value;

			SellModeUpdated();
		}
	}

	/// <summary>
	///     Toggles sell mode on and off.
	/// </summary>
	public void ToggleSellMode() {
		SellModeActive = !SellModeActive;
		GD.Print($"Sell mode is now {(SellModeActive ? "ON" : "OFF")}");
		Global.MouseController.SetSellMode(SellModeActive);
		HUD.sellModeButton?.UpdateIcon();
	}

	private void SellModeUpdated() {
		if (!sellModeActive)
			CardController.HideCardValue();

		SellModeLabel.Visible = sellModeActive;

		SoundController.Singleton.PlaySound(SellModeActive
			? SELL_MODE_ACTIVE_SFX
			: SELL_MODE_DEACTIVE_SFX);

		Global.MouseController.SetSellMode(SellModeActive);
		HUD.sellModeButton?.UpdateIcon();
		// GD.Print($"Sell mode set to {(SellModeActive ? "ON" : "OFF")}");
	}

	#region HUD visibility

	public HUD HUD { get; private set; }

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

		SellModeActive = false;
	}

	private void SetupControllers() {
		CardController = new CardController(this, MouseController, MenuController);
		GameEventManager = new GameEventManager(this);
		CameraController = new CameraController();
	}

	private void SetupUI() {
		HUD = GetNode<HUD>("HUD");
	}

	private void ConfigureControllers() {
		SoundController = GetNode<SoundController>("/root/SoundController");


		MenuController = GetNode<MenuController>("/root/MenuController");
		// MenuController.SetGameController(this);

		Global.Singleton.Money = 0;
	}

	#endregion Initialization

	#region Callbacks related

	private readonly IList<IPausable> pausedCallbacks = [];

	public void CallPausedCallbacks(bool isPaused) {
		if (pausedCallbacks is null) return;

		foreach (IPausable callback in pausedCallbacks) callback.SetPaused(isPaused);
	}

	public void RegisterPauseCallback(IPausable callback) {
		pausedCallbacks.Add(callback);
	}

	public void RemovePauseCallback(IPausable callback) {
		pausedCallbacks.Remove(callback);
	}

	#endregion Callbacks related
}
