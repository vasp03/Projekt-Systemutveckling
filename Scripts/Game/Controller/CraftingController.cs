using System;
using System.Collections.Generic;
using System.Linq;
using Goodot15.Scripts.Game.Model.Div;

namespace Goodot15.Scripts.Game.Controller;

public class CraftingController {
    public CraftingController(CardCreationHelper cardCreationHelper) {
        CardCreationHelper = cardCreationHelper;

        CreateStartingRecipes();
    }

    public CardCreationHelper CardCreationHelper { get; private set; }

    /// <summary>
    ///     Check if the cards in the stack can be crafted into a new card
    ///     Returns a list of the cards that can be crafted
    ///     If no cards can be crafted, returns null
    /// </summary>
    /// <param name="Cards">List of cards to check</param>
    public StringAndBoolRet CheckForCraftingWithStackable(IReadOnlyList<Card> Cards) {
        List<StringIntHolder> CardForCraftingAmount = [];

        foreach (Card card in Cards) {
            StringIntHolder cardForCrafting =
                CardForCraftingAmount.FirstOrDefault(x => x.StringValue == card.TextureType);
            if (cardForCrafting is not null)
                cardForCrafting.IntValue++;
            else
                CardForCraftingAmount.Add(new StringIntHolder(card.TextureType, 1));
        }

        // Sort the list by the name of the card
        CardForCraftingAmount.Sort((x, y) => x.StringValue.CompareTo(y.StringValue));

        foreach (CraftingRecipe recipe in recipes) {
            List<StringIntHolder> CardsInRecipeAndAmount = [];

            foreach (string cardName in recipe.CardsForCrafting) {
                StringIntHolder cardInRecipe = CardsInRecipeAndAmount.FirstOrDefault(x => x.StringValue == cardName);
                if (cardInRecipe is not null)
                    cardInRecipe.IntValue++;
                else
                    CardsInRecipeAndAmount.Add(new StringIntHolder(cardName, 1));
            }

            // Sort the list by the name of the card
            CardsInRecipeAndAmount.Sort((x, y) => x.StringValue.CompareTo(y.StringValue));

            // Check if the recipe matches the cards in the stack
            bool recipeMatches = true;

            if (CardsInRecipeAndAmount.Count != CardForCraftingAmount.Count) continue;

            for (int i = 0; i < CardsInRecipeAndAmount.Count; i++)
                if (CardsInRecipeAndAmount[i].StringValue != CardForCraftingAmount[i].StringValue ||
                    CardsInRecipeAndAmount[i].IntValue != CardForCraftingAmount[i].IntValue) {
                    recipeMatches = false;
                    break;
                }

            if (recipeMatches) {
                List<string> craftedCards = recipe.CardsForCraftingResult;
                return new StringAndBoolRet(craftedCards, recipe.ConsumeTool);
            }
        }

        return null;
    }

