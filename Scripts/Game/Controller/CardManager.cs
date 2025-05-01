using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

namespace Goodot15.Scripts.Game.Controller;

public class CardManager : GameManagerBase {
    public const string CARD_GROUP_NAME = "CARDS";

    private readonly List<CardNode> hoveredCards = [];

    private CardNode selectedCard;

    public Vector2 CraftButtonOffset { get; } = new(0, -110);

    private MouseManager MouseManager => GameController.GetManager<MouseManager>();
    private CardCreationHelper CardCreationHelper => GameController.GetManager<CardCreationHelper>();

    private CraftingManager CraftingManager => GameController.GetManager<CraftingManager>();

    public int CardCount => AllCards.Count;

    public IReadOnlyCollection<CardNode> AllCards =>
        GameController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

    public IReadOnlyCollection<CardNode> AllCardsSorted =>
        AllCards.OrderBy(x => x.ZIndex).ToArray();

    private List<CardNode> Stacks =>
        AllCards.Where(x => x.HasNeighbourAbove && !x.HasNeighbourBelow && x.CardType is IStackable).ToList();

    // Constructor

    public override void OnReady() {
        CreateStartingRecipes();
    }

    public CardNode CreateCard(Card card, Vector2 position = default) {
        ArgumentNullException.ThrowIfNull(card);

        CardNode cardInstance = CreateCard();

        cardInstance.CardType = card;
        cardInstance.CardManager = this;

        cardInstance.Position = position;
        // if (cardInstance.GetParent() != null) cardInstance.GetParent().RemoveChild(cardInstance);
        // GameController.AddChild(cardInstance);

        return cardInstance;
    }

    public CardNode CreateCard(string cardType, Vector2 position = default) {
        return CreateCard(CardCreationHelper.GetCreatedInstanceOfCard(cardType), position);
    }

    /// <summary>
    ///     Creates a new card and adds it to the scene, with a random underlying CardType
    /// </summary>
    /// <returns>
    ///     The created card instance.
    /// </returns>
    public CardNode CreateCard() {
        // Create a new card by copying the card from Card scene and adding a instance of CardMaterial to it
        PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
        CardNode cardInstance = cardScene.Instantiate<CardNode>();

        cardInstance.CardManager = this;

        cardInstance.ZIndex = CardCount + 1;
        GameController.GetTree().CurrentScene.AddChild(cardInstance);

        return cardInstance;
    }

