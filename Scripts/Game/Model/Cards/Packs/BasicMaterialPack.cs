using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class BasicMaterialPack : CommonAndRarePack {
    public override string PackButtonTexture => PackTexture("Material Pack");

    public override bool SingleUse => false;

    public override string Name => "Basic Material pack";

    public override int Cost => 140;
    
    protected override int CardCount => GD.RandRange(3, 4);

    protected override IReadOnlyList<Card> CommonCards => [
        new MaterialWood(),
        new MaterialStone(),
        new MaterialLeaves(),
        new MaterialSand(),
        new MaterialStick()
    ];

    protected override IReadOnlyList<Card> RareCards => [
        new MaterialClay(),
        new MaterialGlass(),
        new MaterialPlank(),
        new MaterialBrick()
    ];
}