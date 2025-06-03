using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class BasicWaterPack : CommonAndRarePack {
    public override string PackButtonTexture => PackTexture("Water Pack");

    public override bool SingleUse => false;

    public override string Name => "Basic Water pack";

    public override int Cost => 50;

    protected override int CardCount => 1;

    protected override IReadOnlyList<Card> CommonCards => [
        new MaterialWater()
    ];

    protected override IReadOnlyList<Card> RareCards => [
        new MaterialWater()
    ];
}