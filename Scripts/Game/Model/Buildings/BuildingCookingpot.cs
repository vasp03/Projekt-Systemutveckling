using System;
using Goodot15.Scripts.Game.Model.Parents;

public class BuildingCookingpot(string textureAddress, bool movable, int cost, int produceTimeInSeconds, CardNode cardNode) : CardBuilding(textureAddress, movable, cost, produceTimeInSeconds, cardNode) {
    private String cardToProduce;
    
    public override String ProduceCard() {
        return cardToProduce;
    }
}