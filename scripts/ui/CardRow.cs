using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Godot;
public partial class CardRow : Node2D
{
	PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
	public CardScn selectedCard;
	public List<CardScn> cardScns = new List<CardScn>();
	public bool allowCardInteraction = false;

	[Signal]
	public delegate void cardSelectedEventHandler(CardScn cardScn);
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void handleCardPress(int idx)
	{
		var children = this.GetChildren();
		var isSelected = (children[idx] as CardScn)!.isSelected;
		foreach (var x in children)
		{
			(x as CardScn)!.setSelected(false);
		}
		if (allowCardInteraction)
		{
			(children[idx] as CardScn)!.setSelected(!isSelected);
			if (isSelected)
			{
				selectedCard = null;
			}
			else
			{
				selectedCard = (children[idx] as CardScn)!;
				EmitSignal(SignalName.cardSelected, selectedCard);
			}
		}
	}

	public void setAllowCardInteraction(bool val)
	{
		allowCardInteraction = val;
		var children = this.GetChildren();
		foreach (var x in children)
		{
			var card = x as CardScn;
			card.setAllowSelectable(val);
			if (!val)
			{
				card.setSelected(false);
			}
		}
	}

	public void setCards(List<Card> cards)
	{
		cardScns = new List<CardScn>();
		Card selected = null;
		if (selectedCard != null)
		{
			selected = selectedCard.card.clone();
		}
		MinosUtils.removeChildren(this);
		if (cards.Count == 0)
		{
			return;
		}
		for (int i = 0; i < cards.Count; i++)
		{
			var scn = cardScn.Instantiate<CardScn>();
			cardScns.Add(scn);
			this.AddChild(scn);
			scn.setCard(cards[i]);
			scn.setAllowSelectable(allowCardInteraction);
			if (selected != null && cards[i].equal(selected))
			{
				scn.setSelected(true);
			}
			var idx = i;
			//	scn.pressed += () => handleCardPress(idx);
			//scn.GlobalPosition = Position with { X = this.GlobalPosition.X + i * scn.sprite2D.size.X, Y = this.GlobalPosition.Y };
		}
	}

	public void sortCards()
	{
		List<CardScn> lightCards = new List<CardScn>();
		List<CardScn> animalCards = new List<CardScn>();
		List<CardScn> scrollCards = new List<CardScn>();
		List<CardScn> plainCards = new List<CardScn>();
		foreach (var x in cardScns)
		{
			if (x.card.type == Types.Light || x.card.type == Types.CherryBlossom || x.card.type == Types.RainMan || x.card.type == Types.Moon)
			{
				lightCards.Add(x);
			}
			else if (x.card.type == Types.Plain)
			{
				plainCards.Add(x);
			}
			else if (x.card.type == Types.Scroll || x.card.type == Types.BlueScroll || x.card.type == Types.PoetryScroll)
			{
				scrollCards.Add(x);
			}
			else
			{
				animalCards.Add(x);
			}
		}
	}
	public void highlightCards(List<Card> cards)
	{
		foreach (var x in cardScns)
		{
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

	public void unHighlightAll()
	{
		highlightCards(new List<Card>());
	}
}
