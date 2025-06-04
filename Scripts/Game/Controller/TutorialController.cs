using System.Threading.Tasks;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;

namespace Goodot15.Scripts.Game.Controller;

public partial class TutorialController : CanvasLayer, IPausable {
    private static readonly PackedScene TutorialBubbleScene =
        GD.Load<PackedScene>("D:/MauProjekt/Projekt-Systemutveckling/Scenes/TutorialBubble.tscn");

    private TutorialBubble bubbleInstance;
    private bool canAdvance;

    private bool isPaused;
    private bool isTyping;

    private int stepIndex;

    private bool tutorialEnded;

    /// <summary>
    ///     Method for setting the paused state of the tutorial.
    /// </summary>
    /// <param name="paused"></param>
    /// <param name="hideOverlay"></param>
    public void SetPaused(bool paused, bool hideOverlay = true) {
        isPaused = paused;

        if (!IsInstanceValid(bubbleInstance)) return;

        // Hide the full tutorial bubble UI
        bubbleInstance.Visible = !paused;

        // Hide or restore the arrow
        if (paused)
            bubbleInstance.HideArrow();
        else if (!tutorialEnded) bubbleInstance.ShowLastArrow();
    }

    public override void _Ready() {
        bubbleInstance = TutorialBubbleScene.Instantiate<TutorialBubble>();
        AddChild(bubbleInstance);
        bubbleInstance.BubbleClicked += OnBubbleClicked;

        GameController.Singleton.AddPauseCallback(this);

        _ = ShowStep(0);
    }

    /// <summary>
    ///     Method called when the tutorial bubble is clicked.
    ///     Used for advancing the tutorial steps or skipping typing.
    /// </summary>
    private void OnBubbleClicked() {
        if (tutorialEnded || isPaused) return;

        if (isTyping) {
            bubbleInstance.SkipTyping();
            isTyping = false;
            canAdvance = true;
        } else if (canAdvance) {
            stepIndex++;
            _ = ShowStep(stepIndex);
        }
    }

    /// <summary>
    ///     Shows the tutorial step based on the index provided.
    ///     Calls the appropriate method to display the text and UI elements for each step.
    /// </summary>
    /// <param name="tutorialStage">Stage in the tutorial to be displayed</param>
    private async Task ShowStep(int tutorialStage) {
        isTyping = true;
        canAdvance = false;

        switch (tutorialStage) {
            case 0:
                await bubbleInstance.ShowText(
                    "Welcome to the island. Survive against the elements by using and crafting cards. This tutorial will guide you though the basics.");
                break;
            case 1:
                bubbleInstance.PointToUI("HUD/HUDRoot/PackContainer", TutorialBubble.PointingDirection.Down,
                    new Vector2(8, -48));
                await bubbleInstance.ShowText(
                    "Use the packs below to get started. The first pack is free and contains a small amount of coins and a set of cards to help you get started on your journey.");
                break;
            case 2:
                bubbleInstance.PointToUI("HUD/GoldIcon", TutorialBubble.PointingDirection.Up, new Vector2(0, 60));
                await bubbleInstance.ShowText(
                    "This is your money. You gain gold from selling cards which you can use to buy more packs.");
                break;
            case 3:
                bubbleInstance.PointToUI("HUD/HUDRoot/SellModeButton", TutorialBubble.PointingDirection.Down,
                    new Vector2(-5, -50));
                await bubbleInstance.ShowText(
                    "This is the Sell Mode button. When activated, you can sell cards for money.");
                break;
            case 4:
                bubbleInstance.PointToUI("HUD/HUDRoot/SellModeButton", TutorialBubble.PointingDirection.Down,
                    new Vector2(-5, -50));
                await bubbleInstance.ShowText(
                    "Click on it to toggle Sell mode on and off or use the hotkey 'S' to toggle it quickly.");
                break;
            case 5:
                bubbleInstance.PointToUI("HUD/DayTimeLabel", TutorialBubble.PointingDirection.Up, new Vector2(32, 65));
                await bubbleInstance.ShowText(
                    "This is the current time. Random events can happen at any moment, so watch out.");
                break;
            case 6:
                bubbleInstance.PointToUI("HUD/DayTimeLabel", TutorialBubble.PointingDirection.Up, new Vector2(32, 65));
                await bubbleInstance.ShowText(
                    "There are different types of events that can happen, such as fires, meteorite strikes, and more.");
                break;
            case 7:
                bubbleInstance.PointToUI("HUD/DayTimeLabel", TutorialBubble.PointingDirection.Up, new Vector2(32, 65));
                await bubbleInstance.ShowText(
                    "Whenever a fire starts, your villagers will start taking damage. You can use water cards to extinguish the fires and save them.");
                break;
            case 8:
                bubbleInstance.PointToUI("HUD/HUDRoot/ThermometerContainer", TutorialBubble.PointingDirection.Up,
                    new Vector2(13, 77));
                await bubbleInstance.ShowText(
                    "Temperature changes with time and weather. If it gets too cold, villagers freeze and take damage. Find a way to keep them warm to survive.");
                break;
            default:
                bubbleInstance.HideArrow();
                isTyping = true;
                canAdvance = false;
                await bubbleInstance.ShowText(
                    "Tutorial complete. If you need help, you can always access the guide meny by clicking the HUD button in the bottom right corner of the screen. Good luck Adventurer!");
                tutorialEnded = true;
                await Task.Delay(3000);
                bubbleInstance.QueueFree();
                QueueFree();
                return;
        }

        isTyping = false;
        canAdvance = true;
    }
}