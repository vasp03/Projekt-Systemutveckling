using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialFireCard() : CardMaterial("Fire"), ITickable, ICardConsumer {
    private const int FIRE_DAMAGE = 1;

    private int LastFireDamageTick;

    public bool ConsumeCard(Card otherCard) {
        if (otherCard is MaterialWater) {
            Extinguish();
            return true;
        }

        return false;
    }

    public void PostTick() {
        LastFireDamageTick++;
        if (LastFireDamageTick >= Utilities.TimeToTicks(1)) {
            LastFireDamageTick = 0;

            CardNode.CardController.AllCards.ToList().ForEach(e => {
                if (e.CardType is CardLiving cardLiving) {
                    cardLiving.Health -= FIRE_DAMAGE;
                    
#if DEBUG
                    GD.Print("dealt damage to " + cardLiving + " at " + cardLiving.CardNode.Position);
#endif
                }
            });
        }
    }

    public void Extinguish() {
        GameController.Singleton.GetSoundController().PlaySound("General Sounds/Interactions/sfx_sounds_interaction5.wav");
        CardNode.Destroy();
    }
}