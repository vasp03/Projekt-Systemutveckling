using Godot;

namespace Goodot15.Scripts.Game.Model.Data;

public abstract class CardRecord {
    /// <summary>
    /// Card Type Name, full class name used for reconstructing the class
    /// </summary>
    public string CardTypeName { get; set; }
    public Vector2 CardPosition { get; set; }
    public int ZIndex { get; set; }
}