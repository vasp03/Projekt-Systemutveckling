using Godot;
using System;

public partial class CardTypeEnum : Node
{
    private static string textureAddress = "res://Assets/Cards/Ready To Use/";

    public enum TypeEnum
    {
        Wood,
        Rock,
        Water,
        Stick,
        Planks,
        Leaves,
        Sword,
        Apple,
        Berry,
        Tree,
        Mine,
        Random
    }

    public static CardTypeEnum.TypeEnum GetRandomCardType()
    {
        Random random = new Random();
        Array values = Enum.GetValues(typeof(TypeEnum));
        return (TypeEnum)values.GetValue(random.Next(values.Length));
    }

    public static Texture GetTexture(CardTypeEnum.TypeEnum type)
    {
        string texture = "";

        if (type == TypeEnum.Random)
        {
            TypeEnum randomType = GetRandomCardType();
            while (randomType == TypeEnum.Random)
            {
                randomType = GetRandomCardType();
            }
            texture = GetTextureAddress(randomType);
        }
        else
        {
            texture = GetTextureAddress(type);
        }

        return GD.Load<Texture>(texture);
    }

    private static string GetTextureAddress(CardTypeEnum.TypeEnum type)
    {
        string texture = "";

        switch (type)
        {
            case TypeEnum.Wood:
                texture = textureAddress + "Wood.png";
                break;
            case TypeEnum.Rock:
                texture = textureAddress + "Rock.png";
                break;
            case TypeEnum.Water:
                texture = textureAddress + "Water.png";
                break;
            case TypeEnum.Stick:
                texture = textureAddress + "Stick.png";
                break;
            case TypeEnum.Planks:
                texture = textureAddress + "Planks.png";
                break;
            case TypeEnum.Leaves:
                texture = textureAddress + "Leaves.png";
                break;
            case TypeEnum.Sword:
                texture = textureAddress + "Sword Mk1.png";
                break;
            case TypeEnum.Apple:
                texture = textureAddress + "Apple.png";
                break;
            case TypeEnum.Berry:
                texture = textureAddress + "Berry.png";
                break;
            case TypeEnum.Tree:
                texture = textureAddress + "Tree.png";
                break;
            case TypeEnum.Mine:
                texture = textureAddress + "Mine.png";
                break;
            default:
                GD.Print("Texture for card not found");
                break;
        }

        GD.Print("--------------------");
        GD.Print("Texture: " + texture + " Type: " + type);

        return texture;
    }
}
