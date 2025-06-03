using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Controller.Events;

namespace Goodot15.Scripts.Game.View;

public partial class HUD : CanvasLayer {
    private Texture2D[] coinIcons = new Texture2D[9];
    private Color defaultColor;
    private float flashDuration = 0.25f;
    private float flashTimer;

    private Global Global;
    private bool isFlashing;
    private PackController packController;
    private Texture2D thermometerCold;

    private Texture2D thermometerNormal;

    public TextureRect ThermometerIcon { get; set; }
    public Label TemperatureLabel { get; set; }
    [Export] public TextureRect GoldIcon { get; set; }
    [Export] public Label MoneyLabel { get; set; }
    [Export] public Control FloatingMoneyRoot { get; set; }

    public SellModeButton sellModeButton { get; private set; }

    #region Setup

    public override void _Ready() {
        Global = Global.Singleton;
        Global.MoneyChanged += OnMoneyChanged;

        packController = GetNodeOrNull<PackController>("HUDRoot/PackContainer");

        thermometerNormal = GD.Load<Texture2D>("res://Assets/UI/Thermometer/thermometer_normal.png");
        thermometerCold = GD.Load<Texture2D>("res://Assets/UI/Thermometer/thermometer_cold.png");

        ThermometerIcon = GetNodeOrNull<TextureRect>("HUDRoot/ThermometerContainer/ThermometerIcon");
        TemperatureLabel = GetNodeOrNull<Label>("HUDRoot/ThermometerContainer/TemperatureLabel");

        SetupSellModeButton();

        defaultColor = MoneyLabel.Modulate;
        LoadCoinTextures();

        OnMoneyChanged(Global.Money);

        CallDeferred(nameof(UpdateThermometerUI));
    }

    public override void _ExitTree() {
        if (Global is not null) Global.MoneyChanged -= OnMoneyChanged;
    }

    private void SetupSellModeButton() {
        sellModeButton = GetNodeOrNull<SellModeButton>("HUDRoot/SellModeButton");
        sellModeButton?.UpdateIcon();
    }

    public void ShowFloatingMoneyLabel(int amount) {
        PackedScene scene = GD.Load<PackedScene>("res://Scenes/ProgressBars/FloatingMoneyLabel.tscn");
        FloatingMoneyLabel label = scene.Instantiate<FloatingMoneyLabel>();
        label.SetAmount(amount);
        label.Position = GetViewport().GetMousePosition();
        FloatingMoneyRoot.AddChild(label);
    }

    private void LoadCoinTextures() {
        for (int i = 0; i < coinIcons.Length; i++) {
            string path = $"res://Assets/UI/Coins/coin_stack_{i}_{GetStageSuffix(i)}.png";
            coinIcons[i] = GD.Load<Texture2D>(path);
        }
    }

    #endregion

    #region Runtime UI

    public override void _Process(double delta) {
        if (!isFlashing) return;

        flashTimer -= (float)delta;
        if (flashTimer <= 0.0f) {
            MoneyLabel.Modulate = defaultColor;
            isFlashing = false;
        }
    }

    private void OnMoneyChanged(int newMoney) {
        packController?.RefreshAvailablePacks();

        MoneyLabel.Text = newMoney.ToString();
        MoneyLabel.Modulate = new Color(0.4f, 1f, 0.4f);
        flashTimer = flashDuration;
        isFlashing = true;

        UpdateGoldIcon(newMoney);
    }

    private void UpdateGoldIcon(int money) {
        int stage = money switch
        {
            >= 7000 => 8,
            >= 5000 => 7,
            >= 4000 => 6,
            >= 3000 => 5,
            >= 2000 => 4,
            >= 1000 => 3,
            >= 500 => 2,
            >= 200 => 1,
            _ => 0
        };

        GoldIcon.Texture = coinIcons[stage];
    }

    public void UpdateThermometerUI() {
        if (ThermometerIcon == null || TemperatureLabel == null) {
            GD.PrintErr("ThermometerIcon or TemperatureLabel is null");
            return;
        }

        float temp = GameController.Singleton.GameEventManager.EventInstance<DayTimeEvent>().CurrentTemperature;

        ThermometerIcon.Texture = temp <= 0 ? thermometerCold : thermometerNormal;

        TemperatureLabel.Text = $"{Mathf.RoundToInt(temp)}°C";
    }

    /// <summary>
    ///     Maps an integer-value to the string suffix equivalent
    /// </summary>
    /// <param name="index">0-9 value (inclusive)</param>
    /// <returns>Stage suffix as string</returns>
    private string GetStageSuffix(int index) {
        return index switch {
            0 => "bronze",
            1 => "silver1",
            2 => "silver2",
            3 => "silver3",
            4 => "silver4",
            5 => "gold1",
            6 => "gold2",
            7 => "gold3",
            8 => "gold4",
            _ => "bronze"
        };
    }

    #endregion
}