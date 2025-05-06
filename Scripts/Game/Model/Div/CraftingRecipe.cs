using System.Collections.Generic;

public class CraftingRecipe {
    public string Name { get; private set; }
    public List<string> CardsForCrafting { get; private set; }
    public List<string> CardsForCraftingResult { get; private set; }
    public bool ConsumeTool { get; private set; }

    public CraftingRecipe(string name, List<string> cardsForCrafting, List<string> cardsForCraftingResult, bool consumeTool = false) {
        Name = name;
        CardsForCrafting = cardsForCrafting;
        CardsForCraftingResult = cardsForCraftingResult;
        ConsumeTool = consumeTool;
    }
}