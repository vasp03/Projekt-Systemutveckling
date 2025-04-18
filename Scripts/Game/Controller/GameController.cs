using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;
using Vector2 = Godot.Vector2;

namespace Goodot15.Scripts.Game.Controller;

public partial class GameController : Node2D {
    private readonly List<int> numberList = new();

    private readonly IList<IGameManager> managers = [];
    
    private CardController CardController => this.GetManager<CardController>();
    private Goodot15.Scripts.Game.Controller.MenuController menuController => this.GetManager<MenuController>();

    public override void _Ready() {
        this.RegisterDefaultManagers();
        
        ConfigureDefaultManagers();
    }

    public T RegisterManager<T>(T manager) where T : IGameManager {
        if (this.managers.Any(e=>e.GetType()==manager.GetType())) 
            throw new InvalidOperationException("Cannot register a duplicate manager");
        this.managers.Add(manager);
        
        return manager;
    }

    public T GetManager<T>() where T : IGameManager {
        if (this.managers.FirstOrDefault(e => e is T) is not T manager) 
            throw new InvalidOperationException($"Cannot get manager of type {typeof(T).FullName} as it is not registered");
        return manager;
    }
    
    private void RegisterDefaultManagers() {
        RegisterManager(new MouseController(this));
        RegisterManager(new CraftingController(this));
        RegisterManager(new CardController(this));
        RegisterManager(new CardCreationHelper(this));
        RegisterManager(new GameEventManager(this));
        RegisterManager(GetNode<MenuController>("/root/MenuController"));
    }

    private void ConfigureDefaultManagers() {
        GetManager<MenuController>().SetNodeController(this);
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey eventKey && eventKey.Pressed) {
            switch (eventKey.Keycode) {
                case Key.Escape:
                    menuController.OpenPauseMenu();
                    Visible = false; // Hide the game scene
                    break;
                case Key.Space:
                    CardController.CreateCard("Random", Vector2.One * 100);
                    break;
                case Key.D:
                    CardController.PrintCardsNeighbours();
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

    public override void _Process(double delta) {
        foreach (IGameManager gameManager in managers)
        {
            ITickable? tickableGameManager = gameManager as ITickable;
            tickableGameManager?.PreTick();
            tickableGameManager?.PostTick();
        }
    }

    public Vector2 GetMousePosition() {
        return GetGlobalMousePosition();
    }

    public void MultipleNumberInput(int number) {
        GD.Print("Number pressed: " + number);
        numberList.Add(number);

        if (numberList.Count >= 2) {
            StringBuilder numbers = new();
            for (int i = 0; i < numberList.Count; i++) {
                GD.PrintS(numberList[i] + "-");
                numbers.Append(numberList[i]);
            }

            // Create a new card with the numbers in the list
            CardController.CreateCard(numbers.ToString(), new Vector2(100, 100));

            numberList.Clear();
        }
    }
}