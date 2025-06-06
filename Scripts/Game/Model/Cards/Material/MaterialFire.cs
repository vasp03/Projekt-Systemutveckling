﻿using System.Linq;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Material_Cards;

public class MaterialFire() : CardMaterial("Fire"), ITickable, ICardConsumer {
    private const int FIRE_DAMAGE = 1;

    private int LastFireDamageTick;

    public override int Value => -1;

    public bool ConsumeCard(Card otherCard) {
        if (otherCard is MaterialWater) {
            Extinguish();
            return true;
        }

        return false;
    }

    public void PostTick(double delta) {
        LastFireDamageTick++;
        if (LastFireDamageTick >= Utilities.TimeToTicks(1)) {
            LastFireDamageTick = 0;

            CardNode.CardController.AllCards.ToList().ForEach(e => {
                if (e.CardType is CardLiving cardLiving) cardLiving.Health -= FIRE_DAMAGE;
            });
        }
    }

    public void Extinguish() {
        GameController.Singleton.SoundController.PlaySound("General Sounds/Interactions/sfx_sounds_interaction5.wav");
        CardNode.Destroy();
    }
}