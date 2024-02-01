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
            if (!x.card.equal(cardScn.card))
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

}