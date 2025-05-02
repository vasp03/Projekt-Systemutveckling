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

    private readonly CardController _cardController;
    private readonly GameController _gameController;

    public CardCreationHelper(GameController gameController, CardController CardController) {
        _gameController = gameController;
        _cardController = CardController;
    }

    public string GetRandomCardType() {
        Random random = new();
        Array values = Enum.GetValues(typeof(TypeEnum));
        TypeEnum type = (TypeEnum)values.GetValue(random.Next(values.Length));

        while (type == TypeEnum.Random) type = (TypeEnum)values.GetValue(random.Next(values.Length));

        return type.ToString();
    }

    public Card GetCreatedInstanceOfCard(string type) {
        switch (type) {
            case "Wood" or "11":
                return new MaterialWood();
            case "Stone" or "12":
                return new MaterialStone();
            case "Water" or "13":
                return new MaterialWater();
            case "Stick" or "14":
                return new MaterialStick();
            case "Planks" or "15":
                return new MaterialPlank();
            case "Brick" or "16":
                return new MaterialBrick();
            case "Sand" or "17":
                return new MaterialSand();
            case "Glass" or "18":
                return new MaterialGlass();
            case "Leaves" or "19":
                return new MaterialLeaves();
            case "Clay" or "20":
                return new MaterialClay();

            case "SwordMK1" or "21":
                return new MaterialSwordMk1();
            case "FishingPole" or "22":
                return new MaterialFishingPole();
            case "Shovel" or "23":
                return new MaterialShovel();
            case "Axe" or "24":
                return new MaterialAxe();


            case "Greenhouse" or "25":
                return new BuildingGreenhouse();
            case "House" or "26":
                return new BuildingHouse();
            case "Campfire" or "27":
                return new BuildingCampfire();
            case "Cookingpot" or "28":
                return new BuildingCookingpot();
            case "Tent" or "29":
                return new BuildingTent();
            case "Field" or "44":
                return new BuildingTent();


            case "Apple" or "30":
                return new MaterialApple();
            case "Berry" or "31":
                return new MaterialBerry();
            case "Jam" or "32":
                return new MaterialJam();
            case "Meat" or "33":
                return new MaterialMeat();
            case "CookedMeat" or "34":
                return new MaterialCookedMeat();
            case "Fish" or "35":
                return new MaterialFish();
            case "CookedFish" or "36":
                return new MaterialCookedFish();
            case "Tree" or "37":
                return new MaterialTree();
            case "Mine" or "38":
                return new MaterialMine();
            case "Bush" or "39":
                return new MaterialBush();


            case "Village" or "40":
                return new PlayerVillager();
            case "Hunter" or "41":
                return new PlayerHunter();
            case "Farmer" or "42":
                return new PlayerFarmer();
            case "Blacksmith" or "43":
                return new PlayerBlacksmith();

            case "Random":
                return GetCreatedInstanceOfCard(GetRandomCardType());
            default:
                GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type. Tried to create: " + type);
                return new ErrorCard();
        }
    }
}