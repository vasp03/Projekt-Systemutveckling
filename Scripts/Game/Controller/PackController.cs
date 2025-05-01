using System.Collections.Generic;
using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class PackController : Node {
    private GameController _gameController;
    private VBoxContainer  _packContainer;  
    private bool _starterOpened = false;

    [Export] public PackedScene PackButtonScene;
    [Export] public PackedScene StarterPackScene;
    [Export] public List<PackedScene> ThemedPackScene;

    public override void _Ready() {
        _gameController = GetNode<GameController>("/root/GameController");
        
    }
}