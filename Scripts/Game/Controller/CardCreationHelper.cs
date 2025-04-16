using System;
using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;

public class CardCreationHelper {
    public enum TypeEnum {
        Wood,
        Stone,
        Water,
        Stick,
        Planks,
        Brick,
        Sand,
        Glass,
        Leaf,
        Clay,
        SwordMK1,
        FishingPole,
        Shovel,
        Axe,
        Greenhouse,
        House,
        Campfire,
        Cookingpot,
        Tent,
        Field,
        Apple,
        Berry,
        Jam,
        Meat,
        CookedMeat,
        Fish,
        CookedFish,
        Tree,
        Mine,
        Bush,
        Village,
        Hunter,
        Farmer,
        Blacksmith,
        Random
    }

    private readonly CardController CardController;
    private readonly GameController GameController;

    public CardCreationHelper(GameController NodeController, CardController CardController) {
        GameController = NodeController;
        this.CardController = CardController;
    }

    public string GetRandomCardType() {
        Random random = new();
        Array values = Enum.GetValues(typeof(TypeEnum));
        TypeEnum type = (TypeEnum)values.GetValue(random.Next(values.Length));

        while (type == TypeEnum.Random) type = (TypeEnum)values.GetValue(random.Next(values.Length));

        return type.ToString();
    }

    public Card GetCreatedInstanceOfCard(string type, CardNode cardNode) {
        switch (type) {
            case "Wood" or "11":
                return new MaterialWood("Wood", 1, cardNode);
            case "Stone" or "12":
                return new MaterialStone("Stone", 1, cardNode);
            case "Water" or "13":
                return new MaterialWater("Water", 1, cardNode);
            case "Stick" or "14":
                return new MaterialStick("Stick", 1, cardNode);
            case "Planks" or "15":
                return new MaterialPlank("Planks", 1, cardNode);
            case "Brick" or "16":
                return new MaterialBrick("Brick", 1, cardNode);
            case "Sand" or "17":
                return new MaterialSand("Sand", 1, cardNode);
            case "Glass" or "18":
                return new MaterialGlass("Glass", 1, cardNode);
            case "Leaves" or "19":
                return new MaterialLeaves("Leaves", 1, cardNode);



            case "SwordMK1" or "21":
                return new MaterialSwordMk1("SwordMK1", 1, cardNode);
            case "FishingPole" or "22":
                return new MaterialFishingPole("FishingPole", 1, cardNode);
            case "Shovel" or "23":
                return new MaterialShovel("Shovel", 1, cardNode);
            case "Axe" or "24":
                return new MaterialAxe("Axe", 1, cardNode);



            case "Greenhouse" or "25":
                return new BuildingGreenhouse("Greenhouse", true, 1, 1, cardNode);
            case "House" or "26":
                return new BuildingHouse("House", true, 1, 1, cardNode);
            case "Campfire" or "27":
                return new BuildingCampfire("Campfire", true, 1, 1, cardNode);
            case "Cookingpot" or "28":
                return new BuildingCookingpot("Cookingpot", true, 1, 1, cardNode);
            case "Tent" or "29":
                return new BuildingTent("Tent", true, 1, 1, cardNode);
            case "Field" or "44":
                return new BuildingTent("Tent", true, 1, 1, cardNode);



            case "Apple" or "30":
                return new MaterialApple("Apple", 1, cardNode);
            case "Berry" or "31":
                return new MaterialBerry("Berry", 1, cardNode);
            case "Jam" or "32":
                return new MaterialJam("Jam", 1, cardNode);
            case "Meat" or "33":
                return new MaterialMeat("Meat", 1, cardNode);
            case "CookedMeat" or "34":
                return new MaterialCookedMeat("CookedMeat", 1, cardNode);
            case "Fish" or "35":
                return new MaterialFish("Fish", 1, cardNode);
            case "CookedFish" or "36":
                return new MaterialCookedFish("CookedFish", 1, cardNode);
            case "Tree" or "37":
                return new MaterialTree("Tree", 1, cardNode);
            case "Mine" or "38":
                return new MaterialMine("Mine", 1, cardNode);
            case "Bush" or "39":
                return new MaterialBush("Bush", 1, cardNode);



            case "Village" or "40":
                return new PlayerVillager("Village", true, 1, 1, cardNode);
            case "Hunter" or "41":
                return new PlayerHunter("Hunter", true, 1, 1, cardNode);
            case "Farmer" or "42":
                return new PlayerFarmer("Farmer", true, 1, 1, cardNode);
            case "Blacksmith" or "43":
                return new PlayerBlacksmith("Blacksmith", true, 1, 1, cardNode);

            case "Random":
                return GetCreatedInstanceOfCard(GetRandomCardType(), cardNode);
            default:
                GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type. Tried to create: " + type);
                return new ErrorCard("Error", true, 0, cardNode);
        }
    }

    public void CreateCard(string type) {
        PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
        CardNode cardInstance = cardScene.Instantiate<CardNode>();

        bool ret = cardInstance.CreateNode(GetCreatedInstanceOfCard(type, cardInstance), CardController);

        if (!ret) {
            GD.PrintErr("CardCreationHelper.CreateCard: Card creation failed");
            return;
        }

        cardInstance.ZIndex = CardController.CardCount + 1;
        cardInstance.SetPosition(new Vector2(100, 100));
        GameController.AddChild(cardInstance);
    }
}