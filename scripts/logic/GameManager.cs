using System;
using System.Collections.Generic;
using Godot;

public class GameManager
{
    public List<Player> players = new List<Player>();
    int currentPlayer = 0;
    public Deck deck = new Deck();
    public Lobby server;
    public List<Card> tableCards = new List<Card>();
    public int activePlayerIdx = 0;
    public Player activePlayer;

    public GameManager(int amountPlayer, Lobby server)
    {
        for (int i = 0; i < amountPlayer; i++)
        {
            this.players.Add(new Player(this, i));
        }
        this.server = server;
    }




    public void startGame()
    {
        activePlayerIdx = 0;
        activePlayer = players[activePlayerIdx];
        server.command(MessageType.InitDeck, Serializer.serializeCards(deck.cards));
        foreach (var x in players)
        {
            for (int i = 0; i < 8; i++)
            {
                x.addHandCard(deck.draw()!);
                server.command(MessageType.MoveCard, Serializer.serializeCardMove(CardPosition.Deck, 0, CardPosition.Hand, x.id));
            }
        }
        for (int i = 0; i < 8; i++)
        {
            tableCards.Add(deck.draw()!);
            server.command(MessageType.MoveCard, Serializer.serializeCardMove(CardPosition.Deck, 0, CardPosition.TableCard, 255));
        }
        startRound(0);
    }

    public void startRound(int playerID)
    {
        activePlayer = players[playerID];
    }

    public void switchPlayer()
    {
        activePlayerIdx = (activePlayerIdx + 1) % players.Count;
        activePlayer = players[activePlayerIdx];
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
        var matches = matchTableCards(deckCard!);
        if (matches.Count == 0)
        {
            tableCards.Add(deckCard);
            switchPlayer();
        }
        else if (matches.Count == 1)
        {
            handleTableCardMatch(deckCard, matches[0]);
            switchPlayer();
        }
        else
        {
            //  uiManager.highlightTableCards(matches);
            // uiManager.setUiMode(UiModes.DeckTurn);
        }
        //     uiManager.update();
    }

    public void startDeckTurn()
    {
        GD.Print("STARTDECKTURN");
        GD.Print(tableCards.Count);
        tableCards.ForEach(x => x.print());
        GD.Print(activePlayer.openCards.Count);
        GD.Print("hand");
        activePlayer.handCards.ForEach(x => x.print());


    }

    public void moveDeckToPlayerHand()
    {
        var card = this.deck.draw();
        if (card == null) { return; }
        activePlayer.handCards.Add(card);
    }
    public void moveDeckToTable()
    {
        var card = this.deck.draw();
        if (card == null) { return; }
        tableCards.Add(card);
    }
    public void handleDeckToTablePlay(Card playerCard)
    {
        activePlayer.addOpenCard(playerCard.clone());
        activePlayer.addOpenCard(deck.openCard.clone());
        tableCards = Utils.removeCard(tableCards, playerCard);
        // uiManager.removeTableCard(playerCard);
        deck.openCard = null;
        //uiManager.setOpenCardVis(false);
        //uiManager.setUiMode(UiModes.PlayerTurn);
        switchPlayer();
        //       uiManager.update();
    }
}