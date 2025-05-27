using System;
using System.Collections.Generic;
using System.Linq;
using Goodot15.Scripts.Game.Model;
using Goodot15.Scripts.Game.Model.Div;
using Goodot15.Scripts.Game.Model.Parents;

namespace Goodot15.Scripts.Game.Controller;

public class CraftingController {
    /// <summary>
    ///     Constructs a new Crafting Controller instance, automatically registers default recipes
    /// </summary>
    public CraftingController() {
        RegisterDefaultRecipes();
    }

    /// <summary>
    ///     Check if the cards in the stack can be crafted into a new card
    ///     Returns a list of the cards that can be crafted
    ///     If no cards can be crafted, returns null
    /// </summary>
    /// <param name="Cards">List of cards to check</param>
    public Pair<IReadOnlyCollection<string>, IReadOnlyCollection<string>> CheckForCraftingWithStackable(
        IReadOnlyList<Card> Cards) {
        Pair<string, int>[] CardForCraftingAmount =
            CollectTotalCardTypesInStack(Cards).OrderByDescending(e => e.Left).ToArray();

        // foreach (Card card in Cards) {
        //     // Left (L) being card type ID; Right (R) being count of cards to crafted
        //     Pair<string, int> cardForCrafting =
        //         CardForCraftingAmount.FirstOrDefault(x => x.Left == card.CardName);
        //     if (cardForCrafting is not null)
        //         cardForCrafting.Right++;
        //     else
        //         CardForCraftingAmount.Add(new Pair<string, int>(card.CardName, 1));
        // }

        // Sort the list by the name of the card
        // CardForCraftingAmount.Sort((x, y) => x.Left.CompareTo(y.Left));

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

            if (CardsInRecipeAndAmount.Count != CardForCraftingAmount.Length) continue;

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
    ///     Collects and counts each type of card in a given stack
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static IReadOnlyCollection<Pair<string, int>> 
        CollectTotalCardTypesInStack(IReadOnlyCollection<Card> cards) {
        IList<Pair<string, int>> CardForCraftingAmount = [];
        foreach (Card card in cards) {
            // Left (L) being card type ID; Right (R) being count of cards to crafted
            Pair<string, int> cardForCrafting =
                CardForCraftingAmount.FirstOrDefault(x => x.Left == card.CardName);
            if (cardForCrafting is not null)
                cardForCrafting.Right++;
            else
                CardForCraftingAmount.Add(new Pair<string, int>(card.CardName, 1));
        }

        return CardForCraftingAmount.ToArray();
    }

    #region Recipe registration

    /// <summary>
    ///     Creates the starting recipes for crafting.
    /// </summary>
    private void RegisterDefaultRecipes() {
        RegisterDefaultFoodRecipes();

        RegisterDefaultMaterialRecipes();

        RegisterDefaultVillagerRecipes();

        RegisterDefaultToolRecipes();

        RegisterDefaultBuildingRecipes();
    }

    /// <summary>
    ///     Registers default tool recipes
    /// </summary>
    private void RegisterDefaultToolRecipes() {
        RegisterRecipe(new CraftingRecipe("Axe", ["Stone", "Stick", "Stick"], ["Axe"], ["Stone", "Stick"]));

        RegisterRecipe(new CraftingRecipe("FishingPole", ["Stick", "Stone"], ["FishingPole"], ["Stick", "Stone"]));

        RegisterRecipe(new CraftingRecipe("SwordMK1", ["Wood", "Wood", "Stone"], ["SwordMK1"], ["Wood", "Stone"]));

        RegisterRecipe(new CraftingRecipe("Shovel", ["Stick", "Stick", "Stone", "Stone"], ["Shovel"],
            ["Stick", "Stone"]));
    }

    /// <summary>
    ///     REgister default building recipes
    /// </summary>
    private void RegisterDefaultBuildingRecipes() {
        RegisterRecipe(new CraftingRecipe("Campfire", ["Wood", "Wood", "Wood", "Stick", "Stick", "Leaves"],
            ["Campfire"],
            ["Wood", "Stick", "Leaves"]));

        RegisterRecipe(new CraftingRecipe("Cookingpot", ["Clay", "Clay", "Stick"], ["CookingPot"], ["Clay", "Stick"]));

        RegisterRecipe(new CraftingRecipe("Tent", ["Leaves", "Leaves", "Leaves", "Leaves", "Wood"], ["Tent"],
            ["Leaves", "Wood"]));

        RegisterRecipe(new CraftingRecipe("House",
            ["Stone", "Stone", "Stone", "Stone", "Planks", "Planks", "Brick", "Brick", "Brick", "Brick"], ["House"],
            ["Stone", "Planks", "Brick"]));

        RegisterRecipe(new CraftingRecipe("Greenhouse", ["Brick", "Brick", "Glass", "Glass", "Glass", "Glass"],
            ["Greenhouse"], ["Brick", "Glass"]));

        RegisterRecipe(new CraftingRecipe("Field", ["Sand", "Sand", "Sand", "Sand", "Stone", "Stone", "Water"],
            ["Field"],
            ["Sand", "Stone", "Water"]));


        RegisterRecipe(new CraftingRecipe("Bush", ["Leaves", "Leaves", "Leaves", "Leaves", "Leaves", "Leaves"],
            ["Bush"],
            ["Leaves"]));


        RegisterRecipe(new CraftingRecipe("Mine",
            ["Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone"], ["Mine"],
            ["Stone"]));
    }

    /// <summary>
    ///     Registers default villager recipes
    /// </summary>
    private void RegisterDefaultVillagerRecipes() {
        RegisterRecipe(new CraftingRecipe("Hunter", ["Villager", "Sword"], ["Hunter"], ["Sword", "Villager"]));

        RegisterRecipe(new CraftingRecipe("Farmer", ["Villager", "Shovel"], ["Farmer"], ["Shovel", "Villager"]));

        RegisterRecipe(new CraftingRecipe("Blacksmith", ["Villager", "Axe"], ["Blacksmith"], ["Axe", "Villager"]));

        RegisterRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Villager", ["Villager", "Hunter", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Villager", ["Villager", "Hunter", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Villager", ["Villager", "Blacksmith", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Villager", ["Villager", "Blacksmith", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Villager", ["Villager", "Farmer", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Villager", ["Villager", "Farmer", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Hunter", ["Hunter", "Hunter", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Hunter", ["Hunter", "Hunter", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Hunter", ["Hunter", "Blacksmith", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Hunter", ["Hunter", "Blacksmith", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Hunter", ["Hunter", "Farmer", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Hunter", ["Hunter", "Farmer", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Blacksmith", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Blacksmith", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Farmer", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Farmer", "Tent"], ["Villager"], []));

        RegisterRecipe(new CraftingRecipe("Farmer", ["Farmer", "Farmer", "House"], ["Villager"], []));
        RegisterRecipe(new CraftingRecipe("Farmer", ["Farmer", "Farmer", "Tent"], ["Villager"], []));
    }

    /// <summary>
    ///     Register default food recipes
    /// </summary>
    private void RegisterDefaultFoodRecipes() {
        RegisterRecipe(new CraftingRecipe("Jam",
            ["Berry", "Berry", "Berry", "Berry", "Berry", "Campfire", "Cookingpot"],
            ["Jam"], ["Berry"]));

        RegisterRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Hunter"], ["Fish"], []));
        RegisterRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Blacksmith"], ["Fish"], []));
        RegisterRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Farmer"], ["Fish"], []));
        RegisterRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Villager"], ["Fish"], []));

        RegisterRecipe(new CraftingRecipe("Berry", ["Bush", "Villager"], ["Berry", "Berry", "Berry"], ["Bush"]));
        RegisterRecipe(new CraftingRecipe("Berry", ["Bush", "Hunter"], ["Berry", "Berry", "Berry"], ["Bush"]));
        RegisterRecipe(new CraftingRecipe("Berry", ["Bush", "Blacksmith"], ["Berry", "Berry", "Berry"], ["Bush"]));
        RegisterRecipe(new CraftingRecipe("Berry", ["Bush", "Farmer"], ["Berry", "Berry", "Berry", "Berry", "Berry"],
            ["Bush"]));

        RegisterRecipe(new CraftingRecipe("CookedFish", ["Fish", "Campfire"], ["CookedFish"], ["Fish"]));

        RegisterRecipe(new CraftingRecipe("CookedMeat", ["Meat", "Campfire"], ["CookedMeat"], ["Meat"]));

        RegisterRecipe(new CraftingRecipe("Meat", ["Field", "Villager", "Tree", "Sword"], ["Meat"], []));
        RegisterRecipe(new CraftingRecipe("Meat", ["Field", "Hunter", "Tree", "Sword"], ["Meat", "Meat", "Meat"], []));
        RegisterRecipe(new CraftingRecipe("Meat", ["Field", "Blacksmith", "Tree", "Sword"], ["Meat"], []));
        RegisterRecipe(new CraftingRecipe("Meat", ["Field", "Farmer", "Tree", "Sword"], ["Meat"], []));
    }

    /// <summary>
    ///     Registers default material recipes
    /// </summary>
    private void RegisterDefaultMaterialRecipes() {
        RegisterRecipe(new CraftingRecipe("Leaves", ["Villager", "Tree"], ["Leaves", "Leaves", "Apple"], ["Tree"]));
        RegisterRecipe(new CraftingRecipe("Leaves", ["Hunter", "Tree"], ["Leaves", "Leaves", "Apple"], ["Tree"]));
        RegisterRecipe(new CraftingRecipe("Leaves", ["Blacksmith", "Tree"], ["Leaves", "Leaves", "Apple"], ["Tree"]));
        RegisterRecipe(new CraftingRecipe("Leaves", ["Farmer", "Tree"], ["Leaves", "Leaves", "Apple"], ["Tree"]));

        RegisterRecipe(new CraftingRecipe("Sand", ["Stone", "Villager"], ["Sand"], ["Stone"]));
        RegisterRecipe(new CraftingRecipe("Sand", ["Stone", "Hunter"], ["Sand"], ["Stone"]));
        RegisterRecipe(new CraftingRecipe("Sand", ["Stone", "Blacksmith"], ["Sand", "Sand", "Sand"], ["Stone"]));
        RegisterRecipe(new CraftingRecipe("Sand", ["Stone", "Farmer"], ["Sand"], ["Stone"]));

        RegisterRecipe(new CraftingRecipe("Stick", ["Villager", "Wood", "Axe"], ["Stick"], ["Wood"]));
        RegisterRecipe(new CraftingRecipe("Stick", ["Hunter", "Wood", "Axe"], ["Stick"], ["Wood"]));
        RegisterRecipe(new CraftingRecipe("Stick", ["Blacksmith", "Wood", "Axe"], ["Stick"], ["Wood"]));
        RegisterRecipe(new CraftingRecipe("Stick", ["Farmer", "Wood", "Axe"], ["Stick"], ["Wood"]));

        RegisterRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Villager"], ["Wood", "Wood", "Wood"], ["Tree"]));
        RegisterRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Hunter"], ["Wood", "Wood", "Wood"], ["Tree"]));
        RegisterRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Blacksmith"], ["Wood", "Wood", "Wood"], ["Tree"]));
        RegisterRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Farmer"], ["Wood", "Wood", "Wood", "Wood", "Wood"],
            ["Tree"]));

        RegisterRecipe(new CraftingRecipe("Stone", ["Mine", "Villager"], ["Stone"], []));
        RegisterRecipe(new CraftingRecipe("Stone", ["Mine", "Hunter"], ["Stone"], []));
        RegisterRecipe(new CraftingRecipe("Stone", ["Mine", "Blacksmith"], ["Stone", "Stone", "Stone"], []));
        RegisterRecipe(new CraftingRecipe("Stone", ["Mine", "Farmer"], ["Stone"], []));
        RegisterRecipe(new CraftingRecipe("Stone", ["Meteorite", "Villager", "Axe"],
            ["Stone", "Stone", "Stone", "Stone", "Stone"], ["Meteorite"]));
        RegisterRecipe(new CraftingRecipe("Stone", ["Meteorite", "Hunter", "Axe"],
            ["Stone", "Stone", "Stone", "Stone", "Stone"], ["Meteorite"]));
        RegisterRecipe(new CraftingRecipe("Stone", ["Meteorite", "Blacksmith", "Axe"],
            ["Stone", "Stone", "Stone", "Stone", "Stone"], ["Meteorite"]));
        RegisterRecipe(new CraftingRecipe("Stone", ["Meteorite", "Farmer", "Axe"],
            ["Stone", "Stone", "Stone", "Stone", "Stone"], ["Meteorite"]));

        RegisterRecipe(new CraftingRecipe("Water", ["Water", "Water"], ["Water", "Water", "Water"], ["Water"]));

        RegisterRecipe(new CraftingRecipe("Glass", ["Sand", "Campfire"], ["Glass"], ["Sand"]));

        RegisterRecipe(new CraftingRecipe("Planks", ["Wood", "Wood"], ["Planks"], ["Wood"]));

        RegisterRecipe(new CraftingRecipe("Clay", ["Sand", "Water"], ["Clay"], ["Sand", "Water"]));

        RegisterRecipe(new CraftingRecipe("Brick", ["Clay", "Campfire"], ["Brick"], ["Clay"]));
    }

    #endregion Recipe registration

    #region Recipe data

    private readonly IList<CraftingRecipe> recipes = [];

    /// <summary>
    ///     Gets the collection of currently registered recipes
    /// </summary>
    public IReadOnlyCollection<CraftingRecipe> RegisteredRecipes => recipes.AsReadOnly();

    /// <summary>
    ///     Registers a new recipe
    /// </summary>
    /// <param name="recipe">Recipe data to be registered</param>
    public void RegisterRecipe(CraftingRecipe recipe) {
        ArgumentNullException.ThrowIfNull(recipe, nameof(recipe));

        recipes.Add(recipe);
    }

    #endregion Recipe data
}