using Godot;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;

public enum UiModes { PlayerTurn, DeckTurn };
public partial class UiManager : Node2D
{
    public DeckScn deck;
    public TableCards tableCards;
    public PlayerScn[] players = new PlayerScn[2];
    public PlayerScn activePlayer;
    public int ownID = -1;
    public int activePlayerId = 0;
    public UiModes uiMode = UiModes.PlayerTurn;
    Label label;

    PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
    InputManager inputManager;
    public Lobby server;
    public Label activeTurnDisplay;
    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        deck = GetNode<DeckScn>("DeckScn");
        tableCards = GetNode<TableCards>("TableCards");
        activeTurnDisplay = GetNode<Label>("ActiveTurnDisplay");
        players[0] = GetNode<PlayerScn>("Player");
        players[1] = GetNode<PlayerScn>("Player2");
        inputManager = GetNode<InputManager>(Constants.inputManagerPath);
        inputManager.uiManager = this;
        //        tableCards.emptyCardPressed += (idx) => handleEmptyCardPressed(idx);
        // tableCards.flowerCardPressed += (cardScn) => handleHanafudaTableCardPressed(cardScn);
        foreach (var x in players)
        {
            x.cardSelected += (x) => handleCardSelected(x);
        }
    }

    public void handleCardSelected(CardScn cardScn)
    {
        if (uiMode == UiModes.DeckTurn) { return; }
        //        highlightTableCards();
    }
    public override void _Process(double delta)
    {
        label.Text = ownID.ToString();
        activeTurnDisplay.Text = ownID == activePlayerId ? "Your Turn" : "Enemy Turn";
    }

    public void setActivePlayer(int activeId)
    {
        foreach (var x in players)
        {
            x.setActive(false);
            x.unselectSelectedCard();
        }
        activePlayerId = activeId;
        activePlayer = players[activeId];
        activePlayer.setActive(true);
        highlightHandCards();
    }

    public void moveDeckToPlayerHand(int playerId)
    {
        var card = deck.draw();
        card.type = CardType.Hand;
        card.isOpen = true;
        card.setAllowInteraction(playerId == ownID && activePlayerId == ownID);
        players[playerId].addHandCard(card);
    }

    public void moveDeckToTable()
    {
        var card = deck.draw();
        card.type = CardType.Table;
        card.isOpen = true;
        tableCards.addCard(card);
        highlightHandCards();

    }

    public void setDeck(List<Card> cards)
    {
        foreach (var x in cards)
        {
            var scn = cardScn.Instantiate<CardScn>();
            scn.type = CardType.Deck;
            deck.addCard(scn);
            scn.setCard(x.clone());
        }
    }
    List<Card> getValidHandCards()
    {
        var retList = new List<Card>();
        foreach (var x in activePlayer.handCards.cardScns)
        {
            foreach (var y in tableCards.cardScns)
            {
                if (y.card.isValid() && y.card.month == x.card.month)
                {
                    retList.Add(x.card);
                    break;
                }
            }
        }
        return retList;
    }

    public void unHighlightTableCards()
    {
        tableCards.highlightCards(new List<Card>());
    }
    public void highlightTableCards(CardScn cardScn)
    {
        var highlightList = new List<Card>();
        foreach (var x in tableCards.cardScns)
        {
            if (!x.card.isValid()) { continue; }
            if (cardScn.card.month == x.card.month)
            {
                highlightList.Add(x.card);
                continue;
            }
        }
        tableCards.highlightCards(highlightList);
    }
    public void startRound(int playerId)
    {
        foreach (var x in players)
        {
            x.setActive(false);
        }
        activePlayer = players[playerId];
        activePlayer.setActive(true);
        activePlayer.handCards.cardScns.ForEach(x =>
        {
            x.setAllowInteraction(true);
        });
    }

    void highlightHandCards()
    {
        if (ownID != activePlayerId) { return; }
        var cards = getValidHandCards();
        activePlayer.highlightHandCards(cards);
    }

    public void setUiMode(UiModes uiMode)
    {
        this.uiMode = uiMode;
        if (uiMode == UiModes.DeckTurn)
        {
            activePlayer.setActive(false);
        }
        else
        {

        }
    }
    public void setOpenCardVis(bool val)
    {
        deck.setOpenCardVisibility(val);
    }

    public void addToEmptyTableCard(CardScn handCard, int idx)
    {
        handCard.type = CardType.Table;
        activePlayer.removeHandCard(handCard);
        handCard.setSelected(false);
        handCard.setAllowInteraction(false);
        tableCards.addCard(handCard);
    }

    public CardScn findCard(List<CardScn> cardScns, Card toFind)
    {
        CardScn retScn = null;
        foreach (var x in cardScns)
        {
            if (x.card.isEqual(toFind))
            {
                retScn = x;
                break;
            }
        }
        return retScn;
    }

    public void matchTableCardDeck(Card tableCard)
    {
        var deckScn = deck.draw();
        deckScn.isOpen = true;
        var tableCardScn = findCard(tableCards.cardScns, tableCard);
        deckScn.type = CardType.Open;
        tableCardScn.type = CardType.Open;
        deckScn.setSelected(false);
        activePlayer.addOpenCard(tableCardScn);
        activePlayer.addOpenCard(deckScn);
        tableCards.removeCard(tableCardScn);
    }

    public void matchTableCard(Card handCard, Card tableCard)
    {
        var handCardScn = findCard(activePlayer.handCards.cardScns, handCard);
        var tableCardScn = findCard(tableCards.cardScns, tableCard);
        handCardScn.type = CardType.Open;
        tableCardScn.type = CardType.Open;
        handCardScn.setSelected(false);
        activePlayer.addOpenCard(tableCardScn);
        activePlayer.addOpenCard(handCardScn);
        tableCards.removeCard(tableCardScn);
        activePlayer.removeHandCard(handCardScn);
    }

    public void selectHandCard(CardScn cardScn)
    {
        activePlayer.setSelectedCard(cardScn);
    }


    public void matchEmptyCard(Card card, int idx)
    {
        var cardScn = findCard(activePlayer.handCards.cardScns, card);
        activePlayer.removeHandCard(cardScn);
        cardScn.type = CardType.Table;
        cardScn.setAllowInteraction(false);
        tableCards.overwriteEmptyCard(cardScn, idx);
    }

    public bool belongsToActivePlayer(CardScn card)
    {
        var retVal = false;
        foreach (var x in activePlayer.handCards.cardScns)
        {
            if (x.card.isEqual(card.card))
            {
                retVal = true;
            }
        }
        return retVal;
    }

    public void handleDeckChoose()
    {
        setOpenCardVis(true);
        if (activePlayerId != ownID) { return; }
        highlightTableCards(deck.cards[0]);
        uiMode = UiModes.DeckTurn;
    }
}