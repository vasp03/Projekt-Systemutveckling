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

    private readonly static PackedScene LABEL_SCENE =
        GD.Load<PackedScene>("res://Scenes/ProgressBars/FloatingMoneyLabel.tscn");

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

        foreach (CardPack pack in RegisteredPacks.Where(e => !e.Consumed)) {
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
        // If the pack is registered as free (Cost = 0)
        // If the player does not have enough coins, cancel the operation
        if (Global.Singleton.Money < pack.Cost) {
            SoundController.Singleton.PlaySound(PACK_INSUFFICIENT_MONEY_SFX);
            return;
        }

        // if (pack.Cost < 0) {
        //     Global.Singleton.AddMoney(Mathf.Abs(pack.Cost));
        // }

        // Will automatically reward the player with coins if the cost is negative (because y'know, negative and negative becomes positive)
        Global.Singleton.AddMoney(-pack.Cost);

        ShowFloatingMoneyLabel(-pack.Cost);

        IReadOnlyCollection<Card> cardsInPack = pack.GenerateCards();

        foreach (Card cardInstance in cardsInPack) {
            Rect2 spawnArea = new(new Vector2(75, 150), new Vector2(1125, 375));

            Vector2 randomPos = new(
                (float)GD.RandRange(spawnArea.Position.X, spawnArea.End.X),
                (float)GD.RandRange(spawnArea.Position.Y, spawnArea.End.Y)
            );

            GameController.Singleton.CardController.CreateCard(cardInstance, randomPos);
        }

        if (pack.SingleUse)
            pack.Consumed = true;

        SoundController tempQualifier1 = SoundController.Singleton;
        tempQualifier1.PlaySound(PACK_OPEN_SFX);

        RefreshAvailablePacks();

        if (GameController.Singleton.SellModeActive) GameController.Singleton.SellModeActive = false;
    }

    private void UnlockAdditionalPacks() {
        RefreshAvailablePacks();
    }

    private static void ShowFloatingMoneyLabel(int amount) {
        FloatingMoneyLabel floatingLabel = LABEL_SCENE.Instantiate<FloatingMoneyLabel>();
        floatingLabel.SetAmount(amount);

        floatingLabel.Position = GameController.Singleton.GetMousePosition();
        GameController.Singleton.HUD.FloatingMoneyRoot.AddChild(floatingLabel);
    }
}