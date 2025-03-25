using Godot;
using System;

public partial class CardController : Node2D
{
    CardCreationHelper cardCreationHelper = new CardCreationHelper();

    public void CreateCard()
    {
        print("Creating card");
        // Create a new card by copying the card from Card scene and adding a instance of CardMaterial to it
        PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Card.tscn");
        CardNode cardInstance = cardScene.Instantiate<CardNode>();

        bool ret = cardInstance.CreateNode(cardCreationHelper.GetCreatedInstanceOfCard(cardCreationHelper.GetRandomCardType()));
        if (ret)
        {
            AddChild(cardInstance);
            cardInstance.SetPosition(new Vector2(100, 100));
        }
    }

    public void SetHighlighted(bool isHighlighted)
    {

    }

    public void SetNeighbourAbove(Card card)
    {

    }

    public void SetNeighbourBelow(Card card)
    {

    }

    public void CanCardsStack(Card card1, Card card2)
    {

    }

    public void RemoveCard()
    {

    }

    public void SetCardMoving(bool isMoving)
    {

    }

    public void SetCardPosition(Card card, Vector2 position)
    {

    }

    public void print(String message)
    {
        GD.Print(message);
    }

    // UUID generator
    public String GenerateUUID()
    {
        return Guid.NewGuid().ToString();
    }

    // 

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
                    print(cardNode.GetCard().GetName() + " | Type: " + cardNode.GetCard().GetType());
                }
            }
            else if (eventKey.Pressed && eventKey.Keycode == Key.S)
            {
                // Print the number of children in the scene
            }
                
        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed)
            {
            }
            else
            {
            }
        }
    }
}