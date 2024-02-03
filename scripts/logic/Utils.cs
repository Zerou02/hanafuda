using System;
using System.Collections.Generic;
using Godot;

public class Utils
{
    public static List<Card> removeCard(List<Card> cards, Card card)
    {
        var newList = new List<Card>();
        cards.ForEach(x =>
        {
            if (!card.equal(x))
            {
                newList.Add(x);
            }
        });
        return newList;
    }

    public static List<Card> cloneCards(List<CardScn> cardScns)
    {
        var cards = extractCards(cardScns);
        return copyCardList(cards);
    }
    public static List<Card> extractCards(List<CardScn> cardScns)
    {
        var retList = new List<Card>();
        cardScns.ForEach(x => retList.Add(x.card));
        return retList;
    }
    public static List<Card> copyCardList(List<Card> cards)
    {
        var retList = new List<Card>();
        cards.ForEach(x =>
        {
            retList.Add(x.clone());
        });
        return retList;
    }
    public static List<CardScn> removeCardScn(List<CardScn> cardScns, CardScn cardScn)
    {
        var newList = new List<CardScn>();
        cardScns.ForEach(x =>
        {
            if (!x.card.isEqual(cardScn.card))
            {
                newList.Add(x);
            }
        });
        return newList;
    }
    public static void reparentTo(Node node, Node newParent)
    {
        node.Reparent(newParent);
    }

    public static byte boolToByte(bool val)
    {
        return (byte)(val == true ? 1 : 0);
    }

    public static bool byteToBool(byte val)
    {
        return val == (byte)1 ? true : false;
    }
}