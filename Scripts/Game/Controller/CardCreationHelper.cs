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
            case "Apple":
                return new MaterialApple();
            case "Berry":
                return new MaterialBerry();
            case "Leaves":
                return new MaterialLeaves();
            case "Mine":
                return new MaterialMine();
            case "Plank":
                return new MaterialPlank();
            case "Stick":
                return new MaterialStick();
            case "Stone":
                return new MaterialStone();
            case "SwordMk1":
                return new MaterialSwordMk1();
            case "Tree":
                return new MaterialTree();
            case "Water":
                return new MaterialWater();
            case "Wood":
                return new MaterialWood();
            case "Random":
                return GetCreatedInstanceOfCard(GetRandomCardType());
            default:
                GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type");
                return new ErrorCard();
        }
    }

    public void CreateCard(string type) {
        PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
        CardNode cardInstance = cardScene.Instantiate<CardNode>();
        
        _cardController.CreateCard(GetCreatedInstanceOfCard(type), Vector2.One*100);

        cardInstance.ZIndex = _cardController.CardCount + 1;
        cardInstance.SetPosition(new Vector2(100, 100));
        _gameController.AddChild(cardInstance);
    }
}