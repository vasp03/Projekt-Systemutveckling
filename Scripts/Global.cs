using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts;

public partial class Global : Node {
    public delegate void MoneyChangedEvent(int newMoney);

    private int money;
    public static Global Singleton { get; private set; }

    public static MouseController MouseController { get; } = new();

    public int Money {
        get => money;
        set {
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

    public event MoneyChangedEvent MoneyChanged;
}