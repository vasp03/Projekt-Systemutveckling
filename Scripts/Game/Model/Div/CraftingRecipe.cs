using System.Collections.Generic;

namespace Goodot15.Scripts.Game.Model.Div;

public class CraftingRecipe(
    string name,
    IReadOnlyCollection<string> craftingIngredients,
    IReadOnlyCollection<string> craftingResult,
    IReadOnlyCollection<string> itemsToRemove) {
    public string Name { get; private set; } = name;
    public IReadOnlyCollection<string> CraftingIngredients { get; private set; } = craftingIngredients;
    public IReadOnlyCollection<string> CraftingResult { get; private set; } = craftingResult;
    public IReadOnlyCollection<string> ItemsToRemove { get; private set; } = itemsToRemove;
}