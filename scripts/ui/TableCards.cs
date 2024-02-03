using Godot;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

public partial class TableCards : Node2D
{
	public List<Card> cards = new List<Card>();
	public List<CardScn> cardScns = new List<CardScn>();
	public PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
	public PackedScene emptyCardScn = GD.Load<PackedScene>("scenes/EmptyCard.tscn");
	public List<Card> highlightedCards = new List<Card>();

	[Signal]
	public delegate void emptyCardPressedEventHandler(int idx);
	[Signal]
	public delegate void flowerCardPressedEventHandler(CardScn card);

	const int rows = 2;
	public override void _Ready()
	{

	}

	public void initCards(List<Card> pCards)
	{
		foreach (var x in pCards)
		{
			cards.Add(x.clone());
		}
		if (cards.Count % 2 == 0)
		{
			cards.Insert(cards.Count / 2, null);
		}
		cards.Add(null);
		updateCards();
	}

	public void addCardAt(int idx, Card card)
	{
		this.cards[idx] = card.clone();
		if (!cards.Contains(null))
		{
			cards.Insert(cards.Count / 2, null);
			cards.Add(null);
		}
		updateCards();
	}

	public void updateCards()
	{
		cardScns = new List<CardScn>();
		MinosUtils.removeChildren(this);
		var idx = 0;
		var row = 0;
		var elementsPerRow = this.cards.Count / 2;
		var ix = 0;
		foreach (var cardsds in this.cards)
		{
			if (elementsPerRow == ix)
			{
				ix = 0;
				row += 1;
			}
			Card? entry = cardsds;
			Node2D card;
			if (entry == null)
			{
				var x = emptyCardScn.Instantiate<EmptyCard>();
				this.AddChild(x);
				var b = idx;
				x.pressed += () => EmitSignal(SignalName.emptyCardPressed, b);
				card = x;
			}
			else
			{
				var x = cardScn.Instantiate<CardScn>();
				this.AddChild(x);
				x.setCard(entry);
				x.setAllowSelectable(false);
				foreach (var c in highlightedCards)
				{
					if (c.equal(x.card))
					{
						x.setValid(true);
					}
				}
				//x.pressed += () => EmitSignal(SignalName.flowerCardPressed, x);
				cardScns.Add(x);
				card = x;
			}
			card.Position = new Vector2(ix * Constants.cardWidth, row * Constants.cardHeight);
			ix += 1;
			idx += 1;
		}
	}

	public void removeCard(Card card)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			if (cards[i] != null && card.equal(cards[i]))
			{
				cards[i] = null;
			}
		}
		updateCards();
	}

	public void highlightCards(List<Card> cards)
	{
		highlightedCards = new List<Card>();
		foreach (var x in cardScns)
		{
			x.setValid(false);
			foreach (var y in cards)
			{
				if (x.card.equal(y))
				{
					highlightedCards.Add(x.card);
					x.setValid(true);
				}
			}
		}
	}



	public void unHighlightCards()
	{
		highlightCards(new List<Card>());
	}


}
