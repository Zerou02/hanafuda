using Godot;
using System.Collections.Generic;

public partial class TableCardsN : Node2D
{
	public List<CardScn> cardScns = new List<CardScn>();
	int rows = 2;

	PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
	InputManager inputManager;

	public override void _Ready()
	{
		inputManager = GetNode<InputManager>(Constants.inputManagerPath);
	}
	public override void _Process(double delta)
	{
	}

	void renderCards()
	{
		var cardsPerRow = cardScns.Count / this.rows;
		for (int i = 0; i < this.rows; i++)
		{
			var row = new List<CardScn>();
			for (int j = 0; j < cardsPerRow; j++)
			{
				row.Add(cardScns[cardsPerRow * i + j]);
			}
			Flexbox.alignLeft(new Rect2(0, i * 100, 800, 100), row);
		}
	}

	void insertEmptyCardAt(int idx)
	{
		var emptyCard = this.cardScn.Instantiate<CardScn>();
		this.AddChild(emptyCard);
		emptyCard.setAllowInteraction(false);

		if (idx == cardScns.Count)
		{
			cardScns.Add(emptyCard);
		}
		else
		{
			this.cardScns.Insert(idx, emptyCard);
		}
		emptyCard.setCard(Card.GetEmpty());
		var cIdx = idx;
		emptyCard.pressed += (x) => inputManager.emptyTableCardPressed(cIdx);
	}

	void addEmptyCardAt(int idx)
	{
		var emptyCard = this.cardScn.Instantiate<CardScn>();
		this.AddChild(emptyCard);
		emptyCard.setAllowInteraction(false);

		if (idx == cardScns.Count)
		{
			cardScns.Add(emptyCard);
		}
		else
		{
			this.cardScns[idx] = emptyCard;
		}
		emptyCard.setCard(Card.GetEmpty());
		var cIdx = idx;
		emptyCard.pressed += (x) => inputManager.emptyTableCardPressed(cIdx);
	}

	void pushEmptyCard()
	{
		addEmptyCardAt(cardScns.Count);
	}
	public void addCard(CardScn cardScn)
	{
		if (cardScns.Count == 0)
		{
			cardScns.Add(cardScn);
		}
		else
		{
			var found = false;
			for (int i = 0; i < cardScns.Count; i++)
			{
				if (!cardScns[i].card.isValid())
				{
					cardScns[i].QueueFree();
					cardScns[i] = cardScn;
					found = true;
					break;
				}
			}
			if (!found)
			{
				cardScns.Add(cardScn);
			}
		}
		Utils.reparentTo(cardScn, this);
		cardScn.setAllowInteraction(false);
		cardScn.pressed += (x) => inputManager.flowerTableCardPressed(x);
		if (!hasEmptyCard())
		{
			var isEven = cardScns.Count % 2 == 0;
			if (isEven)
			{
				insertEmptyCardAt(cardScns.Count / 2);
			}
			pushEmptyCard();
		}
		renderCards();
	}

	bool hasEmptyCard()
	{
		foreach (var x in cardScns)
		{
			if (!x.card.isValid()) { return true; }
		}
		return false;
	}

	public void highlightCards(List<Card> cards)
	{
		foreach (var x in cardScns)
		{
			if (!x.card.isValid()) { continue; }
			x.setValid(false);
			foreach (var y in cards)
			{
				if (x.card.equal(y))
				{
					x.setValid(true);
				}
			}
		}
	}

	public void unHighlightCards()
	{
		highlightCards(new List<Card>());
	}
	public void removeCard(CardScn cardScn)
	{
		for (int i = 0; i < cardScns.Count; i++)
		{
			if (cardScns[i].card.equal(cardScn.card))
			{
				addEmptyCardAt(i);
				break;
			}
		}
		renderCards();
	}
}