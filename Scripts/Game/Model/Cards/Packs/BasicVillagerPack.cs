using System.Collections.Generic;
using Goodot15.Scripts.Game.Model.Living.Villagers;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Model.Cards.Packs;

public class BasicVillagerPack : CommonAndRarePack {
    public override string PackButtonTexture => PackTexture("Villager Pack");

    public override bool SingleUse => false;

    public override string Name => "Basic Villager pack";

    public override int Cost => 1000;

    protected override int CardCount => 1;

    protected override float RareCardChance => 0.5f;

    protected override IReadOnlyList<Card> CommonCards => [
        new PlayerVillager()
    ];

    protected override IReadOnlyList<Card> RareCards => [
        new PlayerBlacksmith(),
        new PlayerFarmer(),
        new PlayerHunter()
    ];
}