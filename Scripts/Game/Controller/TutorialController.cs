using Godot;
using System.Threading.Tasks;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

public partial class TutorialController : CanvasLayer {
	[Export] public PackedScene TutorialBubbleScene;

	private TutorialBubble bubbleInstance;
    
    private bool tutorialEnded = false;

	private int stepIndex = 0;
	private bool isTyping = false;
	private bool canAdvance = false;

	public override void _Ready() {
		bubbleInstance = TutorialBubbleScene.Instantiate<TutorialBubble>();
		AddChild(bubbleInstance);

		_ = ShowStep(0);
	}

	public override void _UnhandledInput(InputEvent @event) {
        if (tutorialEnded) return;
        
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
			case 0:
				await bubbleInstance.ShowText("Welcome to the island. Survive as long as you can by collecting cards and using them to your advantage. This tutorial will guide you through the basics of the game.");
				break;
			case 1:
				await bubbleInstance.ShowText("Use the packs below to get started. The first pack is free and contains a set of cards to help you get started on your journey.");
				bubbleInstance.PointToUI("HUD/HUDRoot/PackContainer", TutorialBubble.PointingDirection.Down, new Vector2(8, -48));
				break;
			case 2:
				await bubbleInstance.ShowText("This is your money. You gain gold from selling cards which you can use to buy more packs.");
				bubbleInstance.PointToUI("HUD/GoldIcon", TutorialBubble.PointingDirection.Up, new Vector2(0, 60));
				break;
			case 3:
				await bubbleInstance.ShowText("This is the current time. Random events can happen at any moment, so watch out.");
				bubbleInstance.PointToUI("HUD/DayTimeLabel", TutorialBubble.PointingDirection.Up, new Vector2(15, 60));
				break;
			default:
				bubbleInstance.HideArrow();
				isTyping = true;
				canAdvance = false;
				await bubbleInstance.ShowText("Tutorial complete. If you need help, you can always access the tutorial again from the menu or from the guide button in the HUD. Good luck Adventurer!");
                tutorialEnded = true;
				await Task.Delay(2000);
				bubbleInstance.QueueFree();
				QueueFree();
				return;
		}

		isTyping = false;
		canAdvance = true;
	}
}
