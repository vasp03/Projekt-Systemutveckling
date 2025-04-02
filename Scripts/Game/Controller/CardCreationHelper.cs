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

	public TypeEnum GetRandomCardType() {
		Random random = new();
		Array values = Enum.GetValues(typeof(TypeEnum));
		TypeEnum type = (TypeEnum)values.GetValue(random.Next(values.Length));

		while (type == TypeEnum.Random) type = (TypeEnum)values.GetValue(random.Next(values.Length));

		return type;
	}

	public Card GetCreatedInstanceOfCard(TypeEnum type, Goodot15.Scripts.Game.Model.CardNode cardNode) {
		switch (type) {
			case TypeEnum.Apple:
				return new MaterialApple("Apple", 1, cardNode);
			// case CardCreationHelper.TypeEnum.Axe:
			//     return new MaterialAxe("Axe", true, 1);
			case TypeEnum.Berry:
				return new MaterialBerry("Berry", 1, cardNode);
			case TypeEnum.Leaves:
				return new MaterialLeaves("Leaves", 1, cardNode);
			case TypeEnum.Mine:
				return new MaterialMine("Mine", 1, cardNode);
			case TypeEnum.Plank:
				return new MaterialPlank("Planks", 1, cardNode);
			case TypeEnum.Stick:
				return new MaterialStick("Stick", 1, cardNode);
			case TypeEnum.Stone:
				return new MaterialStone("Stone", 1, cardNode);
			case TypeEnum.SwordMk1:
				return new MaterialSwordMk1("Sword Mk1", 1, cardNode);
			case TypeEnum.Tree:
				return new MaterialTree("Tree", 1, cardNode);
			case TypeEnum.Water:
				return new MaterialWater("Water", 1, cardNode);
			case TypeEnum.Wood:
				return new MaterialWood("Wood", 1, cardNode);
			case TypeEnum.Random:
				return GetCreatedInstanceOfCard(GetRandomCardType(), cardNode);
			default:
				GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type");
				return null;
		}
	}
}