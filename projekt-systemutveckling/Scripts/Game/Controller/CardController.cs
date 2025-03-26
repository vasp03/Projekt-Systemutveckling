using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardController : Node2D
{
	public const string CARD_GROUP_NAME = "CARDS";
    CardCreationHelper cardCreationHelper = new CardCreationHelper();

    private CardNode selectedCard;

    private List<CardNode> hoveredCards = new List<CardNode>();

    public void CreateCard()
    {
        print("Creating card");
        // Create a new card by copying the card from Card scene and adding a instance of CardMaterial to it
        PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
        CardNode cardInstance = cardScene.Instantiate<CardNode>();

        bool ret = cardInstance.CreateNode(cardCreationHelper.GetCreatedInstanceOfCard(cardCreationHelper.GetRandomCardType()), this);
        if (ret)
        {
            cardInstance.ZIndex = CardCount;
            AddChild(cardInstance);
            cardInstance.SetPosition(new Vector2(100, 100));
        }
    }

    public int CardCount {
	    get =>

		    AllCards.Count;
    }

    private void SetTopCard(Node2D cardNode) {
	    IReadOnlyCollection<CardNode> cardNodes = AllCardsSorted;

        foreach (CardNode node in cardNodes)
        {
            if (node.ZIndex > cardNode.ZIndex)
            {
                node.ZIndex -= 1;
            }
        }

        // Set the card that is being dragged to the top
        cardNode.ZIndex = cardNodes.Count;
    }

    public IReadOnlyCollection<CardNode> AllCards => 
	    GetTree().GetNodesInGroup(CARD_GROUP_NAME).Cast<CardNode>().ToArray();
    public IReadOnlyCollection<CardNode> AllCardsSorted => 
	    AllCards.OrderBy(x=>x.ZIndex).ToArray();

    private bool CardIsTopCard(Node2D cardNode)
    {
        // Get all the card nodes
        IReadOnlyCollection<CardNode> cardNodes = AllCardsSorted;

        foreach (CardNode node in cardNodes)
        {
            if (node.ZIndex > cardNode.ZIndex)
            {
                if (hoveredCards.Contains(node))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void print(String message)
    {
        GD.Print(message);
    }

    // UUID generator
    public static String GenerateUUID()
    {
        return Guid.NewGuid().ToString();
    }

    // Get all cards


    // Move card to the mouse position
    public void MoveCardToMousePosition(CardNode cardNode)
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        cardNode.SetPosition(mousePosition);
    }

    // Get the top card at the mouse position
    public CardNode GetTopCardAtMousePosition()
    {
        CardNode topCard = null;

        foreach (CardNode card in hoveredCards)
        {
            if (topCard == null)
            {
                topCard = card;
            }
            else if (card.GetZIndex() > topCard.GetZIndex())
            {
                topCard = card;
            }
        }

        return topCard;
    }

    public void AddCardToHoveredCards(CardNode cardNode)
    {
        hoveredCards.Add(cardNode);
        CheckForHighLight();
    }

    public void RemoveCardFromHoveredCards(CardNode cardNode)
    {
        hoveredCards.Remove(cardNode);
        CheckForHighLight();
        cardNode.SetHighlighted(false);
    }

    public void CheckForHighLight()
    {
        foreach (CardNode card in hoveredCards)
        {
            if (CardIsTopCard(card))
            {
                card.SetHighlighted(true);
            }
            else
            {
                card.SetHighlighted(false);
            }
        }
    }

    public void CheckForStacking(){
        IReadOnlyCollection<CardNode> cardNodes = AllCards;

        // foreach (CardNode card in cardNodes){
        //     card.GetOverlappingCards();
        // }
    }

    public override void _Input(InputEvent @event)
    {
        // Detect mouse movement
        if (@event is InputEventMouseMotion mouseMotion)
        {
        }
        else if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.Space)
            {
                CreateCard();
            }
            else if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
            {
                // Exit the game
                GetTree().Quit();
            }
            else if (eventKey.Pressed && eventKey.Keycode == Key.A)
            {
                // Print all the cards in the scene that is in the group "Cards"
                foreach (Node node in GetTree().GetNodesInGroup("Cards"))
                {
                    CardNode cardNode = (CardNode)node;
                    print(cardNode.CardType.ID + " | Type: " + cardNode.CardType.GetType());
                }
            }

        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed)
            {
                selectedCard = GetTopCardAtMousePosition();
                if (selectedCard != null)
                {
                    SetTopCard(selectedCard);
                    selectedCard.SetIsBeingDragged(true);
                }
            }
            else
            {
                if (selectedCard != null)
                {
                    selectedCard.SetIsBeingDragged(false);
                    selectedCard = null;
                }

                CheckForStacking();
            }
        }
    }
}