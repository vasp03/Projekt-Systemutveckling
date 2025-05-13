using Godot;
using System;

namespace Goodot15.Scripts.Game;

public partial class Global : Node {
    public static Global Singleton { get; private set; }

    public override void _Ready() {
        Singleton = this;
    }

    private int money;

    public int Money {
        get => money;
        private set {
            money = value;
            MoneyChanged?.Invoke(money);
        }
    }

    public void AddMoney(int amount) {
        Money += amount;
        GD.Print($"[Global] Money updated: {Money}");
    }

    public event Action<int> MoneyChanged;
}