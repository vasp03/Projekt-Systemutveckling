using Godot;
using System.Threading.Tasks;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

public partial class TutorialController : CanvasLayer {
	[Export] public PackedScene TutorialBubbleScene;

	private TutorialBubble bubbleInstance;

	private int stepIndex = 0;
	private bool isTyping = false;
	private bool canAdvance = false;

	public override void _Ready() {
		bubbleInstance = TutorialBubbleScene.Instantiate<TutorialBubble>();
		AddChild(bubbleInstance);

		StartTutorial();
	}

	private async void StartTutorial() {
		isTyping = true;
		canAdvance = false;
		await bubbleInstance.ShowText("Welcome to the island. Survive the days, craft, and sacrifice.");
		isTyping = false;
		canAdvance = true;
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseButton { Pressed: true }) {
			if (isTyping) {
				bubbleInstance.SkipTyping();
				isTyping = false;
				canAdvance = true;
			} else if (canAdvance) {
				stepIndex++;
				_ = ShowStep(stepIndex);
			}
		}
	}

	private async Task ShowStep(int index) {
		isTyping = true;
		canAdvance = false;

		switch (index) {
			case 1:
				await bubbleInstance.ShowText("Use the packs below to get started.");
				bubbleInstance.PointToUI("HUD/HUDRoot/PackContainer", TutorialBubble.PointingDirection.Down, new Vector2(8, -48));
				break;
			case 2:
				await bubbleInstance.ShowText("This is your money. You gain gold from selling cards.");
				bubbleInstance.PointToUI("HUD/GoldIcon", TutorialBubble.PointingDirection.Up, new Vector2(0, 45));
				break;
			case 3:
				await bubbleInstance.ShowText("This is the current day and time.");
				bubbleInstance.PointToUI("HUD/DayTimeLabel", TutorialBubble.PointingDirection.Up, new Vector2(20, 45));
				break;
			default:
				bubbleInstance.HideArrow();
				await bubbleInstance.ShowText("Tutorial complete. Good luck!");
                await Task.Delay(2000);
				QueueFree();
				break;
		}

		isTyping = false;
		canAdvance = true;
	}
}
