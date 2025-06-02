using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Buildings;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class BasicBuildingPack : CommonAndRarePack {
    public override string PackButtonTexture => PackTexture("Building Pack");

    public override bool SingleUse => false;

    public override string Name => "Basic building pack";

    public override int Cost => 200;
    
    protected override int CardCount => 1;

    protected override float RareCardChance => 0.2f;

    protected override IReadOnlyList<Card> CommonCards => [
        new BuildingField(),
        new BuildingCampfire()
    ];

    protected override IReadOnlyList<Card> RareCards => [
        new BuildingHouse()
    ];
}