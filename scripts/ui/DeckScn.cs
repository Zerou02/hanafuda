using System.Collections.Generic;
using Godot;
public partial class DeckScn : Node2D
{
	public Deck deck;
	public CardScn upperCard;
	public List<CardScn> cards = new List<CardScn>();
	PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
	public override void _Ready()
	{
		upperCard = GetNode<CardScn>("UpperCard");
		upperCard.setAllowHover(false);
		upperCard.setAllowSelectable(false);
		upperCard.Visible = false;
	}

	public override void _Process(double delta)
	{
	}

	public void init(Deck deck)
	{
		this.deck = deck;
		deck.cards.ForEach(x =>
		{
			var y = cardScn.Instantiate<CardScn>();
			this.cards.Add(y);
			AddChild(y);
			y.setCard(x);
		});
	}

	public void setUpperCard()
	{
		var openCard = deck.openCard;

		if (openCard == null) { return; }
		foreach (var x in this.cards)
		{
			if (x.card.equal(openCard))
			{
				upperCard = x;
				break;
			}
		}
	}
	public CardScn draw()
	{
		setUpperCard();
		return upperCard;
		//this.openCard.Visible = val;
		//if (openCard == null) { return; }
		//this.openCard.setCard(openCard);
	}
	public void setOpenCardVisibility(bool val)
	{
		upperCard.isOpen = val;
	}
}
