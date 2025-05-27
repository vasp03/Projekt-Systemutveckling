using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class BasicMaterialPack : CommonAndRarePack {
    // List<string> materialCommons = ["Wood", "Stone", "Leaves", "Sand", "Stick", "Water", "Brick"];
    // List<string> materialRares = ["Clay", "Glass", "Planks"];
    public override string PackButtonTexture => PackTexture("Material pack");

    public override bool SingleUse => false;

    public override string Name => "Basic Material pack";

    public override int Cost => 80;

    protected override IReadOnlyList<Card> CommonCards => [
        new MaterialWood(),
        new MaterialStone(),
        new MaterialLeaves(),
        new MaterialSand(),
        new MaterialStick(),
        new MaterialWater(),
        new MaterialBrick()
    ];

    protected override IReadOnlyList<Card> RareCards => [
        new MaterialClay(),
        new MaterialGlass(),
        new MaterialPlank()
    ];

    protected override int CardCount => GD.RandRange(3, 5);
}