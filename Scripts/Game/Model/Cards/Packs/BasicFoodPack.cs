using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class BasicFoodPack : CommonAndRarePack {
    public override string PackButtonTexture => PackTexture("Food Pack");

    public override bool SingleUse => false;

    public override string Name => "Basic Food Pack";

    public override int Cost => 125;

    protected override int CardCount => GD.RandRange(2, 3);

    protected override IReadOnlyList<Card> CommonCards => [
        new MaterialBerry(),
        new MaterialApple(),
        new MaterialFish(),
        new MaterialMeat()
    ];

    protected override IReadOnlyList<Card> RareCards => [
        new MaterialJam(),
        new MaterialCookedMeat(),
        new MaterialCookedFish()
    ];
}