using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Div;

namespace Goodot15.Scripts.Game.View;

public partial class PackController : HBoxContainer {
    private const string PACK_OPEN_SFX = "General Sounds/Interactions/sfx_sounds_interaction17.wav";
    private const string PACK_INSUFFICIENT_MONEY_SFX = "General Sounds/Negative Sounds/sfx_sounds_error3.wav";

    private readonly IList<CardPack> availablePacks = [];

    [Export] public PackedScene PackButtonScene;

    public override void _Ready() {
        Initialize();
    }

    private void Initialize() {
        RegisterDefaultPacks();
        DisplayAvailablePacks();
    }

    private void RegisterDefaultPacks() {
        List<string> starterCommons = ["Villager", "Tree", "Bush", "Stone", "Stick", "Stick"];
        List<string> starterRares = [];
        RegisterPack(new CardPack("Starter Pack", 0, starterCommons, starterRares));

        List<string> materialCommons = ["Wood", "Stone", "Leaves", "Sand", "Stick", "Water", "Brick"];
        List<string> materialRares = ["Clay", "Glass", "Planks"];
        RegisterPack(new CardPack("Material Pack", 80, materialCommons, materialRares));

        List<string> foodCommons = ["Berry", "Apple", "Fish", "Meat"];
        List<string> foodRares = ["Jam", "CookedFish", "CookedMeat"];
        RegisterPack(new CardPack("Food Pack", 120, foodCommons, foodRares));

        List<string> buildingCommons = ["Field", "Campfire", "House"];
        List<string> buildingRares = ["Greenhouse"];
        // availablePacks.Add(new CardPack("Building Pack", 200, buildingCommons, buildingRares));
    }

    /// <summary>
    /// Registers a pack
    /// </summary>
    /// <param name="pack"></param>
    public void RegisterPack(CardPack pack) {
        availablePacks.Add(pack);
    }

    private void DisplayAvailablePacks() {
        if (Global.Singleton is null || PackButtonScene is null) return;

        foreach (Node child in GetChildren()) child.QueueFree();

        foreach (CardPack pack in availablePacks) {
            PackButton button = PackButtonScene.Instantiate<PackButton>();
            button.SetPack(pack);
            button.PackClicked += OnPackClicked;

            bool isAffordable = Global.Singleton.Money >= pack.Cost;
            // button.Disabled = !isAffordable;
            button.Modulate = isAffordable ? Colors.White : new Color(1, 1, 1, 0.5f);
            button.SetPriceColor(isAffordable ? Colors.White : Colors.Red);

            AddChild(button);
        }
    }

    private void OnPackClicked(CardPack pack) {
        if (Global.Singleton.Money < pack.Cost) {
            SoundController.Singleton.PlaySound(PACK_INSUFFICIENT_MONEY_SFX);
            return;
        }

        Global.Singleton.AddMoney(-pack.Cost);

        ShowFloatingMoneyLabel(-pack.Cost);

        IReadOnlyCollection<string> cardsToSpawn = pack.Name == "Starter Pack"
            ? new List<string>(pack.CommonCards).Concat(pack.RareCards).ToList()
            : GeneratePackContents(pack);

        foreach (string cardName in cardsToSpawn) {
            Vector2 randomPos = GameController.Singleton.NextRandomPositionOnScreen();
            GameController.Singleton.CardController.CreateCard(cardName, randomPos);
        }

        if (pack.Name == "Starter Pack") {
            availablePacks.Remove(pack);
            UnlockAdditionalPacks();
        }

        SoundController.Singleton.PlaySound(PACK_OPEN_SFX);
    }

    private void UnlockAdditionalPacks() {
        DisplayAvailablePacks();
    }

    public void RefreshPackStates(int newMoney) {
        foreach (Node child in GetChildren()) {
            if (child is not PackButton button || button.Pack is null) continue;

            bool isAffordable = Global.Singleton.Money >= button.Pack.Cost;
            // button.Disabled = !isAffordable;
            button.Modulate = isAffordable ? Colors.White : new Color(1, 1, 1, 0.5f);
            button.SetPriceColor(isAffordable ? Colors.White : Colors.Red);
        }
    }

    private static IReadOnlyCollection<string> GeneratePackContents(CardPack pack) {
        IList<string> selectedCards = [];
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

        return selectedCards.AsReadOnly();
    }

    private void ShowFloatingMoneyLabel(int amount) {
        PackedScene labelScene = GD.Load<PackedScene>("res://Scenes/ProgressBars/FloatingMoneyLabel.tscn");
        FloatingMoneyLabel floatingLabel = labelScene.Instantiate<FloatingMoneyLabel>();
        floatingLabel.SetAmount(amount);

        floatingLabel.Position = GameController.Singleton.GetMousePosition();
        GameController.Singleton.HUD.FloatingMoneyRoot.AddChild(floatingLabel);
    }
}