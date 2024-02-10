using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class OpenCards : Node2D
{
	public List<CardScn> cardScns = new List<CardScn>();
	AnimationManager animationManager;
	public override void _Ready()
	{
		animationManager = GetNode<AnimationManager>(Constants.animationManagerPath);
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
		Flexbox.alignLeftAnimated(new Rect2(0, 0, 100, Constants.cardHeight), upperLeft, animationManager);
		Flexbox.alignLeftAnimated(new Rect2(110, 0, 100, Constants.cardHeight), upperRight, animationManager);
		Flexbox.alignLeftAnimated(new Rect2(220, 0, 100, Constants.cardHeight), lowerLeft, animationManager);
		Flexbox.alignLeftAnimated(new Rect2(330, 0, 100, Constants.cardHeight), lowerRight, animationManager);
	}
	public void addCardScn(CardScn cardScn)
	{
		this.cardScns.Add(cardScn);
		cardScn.isOpen = true;
		Utils.reparentTo(cardScn, this);
		cardScn.setAllowInteraction(false);
		renderCards();
	}

}
