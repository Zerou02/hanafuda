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