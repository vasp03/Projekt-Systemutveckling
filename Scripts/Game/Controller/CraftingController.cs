using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Goodot15.Scripts.Game.Controller;

public class CraftingController : GameManagerBase {
    public CraftingController(GameController gameController) : base(gameController) {
    }

    private CardCreationHelper CardCreationHelper => CoreGameController.GetManager<CardCreationHelper>();

    public List<CraftingRecipe> Recipes { get; } = [];

    public void AddRecipe(CraftingRecipe recipe) {
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

        // Sort the list by the name of the card
        CardForCraftingAmount.Sort((x, y) => x.StringValue.CompareTo(y.StringValue));


        foreach (CraftingRecipe recipe in Recipes) {
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
            for (int i = 0; i < CardsInRecipeAndAmount.Count; i++)
                if (CardsInRecipeAndAmount[i].StringValue != CardForCraftingAmount[i].StringValue ||
                    CardsInRecipeAndAmount[i].IntValue != CardForCraftingAmount[i].IntValue) {
                    recipeMatches = false;
                    break;
                }

            if (recipeMatches) {
                List<string> craftedCards = recipe.CardsForCraftingResult;
                return craftedCards;
            }
        }

        GD.Print("No recipe found for crafting.");
        return null;
    }
}