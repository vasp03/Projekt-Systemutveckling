using System;
using Godot;

public partial class CardCreationHelper
{
    public enum TypeEnum
    {
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

    public CardCreationHelper.TypeEnum GetRandomCardType()
    {
        Random random = new Random();
        Array values = Enum.GetValues(typeof(TypeEnum));
        CardCreationHelper.TypeEnum type = (TypeEnum)values.GetValue(random.Next(values.Length));
        
        while (type == CardCreationHelper.TypeEnum.Random)
        {
            type = (TypeEnum)values.GetValue(random.Next(values.Length));
        }

        return type;
    }

    public Card GetCreatedInstanceOfCard(CardCreationHelper.TypeEnum type)
    {
        switch (type)
        {
            case CardCreationHelper.TypeEnum.Apple:
                return new MaterialApple("Apple", true, 1);
            // case CardCreationHelper.TypeEnum.Axe:
            //     return new MaterialAxe("Axe", true, 1);
            case CardCreationHelper.TypeEnum.Berry:
                return new MaterialBerry("Berry", true, 1);
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
            case CardCreationHelper.TypeEnum.Leaves:
                return new MaterialLeaves("Leaves", true, 1);
            case CardCreationHelper.TypeEnum.Mine:
                return new MaterialMine("Mine", true, 1);
            // case CardCreationHelper.TypeEnum.Oven:
            //     return new MaterialOven("Oven", true, 1);
            case CardCreationHelper.TypeEnum.Plank:
                return new MaterialPlank("Planks", true, 1);
            // case CardCreationHelper.TypeEnum.Sand:
            //     return new MaterialSand("Sand", true, 1);
            // case CardCreationHelper.TypeEnum.Shovel:
            //     return new MaterialShovel("Shovel", true, 1);
            case CardCreationHelper.TypeEnum.Stick:
                return new MaterialStick("Stick", true, 1);
            case CardCreationHelper.TypeEnum.Stone:
                return new MaterialStone("Stone", true, 1);
            case CardCreationHelper.TypeEnum.SwordMk1:
                return new MaterialSwordMk1("Sword Mk1", true, 1);
            // case CardCreationHelper.TypeEnum.Tent:
            //     return new MaterialTent("Tent", true, 1);
            case CardCreationHelper.TypeEnum.Tree:
                return new MaterialTree("Tree", true, 1);
            case CardCreationHelper.TypeEnum.Water:
                return new MaterialWater("Water", true, 1);
            case CardCreationHelper.TypeEnum.Wood:
                return new MaterialWood("Wood", true, 1);
            case CardCreationHelper.TypeEnum.Random:
                return GetCreatedInstanceOfCard(GetRandomCardType());
            default:
                GD.PrintErr("CardCreationHelper.GetCreatedInstanceOfCard: Invalid card type");
                return null;
        }
    }
}