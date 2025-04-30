using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller;

public partial class GameController : Node2D {
    private static readonly int a = 0;

    private readonly IDictionary<Type,IGameManager> managers = new Dictionary<Type,IGameManager>();
    private readonly List<int> numberList = [];

    private CardManager CardManager => GetManager<CardManager>();
    private MenuManager MenuManager => GetManager<MenuManager>();

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
        foreach (IGameManager gameManager in managers.Values) gameManager.OnUnload();
    }

    private void PostReadyEventForManagers() {
        foreach (IGameManager manager in managers.Values) manager.OnReady();
    }

    public T RegisterManager<T>(T manager) where T : IGameManager {
        ArgumentNullException.ThrowIfNull(manager);
        if (managers.ContainsKey(typeof(T))) {
            throw new InvalidOperationException("Cannot register a duplicate manager");
        }
        
        GD.Print($"Registering manager {manager.GetType().FullName}");
        
        managers.Add(typeof(T), manager);
        
        return manager;
    }

    public T GetManager<T>() where T : IGameManager {
        if (!managers.TryGetValue(typeof(T), out IGameManager? manager)) {
            throw new InvalidOperationException($"Cannot get manager of type {typeof(T).FullName} as it is not registered");
        }

        GD.Print($"Returning manager {manager.GetType().FullName}");
        return (T)manager;
    }

    private void RegisterDefaultManagers() {
        RegisterManager(new SettingsManager());
        RegisterManager(new SoundManager());
        RegisterManager(new MouseManager());
        RegisterManager(new CraftingManager());
        RegisterManager(new CardCreationHelper());
        RegisterManager(new CardManager());
        RegisterManager(new GameEventManager());
        RegisterManager(new MenuManager());

        RegisterManager(new DayTimeManager());
        //		DayTimeEvent = new DayTimeEvent(this);
        //  		DayTimeController.AddCallback(DayTimeEvent);
        // 
    }

    private void ConfigureDefaultManagers() {
        GetManager<MenuManager>().SetNodeController(this);

        GetManager<DayTimeManager>().AddCallback(new DayTimeEvent());
    }

    public override void _Input(InputEvent @event) {
        switch (@event)
        {
            case InputEventKey eventKey when eventKey.Pressed:
                switch (eventKey.Keycode) {
                    case Key.Escape:
                        MenuManager.OpenPauseMenu();
                        // DayTimeController.SetPaused(true);
                        GetManager<SoundManager>().MusicMuted = true;
                        Visible = false;
                        break;
                    case Key.Space:
                        CardManager.CreateCard("Random", Vector2.One * 100);
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

                break;
            case InputEventMouseButton mouseButton: {
                if (mouseButton.Pressed)
                    CardManager.LeftMouseButtonPressed();
                else
                    CardManager.LeftMouseButtonReleased();
                break;
            }
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
            CardManager.CreateCard(numbers.ToString(), new Vector2(100, 100));

            numberList.Clear();
        }
    }

    // Set the scene darknes
    public void SetSceneDarkness(float darkness) {
        // if (GetTree().CurrentScene is null)
        //     return;
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
        foreach (IGameManager gameManager in managers.Values) {
            ITickable? tickableGameManager = gameManager as ITickable;
            tickableGameManager?.PreTick();
            tickableGameManager?.PostTick();
        }
    }

    public bool IsPaused() {
        return MenuManager.IsPaused();
    }
}