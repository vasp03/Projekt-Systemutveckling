using System.Collections.Generic;
using System.Linq;
using Godot;

public class CraftingController {
    private readonly CardCreationHelper CardCreationHelper;

    public CraftingController(CardCreationHelper cardCreationHelper) {
        CardCreationHelper = cardCreationHelper;
    }

    public List<CraftingRecipe> Recipes { get; private set; }

    public void AddRecipe(CraftingRecipe recipe) {
        if (recipe == null) {
            GD.Print("Recipe is null");
            return;
        }
        
        if (Recipes == null) {
            Recipes = new List<CraftingRecipe>();
        }

        Recipes.Add(recipe);
    }

    public List<string> CheckForCrafting(List<Card> Cards) {
        List<StringIntHolder> CardForCraftingAmount = [];

        foreach (Card card in Cards) {
            StringIntHolder cardForCrafting =
                CardForCraftingAmount.FirstOrDefault(x => x.StringValue == card.TextureType);
            if (cardForCrafting != null)
                cardForCrafting.IntValue++;
            else
                CardForCraftingAmount.Add(new StringIntHolder(card.TextureType, 1));
        }

        GD.Print("Checkpoint 1");

        // Sort the list by the name of the card
        CardForCraftingAmount.Sort((x, y) => x.StringValue.CompareTo(y.StringValue));

        GD.Print("Number of recipes: " + Recipes.Count);

        foreach (CraftingRecipe recipe in Recipes) {
            GD.Print("Recipe: " + recipe.Name);

            List<StringIntHolder> CardsInRecipeAndAmount = [];

            foreach (string cardName in recipe.CardsForCrafting) {
                StringIntHolder cardInRecipie = CardsInRecipeAndAmount.FirstOrDefault(x => x.StringValue == cardName);
                if (cardInRecipie != null)
                    cardInRecipie.IntValue++;
                else
                    CardsInRecipeAndAmount.Add(new StringIntHolder(cardName, 1));
            }

            // Sort the list by the name of the card
            CardsInRecipeAndAmount.Sort((x, y) => x.StringValue.CompareTo(y.StringValue));

            // Check if the recipe matches the cards in the stack
            bool recipeMatches = true;

            if (CardsInRecipeAndAmount.Count != CardForCraftingAmount.Count) {
                continue;
            }

            for (int i = 0; i < CardsInRecipeAndAmount.Count; i++)
                if (CardsInRecipeAndAmount[i].StringValue != CardForCraftingAmount[i].StringValue ||
                    CardsInRecipeAndAmount[i].IntValue != CardForCraftingAmount[i].IntValue) {
                    recipeMatches = false;
                    break;
                }

            GD.Print("Checkpoint 4");

            if (recipeMatches) {
                GD.Print("Recipe found for crafting.");
                List<string> craftedCards = recipe.CardsForCraftingResult;
                return craftedCards;
            }

            GD.Print("Checkpoint 5");
        }

        GD.Print("No recipe found for crafting.");
        return null;
    }
}