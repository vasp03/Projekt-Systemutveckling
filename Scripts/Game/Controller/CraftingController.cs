using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class CraftingController : GameManagerBase {
    public CraftingController() {
    }

    private CardCreationHelper CardCreationHelper => GameController.GetManager<CardCreationHelper>();

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

    /// <summary>
    ///    Check if the cards in the stack can be crafted into a new card
    /// </summary>
    /// <returns> List of the cards that will be crafted from the recipie</returns>
    /// <param name="Cards">List of cards to check</param>
    public List<string> CheckForCrafting(List<Card> Cards) {
        List<StringIntHolder> CardForCraftingAmount = [];

        foreach (Card card in Cards) {
            StringIntHolder cardForCrafting =
                CardForCraftingAmount.FirstOrDefault(x => x.StringValue == card.TextureType);
            if (cardForCrafting != null) {
                cardForCrafting.IntValue++;
            } else {
                CardForCraftingAmount.Add(new StringIntHolder(card.TextureType, 1));
            }
        }

        // Sort the list by the name of the card
        CardForCraftingAmount.Sort((x, y) => x.StringValue.CompareTo(y.StringValue));

        foreach (CraftingRecipe recipe in Recipes) {
            List<StringIntHolder> CardsInRecipeAndAmount = [];

            foreach (string cardName in recipe.CardsForCrafting) {
                StringIntHolder cardInRecipie = CardsInRecipeAndAmount.FirstOrDefault(x => x.StringValue == cardName);
                if (cardInRecipie != null) {
                    cardInRecipie.IntValue++;
                } else {
                    CardsInRecipeAndAmount.Add(new StringIntHolder(cardName, 1));
                }
            }

            // Sort the list by the name of the card
            CardsInRecipeAndAmount.Sort((x, y) => x.StringValue.CompareTo(y.StringValue));

            // Check if the recipe matches the cards in the stack
            bool recipeMatches = true;

            if (CardsInRecipeAndAmount.Count != CardForCraftingAmount.Count) {
                continue;
            }

            for (int i = 0; i < CardsInRecipeAndAmount.Count; i++) {
                if (CardsInRecipeAndAmount[i].StringValue != CardForCraftingAmount[i].StringValue ||
                    CardsInRecipeAndAmount[i].IntValue != CardForCraftingAmount[i].IntValue) {
                    recipeMatches = false;
                    break;
                }
            }


            if (recipeMatches) {
                List<string> craftedCards = recipe.CardsForCraftingResult;
                return craftedCards;
            }
        }

        return new List<string>();
    }

    /// <summary>
    ///     Check if the cards in the stack can be crafted into a new card
    ///     Returns a list of the cards that can be crafted
    ///     If no cards can be crafted, returns null
    /// </summary>
    /// <param name="Cards">List of cards to check</param>
    /// 
    public List<string> CheckForCraftingWithStackable(List<IStackable> Cards) {
        List<StringIntHolder> CardForCraftingAmount = [];

        foreach (IStackable card in Cards) {
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

            if (CardsInRecipeAndAmount.Count != CardForCraftingAmount.Count) {
                continue;
            }

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

        return null;
    }
}