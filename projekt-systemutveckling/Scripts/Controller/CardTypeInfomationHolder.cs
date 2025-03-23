using Godot;
using System;

public partial class CardTypeInfomationHolder : Node
{
    string textureAddress;
    CardTypeEnum.TypeEnum type;

    public CardTypeInfomationHolder(string textureAddress, CardTypeEnum.TypeEnum type)
    {
        this.textureAddress = textureAddress;
        this.type = type;
    }

    public string GetTexture()
    {
        return textureAddress;
    }

    public CardTypeEnum.TypeEnum GetCardType()
    {
        return type;
    }
}
