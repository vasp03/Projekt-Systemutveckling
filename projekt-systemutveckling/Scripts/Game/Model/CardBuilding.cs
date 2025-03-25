using System;

public partial class CardBuilding(String name, String textureAddress, bool movable, int cost, int produceTime, Card cardToProduce) : Card(name, textureAddress, movable, cost)
{
    private int produceTime = produceTime;
    private Card cardToProduce = cardToProduce;
}
