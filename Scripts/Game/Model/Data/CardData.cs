using System;

namespace Goodot15.Scripts.Game.Model.Data;

public class CardData {
    public string CardTypeName { get; set; }
    public Type CardTypeClass => Type.GetType(this.CardTypeName);
    
    public object CardTypeData { get; set; }
}