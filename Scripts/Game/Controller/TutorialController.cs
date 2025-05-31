using Godot;
using System.Threading.Tasks;

namespace Goodot15.Scripts.Game.Controller;

public partial class TutorialController : CanvasLayer {
    [Export] public PackedScene TutorialBubbleScene;

    //private TutorialBubble bubbleInstance;

    private int stepIndex = 0;
    private bool isTyping = false;
    private bool canAdvance = false;
    
    public override void _Ready() {
        //bubbleInstance = TutorialBubbleScene.Instantiate<TutorialBubble>();
        //AddChild(bubbleInstance);

        StartTutorial();
    }

    private async void StartTutorial() {
     //   await ShowStep(0);
    }
}