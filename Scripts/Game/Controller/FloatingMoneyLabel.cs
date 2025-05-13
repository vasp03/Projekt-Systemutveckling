using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class FloatingMoneyLabel : CanvasLayer {
    public override void _Ready() {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("float_and_fade");
    }
}