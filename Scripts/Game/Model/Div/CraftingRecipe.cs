using System.Collections.Generic;

namespace Goodot15.Scripts.Game.Model.Div;

public class CraftingRecipe {
    public CraftingRecipe(string name, List<string> cardsForCrafting, List<string> cardsForCraftingResult,
        bool consumeTool = false) {
        Name = name;
        CardsForCrafting = cardsForCrafting;
        CardsForCraftingResult = cardsForCraftingResult;
        ConsumeTool = consumeTool;
    }

    public string Name { get; private set; }
    public List<string> CardsForCrafting { get; private set; }
    public List<string> CardsForCraftingResult { get; private set; }
    public bool ConsumeTool { get; private set; }
}