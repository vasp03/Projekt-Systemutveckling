using Godot;

namespace Goodot15.Scripts;

public partial class Global : Node {
    public int Money { get; set; } = 0;
    
    public delegate void MoneyChangedEventHandler(int newMoney);

    public void AddMoney(int newMoney) {
        Money += newMoney;
        EmitSignal("MoneyChanged", newMoney);
    }

    public bool SpendMoney(int newMoney) {
        if (Money >= newMoney) {
            Money -= newMoney;
            EmitSignal("MoneyChanged", newMoney);
            return true;
        }
        return false;
    }
}