using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller;

public partial class GameController : Node2D {
	private static int a = 0;
	
	private readonly IList<IGameManager> managers = [];
	private readonly List<int> numberList = new();

	private CardController CardController => GetManager<CardController>();
	private MenuController menuController => GetManager<MenuController>();

	public override void _Ready() {
		RegisterDefaultManagers();
	
		ConfigureDefaultManagers();
	
		PostReadyEventForManagers();
	
		FirstTimeStart();
	}
	// public GameController() {  
	//     RegisterDefaultManagers();
	//     ConfigureDefaultManagers();
	//     PostReadyEventForManagers();
	//     FirstTimeStart();
	// }

	private void FirstTimeStart() { 
		GetTree().Paused = true;
	}

	public override void _ExitTree() {
		foreach (IGameManager gameManager in managers) {
			gameManager.OnUnload();
		}
	}

	private void PostReadyEventForManagers() {
		foreach (IGameManager manager in managers) {
			manager.OnReady();
		}
	}

	public T RegisterManager<T>(T manager) where T : IGameManager {
		ArgumentNullException.ThrowIfNull(manager,"Manager cannot be null");
		if (managers.Any(e => e.GetType() == manager.GetType()))
			throw new InvalidOperationException("Cannot register a duplicate manager");
		
		GD.Print($"Registering manager {manager.GetType().FullName}");
		
		managers.Add(manager);

		return manager;
	}

	public T GetManager<T>() where T : IGameManager {
		IGameManager manager = managers.FirstOrDefault(e => e.GetType() == typeof(T));
		if (manager is null) {
			throw new InvalidOperationException($"Cannot get manager of type {typeof(T).FullName} as it is not registered");
		}
		
		GD.Print($"Returning manager {manager.GetType().FullName}");
		return (T)manager;
	}

	private void RegisterDefaultManagers() {
		GD.Print($"{a} times");
		RegisterManager(new SettingsManager());
		RegisterManager(new SoundController());
		RegisterManager(new MouseController());
		RegisterManager(new CraftingController());
		RegisterManager(new CardCreationHelper());
		RegisterManager(new CardController());
		RegisterManager(new GameEventManager());
		RegisterManager(new MenuController());

		RegisterManager(new DayTimeController());
		//		DayTimeEvent = new DayTimeEvent(this);
		//  		DayTimeController.AddCallback(DayTimeEvent);
		// 
	}

	private void ConfigureDefaultManagers() {
		GetManager<MenuController>().SetNodeController(this);

		GetManager<DayTimeController>().AddCallback(new DayTimeEvent());
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventKey eventKey && eventKey.Pressed) {
			switch (eventKey.Keycode) {
				case Key.Escape:
					menuController.OpenPauseMenu();
					// DayTimeController.SetPaused(true);
					GetManager<SoundController>().MusicMuted = true;
					Visible = false;
					break;
				case Key.Space:
					CardController.CreateCard("Random", Vector2.One * 100);
					break;
				case Key.D:
					// CardController.PrintCardsNeighbours();
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
			for (int i = 0; i < numberList.Count; i++) {
				numbers.Append(numberList[i]);
			}

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
		CanvasLayer canvasLayer = GetTree().CurrentScene.GetNode<CanvasLayer>("CanvasLayer");

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
		foreach (IGameManager gameManager in managers) {
			ITickable? tickableGameManager = gameManager as ITickable;
			tickableGameManager?.PreTick();
			tickableGameManager?.PostTick();
		}
	}

	public bool IsPaused() {
		return menuController.IsPaused();
	}
}
