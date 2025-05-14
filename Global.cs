using System;
using Godot;

namespace Goodot15.Scripts.Game;

public partial class Global : Node {
    private int money;
    public static Global Singleton { get; private set; }

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