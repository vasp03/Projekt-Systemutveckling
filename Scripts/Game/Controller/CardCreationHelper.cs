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

	public Card GetCreatedInstanceOfCard(TypeEnum type, CardNode cardNode) {
		switch (type) {
			case TypeEnum.Apple:
				return new MaterialApple(1, cardNode);
			// case CardCreationHelper.TypeEnum.Axe:
			//     return new MaterialAxe("Axe", true, 1);
			case TypeEnum.Berry:
				return new MaterialBerry(1, cardNode);
			case TypeEnum.Leaves:
				return new MaterialLeaves(1, cardNode);
			case TypeEnum.Mine:
				return new MaterialMine(1, cardNode);
			case TypeEnum.Plank:
				return new MaterialPlank(1, cardNode);
			case TypeEnum.Stick:
				return new MaterialStick(1, cardNode);
			case TypeEnum.Stone:
				return new MaterialStone(1, cardNode);
			case TypeEnum.SwordMk1:
				return new MaterialSwordMk1(1, cardNode);
			case TypeEnum.Tree:
				return new MaterialTree(1, cardNode);
			case TypeEnum.Water:
				return new MaterialWater(1, cardNode);
			case TypeEnum.Wood:
				return new MaterialWood(1, cardNode);
			case TypeEnum.Random:
				return GetCreatedInstanceOfCard(GetRandomCardType(), cardNode);
			default:
				GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type");
				return null;
		}
	}
}