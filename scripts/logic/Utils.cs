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
        var retVal = val == (byte)1 ? true : false;
        GD.Print(retVal);
        return retVal;
    }

    public static bool cardListsSame(List<Card> list1, List<Card> list2)
    {
        if (list1.Count != list2.Count) { return false; }
        var retVal = true;
        for (int i = 0; i < list1.Count; i++)
        {
            if (!list1[i].isEqual(list2[i])) { retVal = false; break; }
        }
        return retVal;
    }
    public static void printDictList<T, K>(Dictionary<T, List<K>> dict) where T : notnull
    {
        foreach (var x in dict)
        {
            GD.Print(x.Key);
            foreach (var y in x.Value)
            {
                GD.Print(y);
            }
            GD.Print();
        }
    }

    public static List<CardScn> clearCardsScns(List<CardScn> cardScns)
    {
        foreach (var x in cardScns)
        {
            x.QueueFree();
        }
        return new List<CardScn>();
    }
}