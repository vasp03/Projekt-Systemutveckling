using Godot;

namespace Goodot15.Scripts;

public partial class Global : Node {
    public int Money { get; set; } = 0;
    [Signal]
    public delegate void MoneyChangedEventHandler(int newMoney);
    public void AddMoney(int newMoney) {
        Money += newMoney;
        EmitSignal("MoneyChanged", Money);
    }

    // Method to spend money
    public bool SpendMoney(int newMoney) {
        if (Money >= newMoney) {
            Money -= newMoney;
            EmitSignal("MoneyChanged", Money);
            return true;
        }
        return false;
    }
}