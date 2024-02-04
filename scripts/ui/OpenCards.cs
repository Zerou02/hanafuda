using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class OpenCards : Node2D
{
	public List<CardScn> cardScns = new List<CardScn>();
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public List<CardScn> getCardsScnsOfType(List<CardScn> cardScns, List<Types> types)
	{
		var retList = new List<CardScn>();
		foreach (var x in cardScns)
		{
			foreach (var y in types)
			{
				if (x.card.type == y)
				{
					retList.Add(x);
					break;
				}
			}
		}
		return retList;
	}
	public void renderCards()
	{
		var upperLeft = getCardsScnsOfType(cardScns, new List<Types>() { Types.BlueScroll, Types.PoetryScroll, Types.Scroll });
		var upperRight = getCardsScnsOfType(cardScns, new List<Types>() { Types.Moon, Types.RainMan, Types.Light, Types.CherryBlossom });
		var lowerLeft = getCardsScnsOfType(cardScns, new List<Types>() { Types.Plain });
		var lowerRight = getCardsScnsOfType(cardScns, new List<Types>() { Types.Animal, Types.Butterfly, Types.Boar, Types.Deer, Types.Sake });

		var paddingY = 4;
		Flexbox.alignLeft(new Rect2(0, 0, 200, Constants.cardHeight), upperLeft);
		Flexbox.alignLeft(new Rect2(200, 0, 200, Constants.cardHeight), upperRight);
		Flexbox.alignLeft(new Rect2(0, Constants.cardHeight + paddingY, 200, Constants.cardHeight), lowerLeft);
		Flexbox.alignLeft(new Rect2(200, Constants.cardHeight + paddingY, 200, Constants.cardHeight), lowerRight);
	}
	public void addCardScn(CardScn cardScn)
	{
		this.cardScns.Add(cardScn);
		Utils.reparentTo(cardScn, this);
		cardScn.setAllowInteraction(false);
		renderCards();
	}
}
