using System;
using Godot;
using Goodot15.Scripts.Game.Model.Material_Cards;

public class CardCreationHelper {
	public enum TypeEnum {
		Apple,

		// Axe,
		Berry,

		// Brick,
		// Bush,
		// Farm,
		// Fireplace,
		// Fishingrod,
		// Glass,
		// Greenhouse,
		// House,
		// Jam,
		Leaves,
		Mine,

		// Oven,
		Plank,

		// Sand,
		// Shovel,
		Stick,
		Stone,
		SwordMk1,

		// Tent,
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

	public Card GetCreatedInstanceOfCard(TypeEnum type) {
		switch (type) {
			case TypeEnum.Apple:
				return new MaterialApple("Apple", 1);
			// case CardCreationHelper.TypeEnum.Axe:
			//     return new MaterialAxe("Axe", true, 1);
			case TypeEnum.Berry:
				return new MaterialBerry("Berry", 1);
			// case CardCreationHelper.TypeEnum.Brick:
			//     return new MaterialBrick("Brick", true, 1);
			// case CardCreationHelper.TypeEnum.Bush:
			//     return new MaterialBush("Bush", true, 1);
			// case CardCreationHelper.TypeEnum.Farm:
			//     return new MaterialFarm("Farm", true, 1);
			// case CardCreationHelper.TypeEnum.Fireplace:
			//     return new MaterialFireplace("Fireplace", true, 1);
			// case CardCreationHelper.TypeEnum.Fishingrod:
			//     return new MaterialFishingrod("Fishingrod", true, 1);
			// case CardCreationHelper.TypeEnum.Glass:
			//     return new MaterialGlass("Glass", true, 1);
			// case CardCreationHelper.TypeEnum.Greenhouse:
			//     return new MaterialGreenhouse("Greenhouse", true, 1);
			// case CardCreationHelper.TypeEnum.House:
			//     return new MaterialHouse("House", true, 1);
			// case CardCreationHelper.TypeEnum.Jam:
			//     return new MaterialJam("Jam", true, 1);
			case TypeEnum.Leaves:
				return new MaterialLeaves("Leaves", 1);
			case TypeEnum.Mine:
				return new MaterialMine("Mine", 1);
			// case CardCreationHelper.TypeEnum.Oven:
			//     return new MaterialOven("Oven", true, 1);
			case TypeEnum.Plank:
				return new MaterialPlank("Planks", 1);
			// case CardCreationHelper.TypeEnum.Sand:
			//     return new MaterialSand("Sand", true, 1);
			// case CardCreationHelper.TypeEnum.Shovel:
			//     return new MaterialShovel("Shovel", true, 1);
			case TypeEnum.Stick:
				return new MaterialStick("Stick", 1);
			case TypeEnum.Stone:
				return new MaterialStone("Stone", 1);
			case TypeEnum.SwordMk1:
				return new MaterialSwordMk1("Sword Mk1", 1);
			// case CardCreationHelper.TypeEnum.Tent:
			//     return new MaterialTent("Tent", true, 1);
			case TypeEnum.Tree:
				return new MaterialTree("Tree", 1);
			case TypeEnum.Water:
				return new MaterialWater("Water", 1);
			case TypeEnum.Wood:
				return new MaterialWood("Wood", 1);
			case TypeEnum.Random:
				return GetCreatedInstanceOfCard(GetRandomCardType());
			default:
				GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type");
				return null;
		}
	}
}