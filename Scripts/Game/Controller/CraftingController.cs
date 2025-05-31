using System;
using System.Collections.Generic;
using System.Linq;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Controller;

public class CraftingController {
    public CraftingController() {
        CreateStartingRecipes();
    }

    /// <summary>
    ///     Check if the cards in the stack can be crafted into a new card
    ///     Returns a list of the cards that can be crafted
    ///     If no cards can be crafted, returns null
    /// </summary>
    /// <param name="Cards">List of cards to check</param>
    public Pair<IReadOnlyCollection<string>, IReadOnlyCollection<string>> CheckForCraftingWithStackable(
        IReadOnlyList<Card> Cards) {
        List<Pair<string, int>> CardForCraftingAmount = [];

        foreach (Card card in Cards) {
            Pair<string, int> cardForCrafting =
                CardForCraftingAmount.FirstOrDefault(x => x.Left == card.CardName);
            if (cardForCrafting is not null)
                cardForCrafting.Right++;
            else
                CardForCraftingAmount.Add(new Pair<string, int>(card.CardName, 1));
        }

        // Sort the list by the name of the card
        CardForCraftingAmount.Sort((x, y) => x.Left.CompareTo(y.Left));

        foreach (CraftingRecipe recipe in recipes) {
            List<Pair<string, int>> CardsInRecipeAndAmount = [];

            foreach (string cardName in recipe.CraftingIngredients) {
                Pair<string, int> cardInRecipe = CardsInRecipeAndAmount.FirstOrDefault(x => x.Left == cardName);
                if (cardInRecipe is not null)
                    cardInRecipe.Right++;
                else
                    CardsInRecipeAndAmount.Add(new Pair<string, int>(cardName, 1));
            }

            // Sort the list by the name of the card
            CardsInRecipeAndAmount.Sort((x, y) => x.Left.CompareTo(y.Left));

            // Check if the recipe matches the cards in the stack
            bool recipeMatches = true;

            if (CardsInRecipeAndAmount.Count != CardForCraftingAmount.Count) continue;

            for (int i = 0; i < CardsInRecipeAndAmount.Count; i++)
                if (CardsInRecipeAndAmount[i].Left != CardForCraftingAmount[i].Left ||
                    CardsInRecipeAndAmount[i].Right != CardForCraftingAmount[i].Right) {
                    recipeMatches = false;
                    break;
                }

            if (recipeMatches) {
                IReadOnlyCollection<string> craftedCards = recipe.CraftingResult;
                return new Pair<IReadOnlyCollection<string>, IReadOnlyCollection<string>>(craftedCards.ToList(),
                    recipe.ItemsToRemove);
            }
        }

        return null;
    }