    /// <summary>
    ///     Creates the starting recipes for crafting.
    /// </summary>
    public void CreateStartingRecipes() {
        AddRecipe(new CraftingRecipe("Jam", ["Berry", "Berry", "Berry", "Berry", "Berry", "Campfire", "CookingPot"],
            ["Jam"]));

        AddRecipe(new CraftingRecipe("Stick", ["Villager", "Wood", "Axe"], ["Stick"]));

        AddRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Hunter"], ["Fish"]));

        AddRecipe(new CraftingRecipe("Axe", ["Stone", "Stick", "Stick"], ["Axe"]));

        AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Villager"], ["Wood"]));
        AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Hunter"], ["Wood"]));
        AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Blacksmith"], ["Wood"]));
        AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Farmer"], ["Wood"]));

        AddRecipe(new CraftingRecipe("Stone", ["Mine", "Villager"], ["Stone"]));
        AddRecipe(new CraftingRecipe("Stone", ["Mine", "Hunter"], ["Stone"]));
        AddRecipe(new CraftingRecipe("Stone", ["Mine", "Blacksmith"], ["Stone", "Stone", "Stone"]));
        AddRecipe(new CraftingRecipe("Stone", ["Mine", "Farmer"], ["Stone"]));

        AddRecipe(new CraftingRecipe("Tent", ["Leaves", "Leaves", "Leaves", "Leaves", "Wood"], ["Tent"]));

        AddRecipe(new CraftingRecipe("Berry", ["Bush", "Villager"], ["Berry"]));
        AddRecipe(new CraftingRecipe("Berry", ["Bush", "Hunter"], ["Berry"]));
        AddRecipe(new CraftingRecipe("Berry", ["Bush", "Blacksmith"], ["Berry"]));
        AddRecipe(new CraftingRecipe("Berry", ["Bush", "Farmer"], ["Berry", "Berry"]));

        AddRecipe(new CraftingRecipe("Leaves", ["Villager", "Tree"], ["Leaves", "Leaves", "Apple"]));

        AddRecipe(new CraftingRecipe("FishingPole", ["Stick", "Stone"], ["FishingPole"]));

        AddRecipe(new CraftingRecipe("CookedFish", ["Fish", "Campfire"], ["CookedFish"]));

        AddRecipe(new CraftingRecipe("CookedMeat", ["Meat", "Campfire"], ["CookedMeat"]));

        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "House"],
            ["Villager", "Villager", "Villager"]));
        AddRecipe(
            new CraftingRecipe("Villager", ["Villager", "Villager", "Tent"], ["Villager", "Villager", "Villager"]));

        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Hunter", "House"], ["Villager", "Hunter", "Villager"]));
        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Hunter", "Tent"], ["Villager", "Hunter", "Villager"]));

        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Blacksmith", "House"],
            ["Villager", "Blacksmith", "Villager"]));
        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Blacksmith", "Tent"],
            ["Villager", "Blacksmith", "Villager"]));

        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Farmer", "House"], ["Villager", "Farmer", "Villager"]));
        AddRecipe(new CraftingRecipe("Villager", ["Villager", "Farmer", "Tent"], ["Villager", "Farmer", "Villager"]));

        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Hunter", "House"], ["Hunter", "Hunter", "Hunter"]));
        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Hunter", "Tent"], ["Hunter", "Hunter", "Hunter"]));

        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Blacksmith", "House"], ["Hunter", "Blacksmith", "Hunter"]));
        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Blacksmith", "Tent"], ["Hunter", "Blacksmith", "Hunter"]));

        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Farmer", "House"], ["Hunter", "Farmer", "Hunter"]));
        AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Farmer", "Tent"], ["Hunter", "Farmer", "Hunter"]));

        AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Blacksmith", "House"],
            ["Blacksmith", "Blacksmith", "Blacksmith"]));
        AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Blacksmith", "Tent"],
            ["Blacksmith", "Blacksmith", "Blacksmith"]));

        AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Farmer", "House"],
            ["Blacksmith", "Farmer", "Blacksmith"]));
        AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Farmer", "Tent"],
            ["Blacksmith", "Farmer", "Blacksmith"]));

        AddRecipe(new CraftingRecipe("Farmer", ["Farmer", "Farmer", "House"], ["Farmer", "Farmer", "Farmer"]));
        AddRecipe(new CraftingRecipe("Farmer", ["Farmer", "Farmer", "Tent"], ["Farmer", "Farmer", "Farmer"]));

        AddRecipe(new CraftingRecipe("House",
            ["Stone", "Stone", "Stone", "Stone", "Planks", "Planks", "Brick", "Brick", "Brick", "Brick"], ["House"]));

        AddRecipe(new CraftingRecipe("Greenhouse", ["Brick", "Brick", "Glass", "Glass", "Glass", "Glass"],
            ["Greenhouse"]));

        AddRecipe(new CraftingRecipe("Clay", ["Sand", "Water"], ["Clay"]));

        AddRecipe(new CraftingRecipe("Brick", ["Clay", "Campfire"], ["Brick"]));

        AddRecipe(new CraftingRecipe("SwordMK1", ["Wood", "Wood", "Stone"], ["SwordMK1"]));

        AddRecipe(new CraftingRecipe("Planks", ["Wood", "Wood"], ["Planks"]));

        AddRecipe(new CraftingRecipe("Sand", ["Stone", "Villager"], ["Sand"]));
        AddRecipe(new CraftingRecipe("Sand", ["Stone", "Hunter"], ["Sand"]));
        AddRecipe(new CraftingRecipe("Sand", ["Stone", "Blacksmith"], ["Sand", "Sand", "Sand"]));
        AddRecipe(new CraftingRecipe("Sand", ["Stone", "Farmer"], ["Sand"]));

        AddRecipe(new CraftingRecipe("Water", ["Water", "Water"], ["Water", "Water", "Water"]));

        AddRecipe(new CraftingRecipe("Glass", ["Sand", "Campfire"], ["Glass"]));

        AddRecipe(new CraftingRecipe("FishingPole", ["Wood", "Wood", "FishingPole"], ["FishingPole"]));

        AddRecipe(new CraftingRecipe("Shovel", ["Stick", "Stick", "Stone", "Stone"], ["Shovel"]));

        AddRecipe(new CraftingRecipe("Axe", ["Stick", "Stick", "Stone", "Stone", "Stone"], ["Axe"]));

        AddRecipe(new CraftingRecipe("Field", ["Sand", "Sand", "Sand", "Sand", "Stone", "Stone", "Water"], ["Field"]));

        AddRecipe(new CraftingRecipe("Campfire", ["Wood", "Wood", "Wood", "Sticks", "Sticks", "Leaves"], ["Campfire"]));

        AddRecipe(new CraftingRecipe("CookingPot", ["Clay", "Clay", "Stick"], ["CookingPot"]));

        AddRecipe(new CraftingRecipe("Bush", ["Leaves", "Leaves", "Leaves", "Leaves", "Leaves", "Leaves"], ["Bush"]));

        AddRecipe(new CraftingRecipe("Meat", ["Field", "Villager", "Tree", "Sword"], ["Meat"]));
        AddRecipe(new CraftingRecipe("Meat", ["Field", "Hunter", "Tree", "Sword"], ["Meat", "Meat", "Meat"]));
        AddRecipe(new CraftingRecipe("Meat", ["Field", "Blacksmith", "Tree", "Sword"], ["Meat"]));
        AddRecipe(new CraftingRecipe("Meat", ["Field", "Farmer", "Tree", "Sword"], ["Meat"]));

        AddRecipe(new CraftingRecipe("Mine",
            ["Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone"], ["Mine"]));

        AddRecipe(new CraftingRecipe("Hunter", ["Villager", "Sword"], ["Hunter"], true));

        AddRecipe(new CraftingRecipe("Farmer", ["Villager", "Shovel"], ["Farmer"], true));

        AddRecipe(new CraftingRecipe("Blacksmith", ["Villager", "Axe"], ["Blacksmith"], true));
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