using System.Collections.Generic;

namespace Goodot15.Scripts.Game.Model.Div;

/// <summary>
/// </summary>
/// <param name="name">Display name of Recipe</param>
/// <param name="craftingIngredients">Input for the crafting recipe</param>
/// <param name="craftingResult">Output of the crafting recipe</param>
/// <param name="itemsToRemove">Collection of items to not be removed when crafting</param>
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