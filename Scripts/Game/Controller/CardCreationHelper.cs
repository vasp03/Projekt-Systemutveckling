using System;
using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;

public class CardCreationHelper {
	private readonly NodeController NodeController;
	private readonly CardController CardController;

	public CardCreationHelper(NodeController NodeController, CardController CardController) {
		this.NodeController = NodeController;
		this.CardController = CardController;
	}

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
				return new MaterialApple(1, cardNode);
			case "Berry":
				return new MaterialBerry(1, cardNode);
			case "Leaves":
				return new MaterialLeaves(1, cardNode);
			case "Mine":
				return new MaterialMine(1, cardNode);
			case "Plank":
				return new MaterialPlank( 1, cardNode);
			case "Stick":
				return new MaterialStick(1, cardNode);
			case "Stone":
				return new MaterialStone(1, cardNode);
			case "SwordMk1":
				return new MaterialSwordMk1(1, cardNode);
			case "Tree":
				return new MaterialTree(1, cardNode);
			case "Water":
				return new MaterialWater(1, cardNode);
			case "Wood":
				return new MaterialWood(1, cardNode);
			case "Random":
				return GetCreatedInstanceOfCard(GetRandomCardType(), cardNode);
			default:
				GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type");
				return  new ErrorCard("Error", true, 0, cardNode);
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
		NodeController.AddChild(cardInstance);
	}
}