using System;
using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game;

public partial class Global : Node {
    private static MouseController mouseController = new();
    private int money;
    public static Global Singleton { get; private set; }

    public static MouseController MouseController {
        get {
            if (mouseController == null) mouseController = new MouseController();
            return mouseController;
        }
        private set { }
    }

    public int Money {
        get => money;
        private set {
            money = value;
            MoneyChanged?.Invoke(money);
        }
    }

    public override void _Ready() {
        Singleton = this;
    }

    public void AddMoney(int amount) {
        Money += amount;
        GD.Print($"[Global] Money updated: {Money}");
    }

    public event Action<int> MoneyChanged;
}