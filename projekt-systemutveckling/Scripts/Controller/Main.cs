using System;
using System.Linq;
using Godot.Collections;
using Godot;

public partial class Main : Node2D
{
    private bool firstCard = true;

    private int cardId = 0;

    private static Array<Node2D> cardNodes = new Array<Node2D>();

    public void CreateNewCard(CardTypeEnum.TypeEnum type = CardTypeEnum.TypeEnum.Random)
    {
        // Duplicate the card named "CardTemplate"
        PackedScene cardScene = GD.Load<PackedScene>("res://Scenes/Cards.tscn");
        Cards cardInstance = cardScene.Instantiate<Cards>();

        cardInstance.StartCard(cardId++, type);

        // Sets the Z index of the card based on the number of cards in the scene
        cardInstance.ZIndex = GetTree().GetNodesInGroup("Card").Count + 1;

        GetParent().AddChild(cardInstance);
        cardNodes.Add(cardInstance);
    }


    // Get when a key is pressed
    public override void _Input(InputEvent @event)
    {
        // Detect mouse movement
        if (@event is InputEventMouseMotion mouseMotion)
        {
            foreach (Node2D card in cardNodes)
            {
                (card as Cards).SetHighlighted(CardIsTopCard(card) && IsMouseOverCard(card));
            }
        }
        else if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.Space)
            {
                // Call the createNew method
                CreateNewCard();
            }
            else if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
            {
                // Exit the game
                GetTree().Quit();
            }
            else if (eventKey.Pressed && eventKey.Keycode == Key.A)
            {
                // Print the position of the card name "CardTemplate"
                GetAllCards();
                foreach (Node2D card in cardNodes)
                {
                    GD.Print("ZIndex: " + card.ZIndex + " Position: " + card.Position + " Type: " + (card as Cards).GetCardTypeInformationHolder().GetCardType() + " Card Id: " + (card as Cards).GetCardId());
                }
                GD.Print("----------------------------------------");
            }
        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed)
            {
                MoveCards();
            }
            else
            {
                StopMoveCards();
            }
        }
    }

    // Get all Card nodes in the scene
    private void GetAllCards()
    {
        // Get all the nodes in the scene
        Array<Node> nodes = GetTree().GetNodesInGroup("Card");

        cardNodes.Clear();

        foreach (Node2D node in nodes)
        {
            if (node is Node2D)
            {
                cardNodes.Add(node);
            }
        }
    }

    // Make card draggable when mouse is pressed
    public void MoveCards()
    {
        // Get all the card nodes
        SortCards();

        foreach (Node2D cardNode in cardNodes)
        {
            if (IsMouseOverCard(cardNode) && CardIsTopCard(cardNode))
            {
                (cardNode as Cards).SetDragging(true);
                SetTopCard(cardNode);
                break;
            }
        }
    }

    // Check if the card is the top card with the cards that the cursor is above
    private Boolean CardIsTopCard(Node2D cardNode)
    {
        // Get all the card nodes
        SortCards();

        foreach (Node2D node in cardNodes)
        {
            if (node.ZIndex > cardNode.ZIndex)
            {
                if (IsMouseOverCard(node))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Set the card to the top of the other cards
    private void SetTopCard(Node2D cardNode)
    {
        // Move every card back one ZIndex except for the cards before the card that is being dragged
        SortCards();

        foreach (Node2D node in cardNodes)
        {
            if (node.ZIndex > cardNode.ZIndex)
            {
                node.ZIndex -= 1;
            }
        }

        // Set the card that is being dragged to the top
        cardNode.ZIndex = cardNodes.Count;
    }

    public static void SortCards()
    {
        cardNodes.OrderBy(card => card.ZIndex);
    }

    // Stop dragging cards
    public void StopMoveCards()
    {
        // Get all the card nodes
        GetAllCards();

        foreach (Node2D cardNode in cardNodes)
        {
            if (cardNode is Cards card)
            {
                card.SetDragging(false);
            }
        }
    }

    // Get if a card has mouse over it
    private Boolean IsMouseOverCard(Node2D cardNode)
    {
        Vector2 mousePosition = GetGlobalMousePosition();

        var x = (cardNode as Cards).GetSprite2D().GetTexture().GetWidth();
        var y = (cardNode as Cards).GetSprite2D().GetTexture().GetHeight();

        // Get the cards area
        Rect2 cardArea = new Rect2(cardNode.Position - (new Vector2(x / 2, y / 2)), new Vector2(x, y));

        // Check if the mouse is inside the card area
        return cardArea.HasPoint(mousePosition);
    }
}
