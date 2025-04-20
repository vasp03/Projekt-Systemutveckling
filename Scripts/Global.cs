using Godot;

namespace Goodot15.Scripts;

public partial class Global : Node {
    public static Vector2 CardOverlappingOffset { get; private set; } = new Vector2(0, -20);
    public static Vector2 CraftButtonOffset { get; private set; } = new Vector2(0, -110);
}