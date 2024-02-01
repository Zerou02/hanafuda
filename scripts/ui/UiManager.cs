using Godot;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Reflection;
using System.Threading;


public enum UiModes { PlayerTurn, DeckTurn };
public partial class UiManager : Node2D
{
    public GameManager gameManager;
    DeckScn deck;
    TableCardsN tableCards;
    PlayerScn[] players = new PlayerScn[2];
    PlayerScn activePlayer;
    public int ownID = -1;
    public UiModes uiMode = UiModes.PlayerTurn;
    Label label;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        deck = GetNode<DeckScn>("DeckScn");
        tableCards = GetNode<TableCardsN>("TableCardsN");
        players[0] = GetNode<PlayerScn>("Player");
        players[1] = GetNode<PlayerScn>("Player2");
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
    /* public void handleHanafudaTableCardPressed(CardScn cardScn)
    {
        if (uiMode == UiModes.DeckTurn)
        {
            if (cardScn.card.month == deck.deck.openCard.month)
            {
                gameManager.handleDeckToTablePlay(cardScn.card);
            }
        }
        else
        {
            // var selected = activePlayer.getSelectedCard();
            if (selected == null || cardScn.card.month != selected.card.month) { return; }
            var arg = selected.card.clone();
            activePlayer.unselectSelectedCard();
            gameManager.playToTableCard(arg, cardScn.card);
        }
    } */

    //public void removeTableCard(Card card)
    // {
    //tableCards.removeCard(card);
    //}

    /* public void handleEmptyCardPressed(int idx)
    {
        if (uiMode == UiModes.DeckTurn) { return; }
        var selected = activePlayer.getSelectedCard();
        if (selected == null) { return; }
        gameManager.playCardToEmpty(selected.card);
        tableCards.addCardAt(idx, selected.card);
        //   tableCards.unHighlightCards();
    } */
    public override void _Process(double delta)
    {
        label.Text = ownID.ToString();
    }

    public void initTableCards(List<Card> cards)
    {
        // tableCards.initCards(cards);
    }

    public void initPlayers(Player player1, Player player2)
    {
        this.players[0].setPlayer(player1);
        this.players[1].setPlayer(player2);
    }

    public void setActivePlayer()
    {
        var activeIdx = gameManager.activePlayerIdx;
        foreach (var x in players)
        {
            x.setActive(false);
            x.unselectSelectedCard();
        }
        activePlayer = players[activeIdx];
        activePlayer.setActive(true);
    }

    public void highlightValidHandCards()
    {
        /* var cardsTable = tableCards.cards;
        // var cardsHand = activePlayer.handCards.cardScns;
        var highlightList = new List<Card>();
        foreach (var x in cardsHand)
        {
            foreach (var y in cardsTable)
            {
                if (y != null && x.card.month == y.month)
                {
                    highlightList.Add(x.card);
                    continue;
                }
            }
        } */
        //activePlayer.unHighlightHandCards();
        //activePlayer.highlightHandCards(highlightList);
    }

    /// <summary>
    /// Basierend auf aktuell ausgewählter in Hand
    /// </summary>
    /* public void highlightTableCards()
    {
        CardScn selected = null;
        if (uiMode == UiModes.PlayerTurn)
        {
            //    selected = activePlayer.getSelectedCard();
        }
        else if (uiMode == UiModes.DeckTurn)
        {
            //selected = deck.openCard;
        }
        //tableCards.unHighlightCards();
        if (selected == null) { return; }
        var cardsTable = tableCards.cards;
        var highlightList = new List<Card>();
        foreach (var x in cardsTable)
        {
            if (x == null) { continue; }
            if (selected.card.month == x.month)
            {
                highlightList.Add(x);
                continue;
            }
        }
        tableCards.highlightCards(highlightList);
    } */

    /* public void highlightTableCards(List<Card> cards)
    {
        tableCards.highlightCards(cards);
    } */

    public void moveDeckToPlayerHand(int playerId)
    {
        players[playerId].addHandCard(deck.draw());
    }

    public void moveDeckToTable()
    {
        tableCards.addCard(deck.draw());
    }
    public void setDeck(Deck deck)
    {
        this.deck.init(deck);
    }
    /* public void update()
    {
        foreach (var x in players)
        {
            x.update();
        }
        tableCards.updateCards();
        highlightHandCards();
        highlightTableCards();
    } */

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
                    continue;
                }
            }
        }
        return retList;
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
        //      deck.setOpenCardVisibility(false);
        foreach (var x in players)
        {
            x.setActive(false);
        }

        activePlayer = players[playerId];
        activePlayer.setActive(true);
        var cards = getValidHandCards();
        activePlayer.highlightHandCards(cards);
    }

    public void init(Deck deck)
    {
        this.deck.deck = deck;
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
        activePlayer.removeHandCard(handCard);
        handCard.setSelected(false);
        handCard.setAllowInteraction(false);
        tableCards.addCard(handCard);
        gameManager.playCardToEmpty(handCard.card.clone());
    }

    public void matchTableCard(CardScn handCard, CardScn tableCard)
    {
        handCard.setSelected(false);
        activePlayer.addOpenCard(tableCard);
        activePlayer.addOpenCard(handCard);
        tableCards.removeCard(tableCard);
        activePlayer.removeHandCard(handCard);
        gameManager.playToTableCard(handCard.card.clone(), tableCard.card.clone());
    }

    public void selectHandCard(CardScn cardScn, int playerId)
    {
        if (ownID != activePlayer.player.id) { return; }
        activePlayer.setSelectedCard(cardScn);
    }
    /* public void addTableCardToFirst(Card card)
    {
        tableCards.addCardFirst(card);
    } */
}