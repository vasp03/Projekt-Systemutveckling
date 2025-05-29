using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Living;

namespace Goodot15.Scripts.Game.Model.Parents;

public class Altar() : Card("Alter_0", true), ICardConsumer, ITickable {
    private const int TOTAL_SACRIFICES_REQUIRED = 10;

    private static readonly Texture2D[] STAGES_TEXTURES =
        Enumerable.Range(0, 5)
            .Select(n => GD.Load<Texture2D>($"res://Assets/Cards/Ready To Use/Alter_{n}.png"))
            .ToArray();

    private bool playedSound;
    private int totalVillagersSacrificed;
    public override int Value => -1;

    public bool ConsumeCard(Card otherCard) {
        if (otherCard is not LivingPlayer || totalVillagersSacrificed > TOTAL_SACRIFICES_REQUIRED) return false;
        totalVillagersSacrificed++;
        GameController.Singleton!.CameraController.Shake(totalVillagersSacrificed, Utilities.TimeToTicks(0.75d));

        switch (totalVillagersSacrificed) {
            case 0:
                CardNode.UpdateCardTexture(STAGES_TEXTURES[0], true);
                break;
            case >= 1 when totalVillagersSacrificed <= TOTAL_SACRIFICES_REQUIRED * 1d / 3d:
                CardNode.UpdateCardTexture(STAGES_TEXTURES[1], true);
                break;
            default: {
                if (totalVillagersSacrificed > TOTAL_SACRIFICES_REQUIRED * 1d / 4d &&
                    totalVillagersSacrificed <= TOTAL_SACRIFICES_REQUIRED * 2d / 3d)
                    CardNode.UpdateCardTexture(STAGES_TEXTURES[2], true);
                else if (totalVillagersSacrificed > TOTAL_SACRIFICES_REQUIRED * 1d / 2d &&
                         totalVillagersSacrificed < TOTAL_SACRIFICES_REQUIRED)
                    CardNode.UpdateCardTexture(STAGES_TEXTURES[3], true);
                else if (totalVillagersSacrificed >= TOTAL_SACRIFICES_REQUIRED)
                    CardNode.UpdateCardTexture(STAGES_TEXTURES[4], true);
                break;
            }
        }

        return true;
    }

    public void PostTick(double delta) {
        if (totalVillagersSacrificed < TOTAL_SACRIFICES_REQUIRED || playedSound) return;
        GameController.Singleton!.SoundController.PlaySound(
            "General Sounds/Interactions/sfx_sounds_interaction5.wav");
        playedSound = true;
        GameController.Singleton.CameraController.PlayEndGameAnimation();
    }
}