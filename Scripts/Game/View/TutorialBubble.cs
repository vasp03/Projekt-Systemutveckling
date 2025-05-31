using Godot;
using System.Threading.Tasks;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class TutorialBubble : CanvasLayer {
	private Label textLabel;
	private TextureRect arrow;
	private AnimationPlayer animator;

	private string fullText;
	private bool typingInterrupted;

	public enum PointingDirection { Up, Down }

	public override void _Ready() {
		textLabel = GetNode<Label>("Panel/Label");
		arrow = GetNode<TextureRect>("Arrow");
		animator = GetNode<AnimationPlayer>("AnimationPlayer");
		arrow.Hide();
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
        Node target = GameController.Singleton.GetNodeOrNull(nodePath);
        if (target is not Control control) return;

        Vector2 controlGlobalPos = control.GetGlobalTransformWithCanvas().Origin;
        Vector2 controlCenter = controlGlobalPos + control.Size / 2f;

        float screenMiddleY = GetViewport().GetVisibleRect().Size.Y / 2f;

        // Determine the pointing direction based on the control's position
        PointingDirection direction = forcedDirection ?? (
            controlCenter.Y < screenMiddleY ? PointingDirection.Down : PointingDirection.Up
        );

        Vector2 arrowOffset;

        if (manualOffset != null) {
            arrowOffset = manualOffset.Value;
        } else if (direction == PointingDirection.Up) {
            arrowOffset = new Vector2(control.Size.X / 2f, control.Size.Y + 10);
            arrow.FlipV = true;
        } else {
            arrowOffset = new Vector2(control.Size.X / 2f, -40);
            arrow.FlipV = false;
        }

        // If using manual offset, still apply correct flip based on direction
        if (manualOffset != null) {
            arrow.FlipV = direction == PointingDirection.Up;
        }

        arrow.Position = controlGlobalPos + arrowOffset;
        arrow.Show();
        animator.Play("BounceUpDown");
    }


    
	public void HideArrow() {
		arrow.Hide();
		animator.Stop();
	}
}