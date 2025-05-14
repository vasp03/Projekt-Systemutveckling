using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Div;

namespace Goodot15.Scripts.Game.View;

public partial class PackController : HBoxContainer {
    private readonly List<CardPack> availablePacks = new();
    private Global global;

    [Export] public PackedScene PackButtonScene;

    public override void _Ready() {
        global = Global.Singleton;
        Init();
    }

    public void Init() {
        RegisterPacks();
        DisplayAvailablePacks();
    }

    private void RegisterPacks() {
        List<string> starterCommons = new() { "Village", "Tree", "Bush", "Stone", "Stick", "Stick" };
        List<string> starterRares = new();
        availablePacks.Add(new CardPack("Starter Pack", 0, starterCommons, starterRares));

        List<string> materialCommons = new() { "Wood", "Stone", "Leaves", "Sand", "Stick", "Water", "Brick" };
        List<string> materialRares = new() { "Clay", "Glass", "Planks" };
        availablePacks.Add(new CardPack("Material Pack", 80, materialCommons, materialRares));

        List<string> foodCommons = new() { "Berry", "Apple", "Fish", "Meat" };
        List<string> foodRares = new() { "Jam", "CookedFish", "CookedMeat" };
        availablePacks.Add(new CardPack("Food Pack", 120, foodCommons, foodRares));

        List<string> buildingCommons = new() { "Field", "Campfire", "House" };
        List<string> buildingRares = new() { "Greenhouse" };
        // availablePacks.Add(new CardPack("Building Pack", 200, buildingCommons, buildingRares));
    }

    private void DisplayAvailablePacks() {
        if (global is null || PackButtonScene is null) return;

        foreach (Node child in GetChildren()) {
            child.QueueFree();
        }

        foreach (CardPack pack in availablePacks) {
            PackButton button = PackButtonScene.Instantiate<PackButton>();
            button.SetPack(pack);
            button.PackClicked += OnPackClicked;

            bool isAffordable = global.Money >= pack.Cost;
            button.Disabled = !isAffordable;
            button.Modulate = isAffordable ? Colors.White : new Color(1, 1, 1, 0.5f);
            button.SetPriceColor(isAffordable ? Colors.White : Colors.Red);

            AddChild(button);
        }
    }

    private void OnPackClicked(CardPack pack) {
        if (global.Money < pack.Cost) {
            GD.Print("Not enough money.");
            return;
        }

        global.AddMoney(-pack.Cost);
        
        ShowFloatingMoneyLabel(-pack.Cost);

        List<string> cardsToSpawn = pack.Name == "Starter Pack"
            ? new List<string>(pack.CommonCards).Concat(pack.RareCards).ToList()
            : GeneratePackContents(pack);

        foreach (string cardName in cardsToSpawn) {
            Vector2 randomPos = GameController.Singleton.GetRandomPositionWithinScreen();
            GameController.Singleton.CardController.CreateCard(cardName, randomPos);
        }

        if (pack.Name == "Starter Pack") {
            availablePacks.Remove(pack);
            UnlockAdditionalPacks();
        }
    }

    private void UnlockAdditionalPacks() {
        DisplayAvailablePacks();
    }

    public void RefreshPackStates(int newMoney) {
        foreach (Node child in GetChildren()) {
            if (child is not PackButton button || button.Pack is null) continue;

            bool isAffordable = global.Money >= button.Pack.Cost;
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
    
    private void ShowFloatingMoneyLabel(int amount) {
        var labelScene = GD.Load<PackedScene>("res://Scenes/ProgressBars/FloatingMoneyLabel.tscn");
        var floatingLabel = labelScene.Instantiate<FloatingMoneyLabel>();
        floatingLabel.SetAmount(amount);

        floatingLabel.Position = GameController.Singleton.GetMousePosition();
        GameController.Singleton.HUD.FloatingMoneyRoot.AddChild(floatingLabel);
    }
}
