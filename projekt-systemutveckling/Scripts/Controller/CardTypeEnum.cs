using Godot;
using System;

public partial class CardTypeEnum : Node
{
    public enum TypeEnum
    {
        wood,
        rock,
        water,
        stick,
        planks,
        leaves,
        sword,
        apple,
        berry,
        tree,
        mine
    }

    public static CardTypeEnum GetRandomCardType()
    {
        var values = Enum.GetValues(typeof(CardTypeEnum));
        var random = new Random();
        return (CardTypeEnum)values.GetValue(random.Next(values.Length));
    }

    public static Texture GetTexture(CardTypeEnum.TypeEnum type)
    {
        Texture texture;

        switch (type)
        {
            case TypeEnum.wood:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Wood.png");
                break;
            case TypeEnum.rock:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Rock.png");
                break;
            case TypeEnum.water:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Water.png");
                break;
            case TypeEnum.stick:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Stick.png");
                break;
            case TypeEnum.planks:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Planks.png");
                break;
            case TypeEnum.leaves:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Leaves.png");
                break;
            case TypeEnum.sword:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Sword Mk1.png");
                break;
            case TypeEnum.apple:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Apple.png");
                break;
            case TypeEnum.berry:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Berry.png");
                break;
            case TypeEnum.tree:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Tree.png");
                break;
            case TypeEnum.mine:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Mine.png");
                break;
            default:
                texture = GD.Load<Texture>("res://Assets/Cards/Ready To Use/Wood.png");
                break;
        }

        return texture;
    }

    public static explicit operator CardTypeEnum(int v)
    {
        throw new NotImplementedException();
    }

}
