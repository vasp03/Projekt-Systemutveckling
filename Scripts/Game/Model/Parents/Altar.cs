

using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Parents;

public class Altar() : Card("Altar", true), ICardConsumer, ITickable {
    private int recuiredSacrificeCount = 3;
    private bool playedSound = false;

    public override int Value => -1;

    public bool ConsumeCard(Card otherCard) {
        if (otherCard is not CardLiving cardLiving) return false;
        recuiredSacrificeCount--;
        return true;
    }

    public void PostTick(double delta) {
        if (recuiredSacrificeCount <= 0 && !playedSound) {
            GameController.Singleton.SoundController.PlaySound("General Sounds/Interactions/sfx_sounds_interaction5.wav");
            playedSound = true;
        }
    }
}