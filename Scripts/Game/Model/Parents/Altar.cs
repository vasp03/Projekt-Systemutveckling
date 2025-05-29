using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Living;

namespace Goodot15.Scripts.Game.Model.Parents;

public class Altar() : Card("Alter_0", true), ICardConsumer, ITickable {
    private const int TOTAL_SACRIFICES_REQUIRED = 10;

    private readonly Texture2D stage0 = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Alter_0.png");
    private readonly Texture2D stage1 = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Alter_1.png");
    private readonly Texture2D stage2 = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Alter_2.png");
    private readonly Texture2D stage3 = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Alter_3.png");
    private readonly Texture2D stage4 = GD.Load<Texture2D>("res://Assets/Cards/Ready To Use/Alter_4.png");
    private bool playedSound;
    private int totalVillagersSacrificed;
    public override int Value => -1;

    public bool ConsumeCard(Card otherCard) {
        if (otherCard is not LivingPlayer || totalVillagersSacrificed > TOTAL_SACRIFICES_REQUIRED) return false;
        totalVillagersSacrificed++;
        GameController.Singleton.CameraController.Shake(totalVillagersSacrificed, Utilities.TimeToTicks(0.75d));

        if (totalVillagersSacrificed == 0)
            CardNode.UpdateCardTexture(stage0, true);
        else if (totalVillagersSacrificed >= 1 && totalVillagersSacrificed <= TOTAL_SACRIFICES_REQUIRED * 0.33)
            CardNode.UpdateCardTexture(stage1, true);
        else if (totalVillagersSacrificed > TOTAL_SACRIFICES_REQUIRED * 0.25 &&
                 totalVillagersSacrificed <= TOTAL_SACRIFICES_REQUIRED * 0.67)
            CardNode.UpdateCardTexture(stage2, true);
        else if (totalVillagersSacrificed > TOTAL_SACRIFICES_REQUIRED * 0.5 &&
                 totalVillagersSacrificed < TOTAL_SACRIFICES_REQUIRED)
            CardNode.UpdateCardTexture(stage3, true);
        else if (totalVillagersSacrificed >= TOTAL_SACRIFICES_REQUIRED) CardNode.UpdateCardTexture(stage4, true);

        return true;
    }

    public void PostTick(double delta) {
        if (totalVillagersSacrificed >= TOTAL_SACRIFICES_REQUIRED && !playedSound) {
            GameController.Singleton.SoundController.PlaySound(
                "General Sounds/Interactions/sfx_sounds_interaction5.wav");
            playedSound = true;
            GameController.Singleton.CameraController.PlayEndGameAnimation();
        }
    }
}