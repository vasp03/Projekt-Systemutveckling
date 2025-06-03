using System.Collections.Generic;
using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class BasicToolsPack : CommonAndRarePack{
    public override string PackButtonTexture => PackTexture("Tools Pack");

    public override bool SingleUse => false;

    public override string Name => "Basic Tools pack";

    public override int Cost => 150;

    protected override float RareCardChance => 0.2f;

    protected override int CardCount => GD.RandRange(1, 2);

    protected override IReadOnlyList<Card> CommonCards => [
        new MaterialAxe(),
        new MaterialFishingPole(),
        new MaterialShovel()
    ];

    protected override IReadOnlyList<Card> RareCards => [
        new MaterialSwordMk1()
    ];
}