using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Buildings;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class BasicBuildingPack : CommonAndRarePack {
    // List<string> buildingCommons = ["Field", "Campfire", "House"];
    // List<string> buildingRares = ["Greenhouse"];
    public override string PackButtonTexture => PackTexture("Building Pack");

    public override bool SingleUse => false;

    public override string Name => "Basic building pack";

    public override int Cost => 200;

    protected override IReadOnlyList<Card> CommonCards => [
        new BuildingField(),
        new BuildingCampfire(),
        new BuildingHouse()
    ];

    protected override IReadOnlyList<Card> RareCards => [
        new BuildingGreenhouse()
    ];
}