

using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Living;

namespace Goodot15.Scripts.Game.Model.Parents;

public class Altar() : Card("Altar", true), ICardConsumer, ITickable {
    private const int TOTAL_SACRIFICES_REQUIRED = 3;
    private int totalSacrificesSacrified = 0;
    private bool playedSound = false;
    public override int Value => -1;

    public bool ConsumeCard(Card otherCard) {
        if (otherCard is not LivingPlayer && totalSacrificesSacrified < TOTAL_SACRIFICES_REQUIRED) return false;
        totalSacrificesSacrified++;
        GameController.Singleton.CameraController.Shake(totalSacrificesSacrified, Utilities.TimeToTicks(seconds: 0.75d));
        return true;
    }

    public void PostTick(double delta) {
        if (totalSacrificesSacrified >= TOTAL_SACRIFICES_REQUIRED && !playedSound) {
            GameController.Singleton.SoundController.PlaySound("General Sounds/Interactions/sfx_sounds_interaction5.wav");
            playedSound = true;
            GameController.Singleton.CameraController.PlayEndGameAnimation();
        }
    }
}