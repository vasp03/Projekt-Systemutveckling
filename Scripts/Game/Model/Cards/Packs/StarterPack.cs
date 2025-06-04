using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Living.Villagers;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class StarterPack : CardPack {
    public override string PackButtonTexture => PackTexture("Starter Pack");

    // List<string> starterCommons = ["Villager", "Tree", "Bush", "Stone", "Stick", "Stick"];
    // List<string> starterRares = [];
    public override bool SingleUse => true;

    public override string Name => "Starter Pack";

    public override int Cost => -150;

    public override IReadOnlyCollection<Card> GenerateCards() {
        return [
            new PlayerVillager(),
            new MaterialTree(),
            new MaterialBush(),
            new MaterialStone(),
            new MaterialStick(),
            new MaterialStick(),
            new MaterialWater(),
            new MaterialWood()
        ];
    }
}