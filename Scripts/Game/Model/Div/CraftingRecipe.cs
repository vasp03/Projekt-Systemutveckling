using System.Collections.Generic;

public class CraftingRecipe {
    public CraftingRecipe(string name, List<string> cardsForCrafting, List<string> cardsForCraftingResult) {
        Name = name;
        CardsForCrafting = cardsForCrafting;
        CardsForCraftingResult = cardsForCraftingResult;
    }

    public string Name { get; private set; }

    public List<string> CardsForCrafting { get; private set; }

    public List<string> CardsForCraftingResult { get; private set; }
}