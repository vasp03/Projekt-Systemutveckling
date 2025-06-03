using System.Threading.Tasks;
using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class TutorialBubble : CanvasLayer {
	[Signal]
	public delegate void BubbleClickedEventHandler();

	public enum PointingDirection {
		Up,
		Down
	}

	private Node2D arrow;
	private TextureRect arrowImage;
	private Tween bounceTween;

	private string fullText;
	private PointingDirection? lastDirection;

	private string lastNodePath;
	private Vector2? lastOffset;
	private Label textLabel;
	private bool typingInterrupted;

	public override void _Ready() {
		textLabel = GetNode<Label>("Panel/Label");
		arrow = GetNode<Node2D>("Arrow");
		arrowImage = arrow.GetNode<TextureRect>("ArrowImage");
		arrow.Hide();
	}

	/// <summary>
	///     Method called when the panel receives a GUI input event.
	///     Connected to the panel's "gui_input" signal from Godot editor.
	/// </summary>
	/// <param name="event"></param>
	private void _on_panel_gui_input(InputEvent @event) {
		if (@event is InputEventMouseButton { Pressed: true }) EmitSignal(SignalName.BubbleClicked);
	}

	/// <summary>
	///     Method for showing text in the tutorial bubble. Uses for loop to simulate typing effect.
	///     Creates a timer for each character to control the speed of typing.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="speed"></param>
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

	/// <summary>
	///     Method for skipping the typing effect and displaying the full text immediately.
	/// </summary>
	public void SkipTyping() {
		typingInterrupted = true;
		textLabel.Text = fullText;
	}

	/// <summary>
	///     Method for pointing the arrow image to a specific UI element.
	///     Has optional parameters for forced pointing direction and manual offset.
	/// </summary>
	/// <param name="nodePath"></param>
	/// <param name="forcedDirection"></param>
	/// <param name="manualOffset"></param>
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

	/// <summary>
	///     Displays the last pointed arrow based on the last stored node path, direction, and offset.
	///     Used for showing the last arrow when needed, such as after pausing or resuming the game.
	/// </summary>
	public void ShowLastArrow() {
		if (string.IsNullOrEmpty(lastNodePath)) return;
		PointToUI(lastNodePath, lastDirection, lastOffset);
	}

	/// <summary>
	///     Method for hiding the arrow image and stopping the bounce animation.
	/// </summary>
	public void HideArrow() {
		arrow.Hide();
		StopBounce();
	}
    /// <summary>
    ///     Method for showing the arrow image and starting the bounce animation.
    /// </summary>
    public void ShowArrow() {
        arrow.Show();
        StartBounce();
    }

	/// <summary>
	///     Method for starting the bounce animation for the arrow image.
	///     Uses a Tween to animate the arrow's vertical position based on its relative position.
	/// </summary>
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

	/// <summary>
	///     Method for stopping the bounce animation and clearing the tween.
	///     This is called when the arrow is hidden or no longer needed.
	/// </summary>
	private void StopBounce() {
		bounceTween?.Kill();
		bounceTween = null;
	}
}
