using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Interface;

public class CardController {
    public const string CARD_GROUP_NAME = "CARDS";

    private readonly GameController _gameController;

    private readonly MouseController _mouseController;
    private readonly CardCreationHelper CardCreationHelper;

    private readonly CraftingController CraftingController;

    private readonly List<CardNode> hoveredCards = [];

    private CardNode selectedCard;

    // Constructor
    public CardController(GameController gameController, MouseController mouseController) {
        _gameController = gameController;
        _mouseController = mouseController;
        CardCreationHelper = new CardCreationHelper(gameController, this);
        CraftingController = new CraftingController(CardCreationHelper);

        CreateStartingRecipes();
    }

    public int CardCount => AllCards.Count;

    public IReadOnlyCollection<CardNode> AllCards =>
        _gameController.GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();

    public IReadOnlyCollection<CardNode> AllCardsSorted =>
        AllCards.OrderBy(x => x.ZIndex).ToArray();

    public CardNode CreateCard(Card card, Vector2 position = default) {
        ArgumentNullException.ThrowIfNull(card);

        CardNode cardInstance = CreateCard();

        cardInstance.CardType = card;
        cardInstance.CardController = this;

        cardInstance.Position = position;
        _gameController.AddChild(cardInstance);

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

        cardInstance.CardController = this;

        cardInstance.ZIndex = CardCount + 1;
        _gameController.AddChild(cardInstance);


        return cardInstance;
    }

    /// <summary>
    ///     Creates the starting recipes for crafting.
    /// </summary>
    public void CreateStartingRecipes() {
        CraftingController.AddRecipe(new CraftingRecipe("Jam", ["Berry", "CookingPot"], ["Jam"]));
        CraftingController.AddRecipe(new CraftingRecipe("Stick", ["Villager", "Wood"], ["Stick"]));
        CraftingController.AddRecipe(new CraftingRecipe("Fish", ["FishingPole", "Water", "Hunter"], ["Fish"]));
        CraftingController.AddRecipe(new CraftingRecipe("Axe", ["Stone", "Stick", "Stick"], ["Axe"]));
        CraftingController.AddRecipe(new CraftingRecipe("Wood", ["Tree", "Axe"], ["Wood"]));
        CraftingController.AddRecipe(new CraftingRecipe("Stone", ["Mine", "Blacksmith"], ["Stone"]));
        CraftingController.AddRecipe(new CraftingRecipe("Tent", ["Leaf", "Leaf", "Leaf", "Leaf", "Wood"], ["Tent"]));
        CraftingController.AddRecipe(new CraftingRecipe("Berry", ["Bush", "Villager"], ["Berry"]));
        CraftingController.AddRecipe(new CraftingRecipe("Leaf", ["Villager", "Stick", "Bush"], ["Leaf"]));
        CraftingController.AddRecipe(new CraftingRecipe("Leaf", ["Villager", "Stick", "Tree"], ["Leaf"]));
        CraftingController.AddRecipe(new CraftingRecipe("FishingPole", ["Stick", "Stone"], ["FishingPole"]));
        CraftingController.AddRecipe(new CraftingRecipe("CookedFish", ["Fish", "Campfire"], ["CookedFish"]));
        CraftingController.AddRecipe(new CraftingRecipe("CookedMeat", ["Meat", "Campfire"], ["CookedMeat"]));
        CraftingController.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "House"], ["Villager"]));
        CraftingController.AddRecipe(new CraftingRecipe("Villager", ["Villager", "Villager", "Tent"], ["Villager"]));
        CraftingController.AddRecipe(new CraftingRecipe("House", ["Stone", "Stone", "Stone", "Stone", "Planks", "Planks"], ["House"]));
        CraftingController.AddRecipe(new CraftingRecipe("Greenhouse", ["Brick", "Brick", "Glass", "Glass", "Glass", "Glass"], ["Greenhouse"]));
        CraftingController.AddRecipe(new CraftingRecipe("Clay", ["Sand", "Water"], ["Clay"]));
        CraftingController.AddRecipe(new CraftingRecipe("Brick", ["Clay", "Campfire"], ["Brick"]));
        CraftingController.AddRecipe(new CraftingRecipe("SwordMK1", ["Wood", "Wood", "Stone"], ["SwordMK1"]));
        CraftingController.AddRecipe(new CraftingRecipe("Planks", ["Wood", "Wood"], ["Planks"]));
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
        _mouseController.SetMouseCursor(MouseController.MouseCursor.hand_close);
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
                    if (stackable.NeighbourAbove == null) {
                        GD.Print($"Card does not have a neighbour above: {card.CardType.TextureType}");
                        card.ZIndex = NumberOfCards;
                    } else {
                        card.ZIndex = CounterForCardsAbove++;
                    }
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
        if (selectedCard != null) {
            selectedCard.SetIsBeingDragged(false);

            // Checks for a card under the moved card and sets if it exists as a neighbour below. 
            CardNode underCard = GetCardUnderMovedCard();

            if (underCard != null && !selectedCard.HasNeighbourBelow && !underCard.HasNeighbourAbove)
                selectedCard.SetOverLappedCardToStack(underCard);

            selectedCard = null;
        }
    }

    /// <summary>
    ///     Used to print the cards and their neighbours for debugging purposes.
    /// </summary>
    public void PrintCardsNeighbours() {
        // Print the all cards and their neighbours
        GD.Print("------------------");
        GD.Print("Cards and their neighbours:");
        GD.Print("------------------");
        int i = 0;

        foreach (CardNode card in AllCardsSorted)
            if (card.CardType is IStackable stackable) {
                string cardInfo =
                    $"This: {card.CardType.TextureType}:{card.ZIndex} - IsBeingDragged: {card.IsBeingDragged}";
                string aboveInfo = stackable.NeighbourAbove != null
                    ? $"Above: {stackable.NeighbourAbove.TextureType} - IsBeingDragged: {
                        ((Card)stackable.NeighbourAbove).CardNode.IsBeingDragged} "
                    : "Above: None ";
                string belowInfo = stackable.NeighbourBelow != null
                    ? $"Below: {stackable.NeighbourBelow.TextureType} - IsBeingDragged: {
                        ((Card)stackable.NeighbourBelow).CardNode.IsBeingDragged} "
                    : "Below: None ";

                GD.Print("------------------");
            }
    }

    public void CreateAllCards() {
        CreateCard("Apple");
        CreateCard("Berry");
        CreateCard("Leaves");
        CreateCard("Mine");
        CreateCard("Planks");
        CreateCard("Stick");
        CreateCard("Stone");
        CreateCard("SwordMk1");
        CreateCard("Tree");
        CreateCard("Water");
        CreateCard("Wood");
    }
}