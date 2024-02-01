using System;
using System.Collections.Generic;
using Godot;

public class GameManager
{
    public List<Player> players = new List<Player>();
    int currentPlayer = 0;
    public Deck deck = new Deck();
    public List<Card> tableCards = new List<Card>();
    public int activePlayerIdx = 0;
    public Player activePlayer;

    public GameManager(int amountPlayer)
    {
        for (int i = 0; i < amountPlayer; i++)
        {
            this.players.Add(new Player(this, i));
        }

    }

    public void startGame()
    {
        foreach (var x in players)
        {
            for (int i = 0; i < 8; i++)
            {
                x.addHandCard(deck.draw()!);
            }
        }
        for (int i = 0; i < 8; i++)
        {
            tableCards.Add(deck.draw()!);
        }
        startRound(0);
        /*uiManager.initTableCards(tableCards);
        this.activePlayer = players[activePlayerIdx];
        uiManager.setActivePlayer();
        uiManager.update(); */
    }

    public void startRound(int playerID)
    {
        activePlayer = players[playerID];
    }

    public void switchPlayer()
    {
        activePlayerIdx = (activePlayerIdx + 1) % players.Count;
        activePlayer = players[activePlayerIdx];
        uiManager.setActivePlayer();
    }
    public void playCardToEmpty(Card selectedCard)
    {
        activePlayer.handCards.Remove(selectedCard);
        tableCards.Add(selectedCard);
        //  uiManager.update();
        handleDeckMove();
    }

    public void handleTableCardMatch(Card selectedCard, Card tableCard)
    {
        tableCards = Utils.removeCard(tableCards, tableCard);
        activePlayer.addOpenCard(selectedCard.clone());
        activePlayer.addOpenCard(tableCard.clone());
        activePlayer.handCards = Utils.removeCard(activePlayer.handCards, selectedCard);
        //uiManager.removeTableCard(tableCard);
    }
    public void playToTableCard(Card selectedCard, Card tableCard)
    {
        if (selectedCard.month != tableCard.month) { return; }
        handleTableCardMatch(selectedCard, tableCard);
        handleDeckMove();
    }

    public List<Card> matchTableCards(Card card)
    {
        var retList = new List<Card>();
        foreach (var x in tableCards)
        {
            if (card.month == x.month)
            {
                retList.Add(x);
                continue;
            }
        }
        return retList;
    }
    public void handleDeckMove()
    {
        var deckCard = deck.draw();
        if (deckCard == null) { return; }
        uiManager.setOpenCardVis(true);
        var matches = matchTableCards(deckCard!);
        if (matches.Count == 0)
        {
            tableCards.Add(deckCard);
            //    uiManager.addTableCardToFirst(deckCard.clone());
            uiManager.setOpenCardVis(false);
            switchPlayer();
        }
        else if (matches.Count == 1)
        {
            handleTableCardMatch(deckCard, matches[0]);
            uiManager.setOpenCardVis(false);
            switchPlayer();
        }
        else
        {
            //  uiManager.highlightTableCards(matches);
            uiManager.setUiMode(UiModes.DeckTurn);
        }
        //     uiManager.update();
    }

    public void handleDeckToTablePlay(Card playerCard)
    {
        activePlayer.addOpenCard(playerCard.clone());
        activePlayer.addOpenCard(deck.openCard.clone());
        tableCards = Utils.removeCard(tableCards, playerCard);
        // uiManager.removeTableCard(playerCard);
        deck.openCard = null;
        uiManager.setOpenCardVis(false);
        uiManager.setUiMode(UiModes.PlayerTurn);
        switchPlayer();
        //       uiManager.update();
    }
}