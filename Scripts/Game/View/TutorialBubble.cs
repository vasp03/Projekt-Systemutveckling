using Godot;
using System.Threading.Tasks;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class TutorialBubble : CanvasLayer {
	private Label textLabel;
	private Node2D arrow;
	private TextureRect arrowImage;
	private Tween bounceTween;

	private string fullText;
	private bool typingInterrupted;
	
	private string lastNodePath;
	private Vector2? lastOffset;
	private PointingDirection? lastDirection;
	
	[Signal] public delegate void BubbleClickedEventHandler();
	public enum PointingDirection { Up, Down }

	public override void _Ready() {
		textLabel = GetNode<Label>("Panel/Label");
		arrow = GetNode<Node2D>("Arrow");
		arrowImage = arrow.GetNode<TextureRect>("ArrowImage");
		arrow.Hide();
	}
	
	private void _on_panel_gui_input(InputEvent @event) {
		if (@event is InputEventMouseButton { Pressed: true }) {
			EmitSignal(SignalName.BubbleClicked);
		}
	}

	public async Task ShowText(string text, float speed = 0.03f) {
		fullText = text;
		typingInterrupted = false;
		textLabel.Text = "";

		foreach (char c in text) {
			if (typingInterrupted) break;
			textLabel.Text += c;
			await ToSignal(GetTree().CreateTimer(speed), "timeout");
		}

		textLabel.Text = fullText;
	}

	public void SkipTyping() {
		typingInterrupted = true;
		textLabel.Text = fullText;
	}

	public void PointToUI(string nodePath, PointingDirection? forcedDirection = null, Vector2? manualOffset = null) {
		lastNodePath = nodePath;
		lastOffset = manualOffset;
		lastDirection = forcedDirection;

		Node target = GameController.Singleton.GetNodeOrNull(nodePath);
		if (target is not Control control) return;

		Vector2 controlPos = control.GetGlobalRect().Position;
		Vector2 controlSize = control.Size;
		Vector2 controlCenter = controlPos + controlSize / 2f;

		float screenMiddleY = GetViewport().GetVisibleRect().Size.Y / 2f;
		PointingDirection direction = forcedDirection ?? (
			controlCenter.Y < screenMiddleY ? PointingDirection.Down : PointingDirection.Up
		);

		Vector2 offset = manualOffset ?? (
			direction == PointingDirection.Up
				? new Vector2(controlSize.X / 2f, controlSize.Y + 10)
				: new Vector2(controlSize.X / 2f, -40)
		);

		// Flip arrow image
		arrowImage.FlipV = direction == PointingDirection.Up;

		// Place the outer arrow container at the target
		arrow.Position = controlPos + offset;
		arrowImage.Position = Vector2.Zero; // Reset local offset
		arrow.Show();

		StartBounce();
	}
	
	public void ShowLastArrow() {
		if (string.IsNullOrEmpty(lastNodePath)) return;
		PointToUI(lastNodePath, lastDirection, lastOffset);
	}

	public void HideArrow() {
		arrow.Hide();
		StopBounce();
	}

	private void StartBounce() {
		if (bounceTween != null && bounceTween.IsRunning()) return;

		bounceTween = CreateTween().SetLoops();

		bounceTween.TweenProperty(arrowImage, "position:y", -8, 0.25)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);

		bounceTween.TweenProperty(arrowImage, "position:y", 0, 0.25)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);
	}

	private void StopBounce() {
		bounceTween?.Kill();
		bounceTween = null;
	}
}
