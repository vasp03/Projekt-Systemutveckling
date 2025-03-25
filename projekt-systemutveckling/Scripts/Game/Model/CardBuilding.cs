using System;

public partial class CardBuilding(String textureAddress, bool movable, int cost, int produceTime, Card cardToProduce) : Card(textureAddress, movable, cost)
{
    private int produceTime = produceTime;
    private Card cardToProduce = cardToProduce;
}
