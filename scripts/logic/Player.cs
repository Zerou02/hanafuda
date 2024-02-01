using System.Collections.Generic;

public class Player
{
    public List<Card> handCards = new List<Card>();
    public List<Card> openCards = new List<Card>();

    public GameManager manager { get; set; }
    public int id;
    public Player(GameManager manager, int id)
    {
        this.manager = manager;
        this.id = id;
    }

    public void addHandCard(Card card)
    {
        this.handCards.Add(card);
    }

    public void addOpenCard(Card card)
    {
        this.openCards.Add(card);
    }
}