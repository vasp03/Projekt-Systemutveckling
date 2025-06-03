using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Cards.Packs;
using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Parents;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

public partial class PackController : HBoxContainer {
    private const string PACK_OPEN_SFX = "General Sounds/Interactions/sfx_sounds_interaction17.wav";
    private const string PACK_INSUFFICIENT_MONEY_SFX = "General Sounds/Negative Sounds/sfx_sounds_error3.wav";

    private readonly IList<CardPack> registeredPacks = [];

    [Export] public PackedScene PackButtonScene;
    public IReadOnlyCollection<CardPack> RegisteredPacks => registeredPacks.AsReadOnly();

    public override void _Ready() {
        Initialize();
    }

    private void Initialize() {
        RegisterDefaultPacks();
        RefreshAvailablePacks();
    }

    private void RegisterDefaultPacks() {
        RegisterPack(new StarterPack());
        RegisterPack(new BasicWaterPack());
        RegisterPack(new BasicMaterialPack());
        RegisterPack(new BasicFoodPack());
        RegisterPack(new BasicToolsPack());
        RegisterPack(new BasicBuildingPack());
        RegisterPack(new BasicVillagerPack());
    }

    /// <summary>
    ///     Registers a pack
    /// </summary>
    /// <param name="pack">Pack to be registered</param>
    public void RegisterPack(CardPack pack) {
        registeredPacks.Add(pack);
    }

    public void RefreshAvailablePacks() {
        if (Global.Singleton is null || PackButtonScene is null) return;

        foreach (Node child in GetChildren()) child.QueueFree();

        foreach (CardPack pack in registeredPacks.Where(e => !e.Consumed)) {
            PackButton button = PackButtonScene.Instantiate<PackButton>();
            button.SetPack(pack);
            button.PackClicked += OnPackClicked;

            bool isAffordable = Global.Singleton.Money >= pack.Cost;
            // button.Disabled = !isAffordable;
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
        if (Global.Singleton.Money < pack.Cost) {
            SoundController.Singleton.PlaySound(PACK_INSUFFICIENT_MONEY_SFX);
            return;
        }

        Global.Singleton.AddMoney(-pack.Cost);

        ShowFloatingMoneyLabel(-pack.Cost);

        IReadOnlyCollection<Card> cardsInPack = pack.GenerateCards();

        foreach (Card cardInstance in cardsInPack) {
            Rect2 spawnArea = new Rect2(new Vector2(75, 150), new Vector2(1125, 375));
            
            Vector2 randomPos = new Vector2(
                (float)GD.RandRange(spawnArea.Position.X, spawnArea.End.X),
                (float)GD.RandRange(spawnArea.Position.Y, spawnArea.End.Y)
            );

            GameController.Singleton.CardController.CreateCard(cardInstance, randomPos);
        }

        if (pack.SingleUse)
            pack.Consumed = true;

        SoundController.Singleton.PlaySound(PACK_OPEN_SFX);

        RefreshAvailablePacks();

        if (GameController.Singleton.SellModeActive) GameController.Singleton.SetSellMode(false);
    }

    private void UnlockAdditionalPacks() {
        RefreshAvailablePacks();
    }

    private void ShowFloatingMoneyLabel(int amount) {
        PackedScene labelScene = GD.Load<PackedScene>("res://Scenes/ProgressBars/FloatingMoneyLabel.tscn");
        FloatingMoneyLabel floatingLabel = labelScene.Instantiate<FloatingMoneyLabel>();
        floatingLabel.SetAmount(amount);

        floatingLabel.Position = GameController.Singleton.GetMousePosition();
        GameController.Singleton.HUD.FloatingMoneyRoot.AddChild(floatingLabel);
    }
}