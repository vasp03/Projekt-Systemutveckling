using Godot;

namespace Goodot15.Scripts.Game.Model.Interface;

public interface ICardAnimateable {
    void Render(Sprite2D cardSprite, double delta);
}