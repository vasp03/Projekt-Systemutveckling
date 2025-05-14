using Godot;

namespace Goodot15.Scripts.Game.View;

public partial class FloatingMoneyLabel : Control {
    [Export] public Label AmountLabel { get; set; }

    public override void _Ready() {
        AnimationPlayer anim = GetNode<AnimationPlayer>("AnimationPlayer");
        anim.Play("float_and_fade");
        anim.AnimationFinished += OnAnimationFinished;
    }

    public void SetAmount(int amount) {
        string prefix = amount >= 0 ? "+" : "-";
        AmountLabel.Text = $"{prefix}{Mathf.Abs(amount)}g";
        AmountLabel.Modulate = amount >= 0 ? new Color(0.2f, 1f, 0.2f) : new Color(1f, 0.3f, 0.3f);
    }

    private void OnAnimationFinished(StringName name) {
        QueueFree();
    }
}