    /// <summary>
    ///     Creates the starting recipes for crafting.
    /// </summary>
    public void CreateStartingRecipes() {
        CraftingManager.AddRecipe(new CraftingRecipe("Jam",
            ["Berry", "Berry", "Berry", "Berry", "Berry", "Campfire", "CookingPot"], ["Jam"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Stick", ["Villager", "Wood", "Axe"], ["Stick"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Hunter"], ["Fish"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Axe", ["Stone", "Stick", "Stick"], ["Axe"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Villager"], ["Wood"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Hunter"], ["Wood"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Blacksmith"], ["Wood"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe", "Farmer"], ["Wood"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Stone", ["Mine", "Villager"], ["Stone"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Stone", ["Mine", "Hunter"], ["Stone"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Stone", ["Mine", "Blacksmith"], ["Stone", "Stone", "Stone"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Stone", ["Mine", "Farmer"], ["Stone"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Tent", ["Leaves", "Leaves", "Leaves", "Leaves", "Wood"],
            ["Tent"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Berry", ["Bush", "Villager"], ["Berry"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Berry", ["Bush", "Hunter"], ["Berry"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Berry", ["Bush", "Blacksmith"], ["Berry"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Berry", ["Bush", "Farmer"], ["Berry", "Berry"]));


        CraftingManager.AddRecipe(new CraftingRecipe("Leaves", ["Villager", "Tree"], ["Leaves", "Leaves", "Apple"]));

        CraftingManager.AddRecipe(new CraftingRecipe("FishingPole", ["Stick", "Stone"], ["FishingPole"]));

        CraftingManager.AddRecipe(new CraftingRecipe("CookedFish", ["Fish", "Campfire"], ["CookedFish"]));

        CraftingManager.AddRecipe(new CraftingRecipe("CookedMeat", ["Meat", "Campfire"], ["CookedMeat"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "House"],
            ["Villager", "Villager", "Villager"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "Tent"],
            ["Villager", "Villager", "Villager"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Hunter", "House"],
            ["Villager", "Hunter", "Villager"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Hunter", "Tent"],
            ["Villager", "Hunter", "Villager"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Blacksmith", "House"],
            ["Villager", "Blacksmith", "Villager"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Blacksmith", "Tent"],
            ["Villager", "Blacksmith", "Villager"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Farmer", "House"],
            ["Villager", "Farmer", "Villager"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Farmer", "Tent"],
            ["Villager", "Farmer", "Villager"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Hunter", "House"],
            ["Hunter", "Hunter", "Hunter"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Hunter", "Tent"],
            ["Hunter", "Hunter", "Hunter"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Blacksmith", "House"],
            ["Hunter", "Blacksmith", "Hunter"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Blacksmith", "Tent"],
            ["Hunter", "Blacksmith", "Hunter"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Farmer", "House"],
            ["Hunter", "Farmer", "Hunter"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Hunter", ["Hunter", "Farmer", "Tent"],
            ["Hunter", "Farmer", "Hunter"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Blacksmith", "House"],
            ["Blacksmith", "Blacksmith", "Blacksmith"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Blacksmith", "Tent"],
            ["Blacksmith", "Blacksmith", "Blacksmith"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Farmer", "House"],
            ["Blacksmith", "Farmer", "Blacksmith"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Blacksmith", ["Blacksmith", "Farmer", "Tent"],
            ["Blacksmith", "Farmer", "Blacksmith"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Farmer", ["Farmer", "Farmer", "House"],
            ["Farmer", "Farmer", "Farmer"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Farmer", ["Farmer", "Farmer", "Tent"],
            ["Farmer", "Farmer", "Farmer"]));

        CraftingManager.AddRecipe(new CraftingRecipe("House",
            ["Stone", "Stone", "Stone", "Stone", "Planks", "Planks", "Brick", "Brick", "Brick", "Brick"], ["House"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Greenhouse",
            ["Brick", "Brick", "Glass", "Glass", "Glass", "Glass"], ["Greenhouse"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Clay", ["Sand", "Water"], ["Clay"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Brick", ["Clay", "Campfire"], ["Brick"]));

        CraftingManager.AddRecipe(new CraftingRecipe("SwordMK1", ["Wood", "Wood", "Stone"], ["SwordMK1"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Planks", ["Wood", "Wood"], ["Planks"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Sand", ["Stone", "Villager"], ["Sand"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Sand", ["Stone", "Hunter"], ["Sand"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Sand", ["Stone", "Blacksmith"], ["Sand", "Sand", "Sand"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Sand", ["Stone", "Farmer"], ["Sand"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Water", ["Water", "Water"], ["Water", "Water", "Water"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Glass", ["Sand", "Campfire"], ["Glass"]));

        CraftingManager.AddRecipe(
            new CraftingRecipe("FishingPole", ["Wood", "Wood", "FishingPole"], ["FishingPole"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Shovel", ["Stick", "Stick", "Stone", "Stone"], ["Shovel"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Axe", ["Stick", "Stick", "Stone", "Stone", "Stone"], ["Axe"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Field",
            ["Sand", "Sand", "Sand", "Sand", "Stone", "Stone", "Water"], ["Field"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Campfire",
            ["Wood", "Wood", "Wood", "Sticks", "Sticks", "Leaves"], ["Campfire"]));

        CraftingManager.AddRecipe(new CraftingRecipe("CookingPot", ["Clay", "Clay", "Stick"], ["CookingPot"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Bush",
            ["Leaves", "Leaves", "Leaves", "Leaves", "Leaves", "Leaves"], ["Bush"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Meat", ["Field", "Villager", "Tree", "Sword"], ["Meat"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Meat", ["Field", "Hunter", "Tree", "Sword"],
            ["Meat", "Meat", "Meat"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Meat", ["Field", "Blacksmith", "Tree", "Sword"], ["Meat"]));
        CraftingManager.AddRecipe(new CraftingRecipe("Meat", ["Field", "Farmer", "Tree", "Sword"], ["Meat"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Mine",
            ["Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone", "Stone"], ["Mine"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Hunter", ["Villager", "Sword"], ["Hunter"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Farmer", ["Villager", "Shovel"], ["Farmer"]));

        CraftingManager.AddRecipe(new CraftingRecipe("Blacksmith", ["Villager", "Axe"], ["Blacksmith"]));
    }

    /// <summary>
    ///     Checks if the card is the top card on the scene.
    /// </summary>
    private bool CardIsTopCard(Node2D cardNode) {
        foreach (CardNode node in hoveredCards)
            if (node.ZIndex > cardNode.ZIndex)
                return false;
        return true;
    }

    /// <summary>
    ///     Generates a new UUID (Universally Unique Identifier) string.
    /// </summary>
    public static string GenerateUUID() {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    ///     Adds the card to the hovered cards list and sets its highlighted state to true.
    /// </summary>
    public void AddCardToHoveredCards(CardNode cardNode) {
        hoveredCards.Add(cardNode);
        CheckForHighLight();
    }

    /// <summary>
    ///     Removes the card from the hovered cards list and sets its highlighted state to false.
    /// </summary>
    public void RemoveCardFromHoveredCards(CardNode cardNode) {
        hoveredCards.Remove(cardNode);
        CheckForHighLight();
        cardNode.SetHighlighted(false);
    }

    /// <summary>
    ///     Checks if the card is the top card on the scene which the mouse is hovering over and sets the highlighted state.
    /// </summary>
    public void CheckForHighLight() {
        foreach (CardNode card in hoveredCards)
            if (CardIsTopCard(card))
                card.SetHighlighted(true);
            else
                card.SetHighlighted(false);
    }

    /// <summary>
    ///     Gets the top card at the mouse position.
    /// </summary>
    /// <returns>
    ///     The top card at the mouse position or null if no card is hovered.
    /// </returns>
    public CardNode GetTopCardAtMousePosition() {
        CardNode topCard = null;

        foreach (CardNode card in hoveredCards)
            if (topCard == null)
                topCard = card;
            else if (card.GetZIndex() > topCard.GetZIndex()) topCard = card;

        return topCard;
    }

    /// <summary>
    ///     Gets the card under the moved card.
    /// </summary>
    /// <returns>
    ///     The card under the moved card or null if no card is found.
    /// </returns>
    private CardNode GetCardUnderMovedCard() {
        IReadOnlyCollection<CardNode> hoveredCardsSorted = selectedCard.HoveredCardsSorted;

        CardNode topUnderCard = null;

        foreach (CardNode card in hoveredCardsSorted)
            if (card.ZIndex < selectedCard.ZIndex && (topUnderCard == null || card.ZIndex > topUnderCard.ZIndex))
                topUnderCard = card;

        return topUnderCard;
    }

    /// <summary>
    ///     Called when the left mouse button is pressed.
    /// </summary>
    public void LeftMouseButtonPressed() {
        MouseManager.SetMouseCursor(MouseManager.MouseCursor.HAND_CLOSE);
        selectedCard = GetTopCardAtMousePosition();
        // SetTopZIndexForCard(selectedCard);

        if (selectedCard != null) SetZIndexForAllCards(selectedCard);

        if (selectedCard != null) {
            selectedCard.SetIsBeingDragged(true);

            if (selectedCard.HasNeighbourAbove)
                selectedCard.IsMovingOtherCards = true;
            else
                // Set the card that is being dragged to the top
                selectedCard.ZIndex = CardCount + 1;

            // Set the neighbour below to null if the card is moved to make the moved card able to get new neighbours
            // And sets the card below if it exists to not have a neighbour above
            if (selectedCard.HasNeighbourBelow)
                if (selectedCard.CardType is IStackable stackable) {
                    if (stackable.NeighbourBelow != null) stackable.NeighbourBelow.NeighbourAbove = null;
                    stackable.NeighbourBelow = null;
                }
        }
    }

    /// <summary>
    ///     Sets the ZIndex of all cards based on the selected card.
    /// </summary>
    /// <param name="cardNode">The card node to set the ZIndex from and its neighbours above.</param>
    public void SetZIndexForAllCards(CardNode cardNode) {
        int NumberOfCards = AllCards.Count;
        List<IStackable> stackAboveSelectedCard =
            cardNode.CardType is IStackable stackableCard ? stackableCard.StackAbove : null;
        int NumberOfCardsAbove = stackAboveSelectedCard != null ? stackAboveSelectedCard.Count : 0;
        int CounterForCardsAbove = NumberOfCards - NumberOfCardsAbove;
        int CounterForCardsBelow = 1;

        foreach (CardNode card in AllCardsSorted)
            if (card == cardNode) {
                if (card.CardType is IStackable stackable) {
                    if (stackable.NeighbourAbove == null)
                        card.ZIndex = NumberOfCards;
                    else
                        card.ZIndex = CounterForCardsAbove++;
                } else {
                    card.ZIndex = NumberOfCards;
                }
            } else if (stackAboveSelectedCard != null && stackAboveSelectedCard.Contains(card.CardType as IStackable)) {
                card.ZIndex = CounterForCardsAbove++;
            } else {
                card.ZIndex = CounterForCardsBelow++;
            }
    }

    /// <summary>
    ///     Called when the left mouse button is released.
    /// </summary>
    public void LeftMouseButtonReleased() {
        MouseManager.SetMouseCursor(MouseManager.MouseCursor.POINT_SMALL);
        if (selectedCard != null) {
            selectedCard.SetIsBeingDragged(false);

            // Checks for a card under the moved card and sets if it exists as a neighbour below. 
            CardNode underCard = GetCardUnderMovedCard();

            if (underCard != null && !selectedCard.HasNeighbourBelow && !underCard.HasNeighbourAbove)
                selectedCard.SetOverLappedCardToStack(underCard);

            selectedCard = null;
        }

        // Checks if a card is supposed to have a craft button above it
        foreach (CardNode card in AllCards)
            if (Stacks.Contains(card) && card.CardType is IStackable stackable &&
                CraftingManager.CheckForCraftingWithStackable(stackable.StackAboveWithItself) != null) {
                AddCraftButton(card);
            } else {
                if (card.CraftButton != null) {
                    card.CraftButton.QueueFree();
                    card.CraftButton = null;
                }
            }
    }

    /// <summary>
    ///     Adds a craft button to the specified card node.
    /// </summary>
    /// <param name="cardNode">The card node to add the craft button to.</param>
    private void AddCraftButton(CardNode cardNode) {
        if (cardNode.CraftButton != null) return;

        PackedScene craftButtonScene = GD.Load<PackedScene>("res://Scenes/CraftButton.tscn");
        CraftButton craftButtonInstance = craftButtonScene.Instantiate<CraftButton>();

        craftButtonInstance.Position = cardNode.Position + CraftButtonOffset;

        cardNode.CraftButton = craftButtonInstance;

        craftButtonInstance.CardNode = cardNode;

        craftButtonInstance.CardManager = this;

        GameController.AddChild(craftButtonInstance);
    }

    /// <summary>
    ///     Crafts a card from the specified card node.
    /// </summary>
    /// <param name="cardNode">The card node to craft from.</param>
    public void CraftCardFromSpecifiedCardNode(CardNode cardNode) {
        if (cardNode == null) return;

        if (cardNode.CraftButton != null) {
            cardNode.CraftButton.QueueFree();
            cardNode.CraftButton = null;
        }

        if (!(cardNode.CardType is IStackable stackable)) return;

        // Check for the recipe
        List<string> recipe = CraftingManager.CheckForCraftingWithStackable(stackable.StackAboveWithItself);
        if (recipe == null) {
            GD.Print("No recipe found for the selected card.");
            return;
        }

        Vector2 spawnPos = cardNode.Position;

        // Remove the cards in the stack part of cardNode
        foreach (IStackable stackableCard in stackable.StackAboveWithItself)
            if (stackableCard is Card card)
                card.CardNode.QueueFree();

        foreach (string cardName in recipe) {
            CardNode card = CreateCard(cardName, cardNode.Position);
            card.ZIndex = cardNode.ZIndex + 1;
            spawnPos += new Vector2(0, -15);
        }
    }
}