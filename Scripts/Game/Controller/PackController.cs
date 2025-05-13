using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Div;

namespace Goodot15.Scripts.Game.Controller;

public partial class PackController : HBoxContainer {
    private readonly List<CardPack> _availablePacks = new();
    private CardController _cardController;
    private Global _global;

    private HBoxContainer _packContainer;

    [Export] public PackedScene PackButtonScene;
    [Export] public NodePath PackContainerPath = "HUDRoot/PackContainer";
    public GameController GameController { get; set; }

    public override void _Ready() {
        _global = GetNode<Global>("/root/Global");
    }

    public void Init() {
        _cardController = GameController.GetCardController();
        RegisterPacks();
        DisplayAvailablePacks();
    }


    private void RegisterPacks() {
        List<string> starterCommons = new() { "Villager", "Tree", "Bush", "Stone", "Stick", "Stick" };
        List<string> starterRares = new();
        CardPack starterPack = new("Starter Pack", 0, starterCommons, starterRares);

        List<string> materialCommons = new() { "Wood", "Stone", "Leaves", "Sand", "Stick", "Water", "Brick" };
        List<string> materialRares = new() { "Clay", "Glass", "Planks" };
        CardPack materialPack = new("Material Pack", 80, materialCommons, materialRares);

        List<string> foodCommons = new() { "Berry", "Apple", "Fish", "Meat" };
        List<string> foodRares = new() { "Jam", "CookedFish", "CookedMeat" };
        CardPack foodPack = new("Food Pack", 120, foodCommons, foodRares);

        List<string> buildingCommons = new() { "Field", "Campfire", "House" };
        List<string> buildingRares = new() { "Greenhouse" };
        CardPack buildingPack = new("Building Pack", 200, buildingCommons, buildingRares);

        _availablePacks.Add(starterPack);
        _availablePacks.Add(materialPack);
        _availablePacks.Add(foodPack);
    }

    private void DisplayAvailablePacks() {
        if (_global == null) {
            GD.PrintErr("PackController: _global is null.");
            return;
        }

        if (PackButtonScene == null) {
            GD.PrintErr("PackController: PackButtonScene not assigned.");
            return;
        }

        foreach (Node child in GetChildren()) {
            child.QueueFree();
        }

        foreach (CardPack pack in _availablePacks) {
            PackButton button = (PackButton)PackButtonScene.Instantiate();
            button.SetPack(pack);
            button.PackClicked += OnPackClicked;

            bool isAffordable = _global.Money >= pack.Cost;
            button.Disabled = !isAffordable;
            button.Modulate = isAffordable
                ? Colors.White
                : new Color(1, 1, 1, 0.5f);
            button.SetPriceColor(isAffordable
                ? Colors.White
                : Colors.Red);

            AddChild(button);
        }
    }

    private void OnPackClicked(CardPack pack) {
        if (_global.Money < pack.Cost) {
            GD.Print("Not enough money.");
            return;
        }

        _global.AddMoney(-pack.Cost);

        List<string> cardsToSpawn;

        if (pack.Name == "Starter Pack") {
            cardsToSpawn = new List<string>();
            cardsToSpawn.AddRange(pack.CommonCards);
            cardsToSpawn.AddRange(pack.RareCards);
        } else {
            cardsToSpawn = GeneratePackContents(pack);
        }

        foreach (string cardName in cardsToSpawn) {
            Vector2 randomPosition = new(
                GD.RandRange(100, 1100),
                GD.RandRange(100, 550)
            );
            _cardController.CreateCard(cardName, randomPosition);
        }

        if (pack.Name == "Starter Pack") {
            _availablePacks.Remove(pack);
            UnlockAdditionalPacks();
        }
    }

    private void UnlockAdditionalPacks() {
        DisplayAvailablePacks();
    }

    public void RefreshPackStates(int newMoney) {
        foreach (Node child in GetChildren()) {
            if (child is not PackButton button) continue;
            if (button.Pack == null) continue; // safety check

            bool isAffordable = _global.Money >= button.Pack.Cost;
            button.Disabled = !isAffordable;
            button.Modulate = isAffordable
                ? Colors.White
                : new Color(1, 1, 1, 0.5f);
            button.SetPriceColor(isAffordable
                ? Colors.White
                : Colors.Red);
        }
    }

    private List<string> GeneratePackContents(CardPack pack) {
        List<string> selectedCards = new();
        RandomNumberGenerator rng = new();
        rng.Randomize();

        int cardCount = rng.RandiRange(3, 5);
        for (int i = 0; i < cardCount; i++) {
            bool isRare = rng.Randf() < 0.1f && pack.RareCards.Count > 0;

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