using System;
using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;

public class CardCreationHelper {
    public enum TypeEnum {
        Apple,
        Berry,
        Leaves,
        Mine,
        Plank,
        Stick,
        Stone,
        SwordMk1,
        Tree,
        Water,
        Wood,
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
            case "Apple":
                return new MaterialApple("Apple", 1, cardNode);
            case "Axe":
                return new MaterialAxe("Axe", 1, cardNode);
            case "Berry":
                return new MaterialBerry("Berry", 1, cardNode);
            case "Blacksmith":
                return new ErrorCard("Error", true, 0, cardNode);
            // return new MaterialBlacksmith("Blacksmith", 1, cardNode);
            case "Brick":
                return new MaterialBrick("Brick", 1, cardNode);
            case "Bush":
                return new MaterialBush("Bush", 1, cardNode);
            case "Campfire":
                return new ErrorCard("Error", true, 0, cardNode);
            // return new MaterialCampfire("Campfire", 1, cardNode);
            case "CookedFish":
                return new MaterialCookedFish("CookedFish", 1, cardNode);
            case "CookedMeat":
                return new MaterialCookedMeat("CookedMeat", 1, cardNode);
            case "CookingPot":
                return new ErrorCard("Error", true, 0, cardNode);
            // return new MaterialCookingPot("CookingPot", 1, cardNode);
            case "Farmer":
                return new PlayerFarmer("Farmer", true, 0, 0, cardNode);
            case "Field":
                return new ErrorCard("Error", true, 0, cardNode);
            // return new MaterialField("Field", 1, cardNode);
            case "Fish":
                return new MaterialFish("Fish", 1, cardNode);
            case "FishingPole":
                return new MaterialFishingPole("FishingPole", 1, cardNode);
            case "Glass":
                return new MaterialGlass("Glass", 1, cardNode);
            case "Greenhouse":
                return new ErrorCard("Error", true, 0, cardNode);
            // return new MaterialGreenhouse("Greenhouse", 1, cardNode);
            case "House":
                return new ErrorCard("Error", true, 0, cardNode);
            // return new MaterialHouse("House", 1, cardNode);
            case "Hunter":
                return new PlayerHunter("Hunter", true, 0, 0, cardNode);
            case "Jam":
                return new MaterialJam("Jam", 1, cardNode);
            case "Leaves":
                return new MaterialLeaves("Leaves", 1, cardNode);
            case "Meat":
                return new MaterialMeat("Meat", 1, cardNode);
            case "Mine":
                return new MaterialMine("Mine", 1, cardNode);
            case "Planks":
                return new MaterialPlank("Planks", 1, cardNode);
            case "Sand":
                return new MaterialSand("Sand", 1, cardNode);
            case "Stick":
                return new MaterialStick("Stick", 1, cardNode);
            case "Stone":
                return new MaterialStone("Stone", 1, cardNode);
            case "SwordMK1":
                return new MaterialSwordMk1("SwordMK1", 1, cardNode);
            case "Tent":
                return new ErrorCard("Error", true, 0, cardNode);
            // return new MaterialTent("Tent", 1, cardNode);
            case "Tree":
                return new MaterialTree("Tree", 1, cardNode);
            case "Villager":
                return new PlayerVillager("Villager", true, 0, 0, cardNode);
            case "Water":
                return new MaterialWater("Water", 1, cardNode);
            case "Wood":
                return new MaterialWood("Wood", 1, cardNode);
            case "Random":
                return GetCreatedInstanceOfCard(GetRandomCardType(), cardNode);
            default:
                GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type");
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