    /// <summary>
    ///     Creates the starting recipes for crafting.
    /// </summary>
    public void CreateStartingRecipes() {
        AddRecipe(new CraftingRecipe("Jam", ["Berry", "Berry", "Berry", "Berry", "Berry", "Campfire", "Cookingpot"],
            ["Jam"], ["Berry"]));

        AddRecipe(new CraftingRecipe("Stick", ["Villager", "Wood", "Axe"], ["Stick"], ["Wood"]));
        AddRecipe(new CraftingRecipe("Stick", ["Hunter", "Wood", "Axe"], ["Stick"], ["Wood"]));
        AddRecipe(new CraftingRecipe("Stick", ["Blacksmith", "Wood", "Axe"], ["Stick"], ["Wood"]));
        AddRecipe(new CraftingRecipe("Stick", ["Farmer", "Wood", "Axe"], ["Stick"], ["Wood"]));

        AddRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Hunter"], ["Fish"], []));
        AddRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Blacksmith"], ["Fish"], []));
        AddRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Farmer"], ["Fish"], []));
        AddRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Villager"], ["Fish"], []));

        AddRecipe(new CraftingRecipe("Axe", ["Stone", "Stick", "Stick"], ["Axe"], ["Stone", "Stick"]));

        AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Villager"], ["Wood", "Wood", "Wood"], ["Tree"]));
        AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Hunter"], ["Wood", "Wood", "Wood"], ["Tree"]));
        AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Blacksmith"], ["Wood", "Wood", "Wood"], ["Tree"]));
        AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Farmer"], ["Wood", "Wood", "Wood", "Wood", "Wood"],
            ["Tree"]));

        AddRecipe(new CraftingRecipe("Stone", ["Mine", "Villager"], ["Stone"], []));
        AddRecipe(new CraftingRecipe("Stone", ["Mine", "Hunter"], ["Stone"], []));
        AddRecipe(new CraftingRecipe("Stone", ["Mine", "Blacksmith"], ["Stone", "Stone", "Stone"], []));
        AddRecipe(new CraftingRecipe("Stone", ["Mine", "Farmer"], ["Stone"], []));
        AddRecipe(new CraftingRecipe("Stone", ["Meteorite", "Villager", "Axe"],
            ["Stone", "Stone", "Stone", "Stone", "Stone"], ["Meteorite"]));
        AddRecipe(new CraftingRecipe("Stone", ["Meteorite", "Hunter", "Axe"],
            ["Stone", "Stone", "Stone", "Stone", "Stone"], ["Meteorite"]));
        AddRecipe(new CraftingRecipe("Stone", ["Meteorite", "Blacksmith", "Axe"],
            ["Stone", "Stone", "Stone", "Stone", "Stone"], ["Meteorite"]));
        AddRecipe(new CraftingRecipe("Stone", ["Meteorite", "Farmer", "Axe"],
            ["Stone", "Stone", "Stone", "Stone", "Stone"], ["Meteorite"]));

        AddRecipe(new CraftingRecipe("Tent", ["Leaves", "Leaves", "Leaves", "Leaves", "Wood"], ["Tent"],
            ["Leaves", "Wood"]));

        AddRecipe(new CraftingRecipe("Berry", ["Bush", "Villager"], ["Berry", "Berry", "Berry"], ["Bush"]));
        AddRecipe(new CraftingRecipe("Berry", ["Bush", "Hunter"], ["Berry", "Berry", "Berry"], ["Bush"]));
        AddRecipe(new CraftingRecipe("Berry", ["Bush", "Blacksmith"], ["Berry", "Berry", "Berry"], ["Bush"]));
        AddRecipe(new CraftingRecipe("Berry", ["Bush", "Farmer"], ["Berry", "Berry", "Berry", "Berry", "Berry"],
            ["Bush"]));

        AddRecipe(new CraftingRecipe("Leaves", ["Villager", "Tree"], ["Leaves", "Leaves", "Apple", "Wood", "Wood"],
            ["Tree"]));
        AddRecipe(new CraftingRecipe("Leaves", ["Hunter", "Tree"], ["Leaves", "Leaves", "Apple", "Wood", "Wood"],
            ["Tree"]));
        AddRecipe(new CraftingRecipe("Leaves", ["Blacksmith", "Tree"],
            ["Leaves", "Leaves", "Apple", "Wood", "Wood", "Wood", "Wood"], ["Tree"]));
        AddRecipe(new CraftingRecipe("Leaves", ["Farmer", "Tree"], ["Leaves", "Leaves", "Apple", "Wood", "Wood"],
            ["Tree"]));

        AddRecipe(new CraftingRecipe("FishingPole", ["Stick", "Stone"], ["FishingPole"], ["Stick", "Stone"]));

        AddRecipe(new CraftingRecipe("CookedFish", ["Fish", "Campfire"], ["CookedFish"], ["Fish"]));

        AddRecipe(new CraftingRecipe("CookedMeat", ["Meat", "Campfire"], ["CookedMeat"], ["Meat"]));

        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Hunter", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Hunter", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Blacksmith", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Blacksmith", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Farmer", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Farmer", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Hunter", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Hunter", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Blacksmith", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Blacksmith", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Farmer", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Farmer", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Blacksmith", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Blacksmith", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Farmer", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Farmer", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("Farmer", ["Farmer", "Farmer", "House"], ["Villager"], []));
        AddRecipe(new CraftingRecipe("Farmer", ["Farmer", "Farmer", "Tent"], ["Villager"], []));

        AddRecipe(new CraftingRecipe("House",
            ["Stone", "Stone", "Stone", "Stone", "Planks", "Planks", "Brick", "Brick", "Brick", "Brick"], ["House"],
            ["Stone", "Planks", "Brick"]));

        AddRecipe(new CraftingRecipe("Greenhouse", ["Brick", "Brick", "Glass", "Glass", "Glass", "Glass"],
            ["Greenhouse"], ["Brick", "Glass"]));

        AddRecipe(new CraftingRecipe("Clay", ["Sand", "Water"], ["Clay"], ["Sand", "Water"]));

        AddRecipe(new CraftingRecipe("Brick", ["Clay", "Campfire"], ["Brick"], ["Clay"]));

        AddRecipe(new CraftingRecipe("SwordMK1", ["Wood", "Wood", "Stone"], ["SwordMK1"], ["Wood", "Stone"]));

        AddRecipe(new CraftingRecipe("Planks", ["Wood", "Wood"], ["Planks"], ["Wood"]));

        AddRecipe(new CraftingRecipe("Sand", ["Stone", "Villager"], ["Sand"], ["Stone"]));
        AddRecipe(new CraftingRecipe("Sand", ["Stone", "Hunter"], ["Sand"], ["Stone"]));
        AddRecipe(new CraftingRecipe("Sand", ["Stone", "Blacksmith"], ["Sand", "Sand", "Sand"], ["Stone"]));
        AddRecipe(new CraftingRecipe("Sand", ["Stone", "Farmer"], ["Sand"], ["Stone"]));

        AddRecipe(new CraftingRecipe("Water", ["Water", "Water"], ["Water", "Water", "Water"], ["Water"]));

        AddRecipe(new CraftingRecipe("Glass", ["Sand", "Campfire"], ["Glass"], ["Sand"]));

        AddRecipe(new CraftingRecipe("Shovel", ["Stick", "Stick", "Stone", "Stone"], ["Shovel"], ["Stick", "Stone"]));

        AddRecipe(new CraftingRecipe("Field", ["Sand", "Sand", "Sand", "Sand", "Stone", "Stone", "Water"], ["Field"],
            ["Sand", "Stone", "Water"]));

        AddRecipe(new CraftingRecipe("Campfire", ["Wood", "Wood", "Wood", "Stick", "Stick", "Leaves"], ["Campfire"],
            ["Wood", "Stick", "Leaves"]));

        AddRecipe(new CraftingRecipe("Cookingpot", ["Clay", "Clay", "Stick"], ["CookingPot"], ["Clay", "Stick"]));

        AddRecipe(new CraftingRecipe("Bush", ["Leaves", "Leaves", "Leaves", "Leaves", "Leaves", "Leaves"], ["Bush"],
            ["Leaves"]));

        AddRecipe(new CraftingRecipe("Meat", ["Field", "Villager", "Tree", "Sword"], ["Meat"], []));
        AddRecipe(new CraftingRecipe("Meat", ["Field", "Hunter", "Tree", "Sword"], ["Meat", "Meat", "Meat"], []));
        AddRecipe(new CraftingRecipe("Meat", ["Field", "Blacksmith", "Tree", "Sword"], ["Meat"], []));
        AddRecipe(new CraftingRecipe("Meat", ["Field", "Farmer", "Tree", "Sword"], ["Meat"], []));

        AddRecipe(new CraftingRecipe("Mine",
            ["Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone"], ["Mine"],
            ["Stone"]));

        AddRecipe(new CraftingRecipe("Hunter", ["Villager", "Sword"], ["Hunter"], ["Sword", "Villager"]));

        AddRecipe(new CraftingRecipe("Farmer", ["Villager", "Shovel"], ["Farmer"], ["Shovel", "Villager"]));

        AddRecipe(new CraftingRecipe("Blacksmith", ["Villager", "Axe"], ["Blacksmith"], ["Axe", "Villager"]));

        AddRecipe(new CraftingRecipe("Altar", ["Brick", "Brick", "Brick", "Brick", "Brick", "Glass", "Glass", "Wood"],
            ["Altar"], ["Brick", "Glass", "Wood"]));
    }

    #region Recipe data

    private readonly IList<CraftingRecipe> recipes = [];
    public IReadOnlyCollection<CraftingRecipe> Recipes => recipes.AsReadOnly();

    public void AddRecipe(CraftingRecipe recipe) {
        ArgumentNullException.ThrowIfNull(recipe, nameof(recipe));

        recipes.Add(recipe);
    }

    #endregion Recipe data
}