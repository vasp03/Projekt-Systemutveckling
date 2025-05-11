using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class HUD : CanvasLayer {
	private Texture2D[] _coinIcons = new Texture2D[9];
	[Export] public TextureRect GoldIcon { get; set; }
	private Color _defaultColor;
	private float _flashDuration = 0.25f;
	private float _flashTimer = 0.0f;
	private bool _isFlashing = false;
	[Export] public Label MoneyLabel { get; set; }
	public GameController GameController { get; set; }

	private Global _global;

	public override void _Ready() {
		var sellModeButton = GetNode<SellModeButton>("HUDRoot/SellModeButton");
		sellModeButton.GameController = GameController;
		sellModeButton.UpdateIcon();
        
        var packController = GetNode<PackController>("HUDRoot/PackContainer");
        packController.GameController = GameController;
        packController.Init();
		
		_defaultColor = MoneyLabel.Modulate;
		
		_global = GetNode<Global>("/root/Global");
		_global.MoneyChanged += OnMoneyChanged;
		
		for (int i = 0; i < _coinIcons.Length; i++) {
			string path = $"res://Assets/UI/Coins/coin_stack_{i}_{GetStageSuffix(i)}.png";
			_coinIcons[i] = GD.Load<Texture2D>(path);
		}
		
		OnMoneyChanged(_global.Money);
	}
	
	public override void _ExitTree() {
		if (_global != null)
			_global.MoneyChanged -= OnMoneyChanged;
	}
	
	private void OnMoneyChanged(int newMoney) {
        PackController packController = GetNodeOrNull<PackController>("HUDRoot/PackContainer");
        packController?.RefreshPackStates(newMoney);
        
		MoneyLabel.Text = newMoney.ToString();
		MoneyLabel.Modulate = new Color(0.4f, 1f, 0.4f);
		_flashTimer = _flashDuration;
		_isFlashing = true;
		
		UpdateGoldIcon(newMoney);
	}
	
	public override void _Process(double delta) {
		if (_isFlashing) {
			_flashTimer -= (float)delta;
			if (_flashTimer <= 0.0f) {
				MoneyLabel.Modulate = _defaultColor;
				_isFlashing = false;
			}
		}
	}
	
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
	
	private void UpdateGoldIcon(int money) {
		int stage = 0;

		if (money >= 25) stage = 1;
		if (money >= 75) stage = 2;
		if (money >= 150) stage = 3;
		if (money >= 250) stage = 4;
		if (money >= 400) stage = 5;
		if (money >= 600) stage = 6;
		if (money >= 800) stage = 7;
		if (money >= 1000) stage = 8;

		GoldIcon.Texture = _coinIcons[stage];
	}
}
