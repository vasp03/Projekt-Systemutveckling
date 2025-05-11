using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Div;

namespace Goodot15.Scripts.Game.Controller;

public partial class PackController : Node {
    private GameController _gameController;
    private Global _global;
    private CardController _cardController;

    private VBoxContainer _packContainer;

    [Export] public PackedScene PackButtonScene;
    [Export] public NodePath PackContainerPath = "HUDRoot/PackContainer";

    private List<CardPack> _availablePacks = new();

    public override void _Ready() {
        _gameController = GetNode<GameController>("Scripts/Game/Controller/PackController.cs");
        _global = GetNode<Global>("/root/Global");
        _cardController = _gameController.GetCardController();
        _packContainer = GetNode<VBoxContainer>(PackContainerPath);

        RegisterPacks();
        DisplayAvailablePacks();
    }

    private void RegisterPacks() {
        List<string> starterCommons = new() { "Villager", "Tree", "Bush", "Stone", "Stick", "Stick" };
        List<string> starterRares = new();
        CardPack starterPack = new("Starter Pack", 0, starterCommons, starterRares);

        List<string> foodCommons = new() { "Berry", "Bush", "Stick" };
        List<string> foodRares = new() { "Jam", "Fish" };
        CardPack foodPack = new("Food Pack", 20, foodCommons, foodRares);

        _availablePacks.Add(starterPack);
        _availablePacks.Add(foodPack);
    }

    private void DisplayAvailablePacks() {
        foreach (Node child in _packContainer.GetChildren()) {
            child.QueueFree();
        }

        foreach (CardPack pack in _availablePacks) {
            PackButton button = (PackButton)PackButtonScene.Instantiate();
            button.SetPack(pack);
            button.PackClicked += OnPackClicked;

            // Check if the pack is affordable
            bool isAffordable = _global.Money >= pack.Cost;
            button.Disabled = !isAffordable;
            button.Modulate = isAffordable ? Colors.White : new Color(1, 1, 1, 0.5f); // Grey out if not affordable
            button.SetPriceColor(isAffordable ? Colors.White : Colors.Red);

            _packContainer.AddChild(button);
        }
    }

    private void OnPackClicked(CardPack pack) {
        if (_global.Money < pack.Cost) {
            GD.Print("Not enough money.");
            return;
        }

        _global.AddMoney(-pack.Cost);

        List<string> cardsToSpawn = GeneratePackContents(pack);
        foreach (string cardName in cardsToSpawn) {
            Vector2 randomPosition = new Vector2(
                GD.RandRange(400, 600), // Random X near the center
                GD.RandRange(300, 500)  // Random Y near the center
            );
            _cardController.CreateCard(cardName, randomPosition);
        }
        
        if (pack.Name == "Starter Pack") {
            _availablePacks.Remove(pack); // Remove starter pack
            UnlockAdditionalPacks();
        }
    }

    private void UnlockAdditionalPacks() {
        DisplayAvailablePacks();
    }
    
    public void RefreshPackStates(int newMoney) {
        foreach (PackButton button in _packContainer.GetChildren()) {
            bool isAffordable = _global.Money >= button.Pack.Cost;
            button.Disabled = !isAffordable;
            button.Modulate = isAffordable ? Colors.White : new Color(1, 1, 1, 0.5f);
            button.SetPriceColor(isAffordable ? Colors.White : Colors.Red);
        }
    }
    
    private List<string> GeneratePackContents(CardPack pack) {
        List<string> selectedCards = new();
        RandomNumberGenerator rng = new();
        rng.Randomize();

        int cardCount = rng.RandiRange(3, 5);
        for (int i = 0; i < cardCount; i++) {
            bool isRare = rng.Randf() < 0.5f && pack.RareCards.Count > 0;

            if (isRare) {
                int index = rng.RandiRange(0, pack.RareCards.Count - 1);
                selectedCards.Add(pack.RareCards[index]);
            } else if (pack.CommonCards.Count > 0) {
                int index = rng.RandiRange(0, pack.CommonCards.Count - 1);
                selectedCards.Add(pack.CommonCards[index]);
            }
        }

        return selectedCards;
    }
}