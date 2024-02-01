using System;
using System.Collections.Generic;
using Godot;

public class Deck
{
    public List<Card> cards = new List<Card>();
    public Card? openCard = null;

    public Deck()
    {
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                this.cards.Add(new Card(i, j));
            }
        }
        shuffle();
    }

    public void shuffle()
    {
        var random = new Random();
        for (int i = 0; i < cards.Count - 2; i++)
        {
            var j = random.Next(i, cards.Count);
            var h = cards[i];
            cards[i] = cards[j];
            cards[j] = h;
        }
    }

    public Card? draw()
    {
        if (this.cards.Count == 0)
        {
            return null;
        }
        var card = this.cards[0];
        openCard = card.clone();
        this.cards.RemoveAt(0);
        return openCard;
    }
}