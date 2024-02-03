using System;
using System.Collections.Generic;
using System.Diagnostics;
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




    public static void handoutCardsAtStartOfGame(Lobby server, List<int> playerIDs)
    {
        foreach (var x in playerIDs)
        {
            for (int i = 0; i < 8; i++)
            {
                GD.Print(i);
                server.command(MessageType.MoveCard, Serializer.serializeCardMove(CardPosition.Deck, 0, CardPosition.Hand, x));
            }
        }
        for (int i = 0; i < 8; i++)
        {
            server.command(MessageType.MoveCard, Serializer.serializeCardMove(CardPosition.Deck, 0, CardPosition.TableCard, 255));
        }
    }

    public void switchPlayer()
    {
        activePlayerIdx = (activePlayerIdx + 1) % players.Count;
        activePlayer = players[activePlayerIdx];
    }

    public static List<Card> matchTableCards(Card card, List<Card> tableCards)
    {
        var retList = new List<Card>();
        foreach (var x in tableCards)
        {
            if (x.isValid() && card.month == x.month)
            {
                retList.Add(x);
                continue;
            }
        }
        return retList;
    }

    public static int findFirstInvalidIdx(List<Card> cards)
    {
        var idx = 255;
        for (int i = 0; i < cards.Count; i++)
        {
            if (!cards[i].isValid())
            {
                idx = i;
                break;
            }
        }
        return idx;
    }
    public static void startDeckTurn(Lobby server, Card deckCard, List<Card> tableCards)
    {
        if (deckCard == null) { return; }
        var matches = matchTableCards(deckCard, tableCards);
        GD.Print("countmatches", matches.Count);
        if (matches.Count == 0)
        {
            server.command(MessageType.MoveCard, Serializer.serializeCardMove(CardPosition.Deck, 0, CardPosition.TableCard, findFirstInvalidIdx(tableCards)));
            server.command(MessageType.SwitchPlayer, new byte[] { });
        }
        else if (matches.Count == 1)
        {
            server.command(MessageType.MatchTableCardWithDeck, Serializer.serializeCards(new List<Card>() { deckCard, matches[0] }));
            server.command(MessageType.SwitchPlayer, new byte[] { });
        }
        else
        {
            server.command(MessageType.OpenDeckCard, new byte[] { });
            server.command(MessageType.DeckChoose, new byte[] { });
        }
    }
}