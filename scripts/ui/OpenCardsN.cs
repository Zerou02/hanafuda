using Godot;
using System;
using System.Collections.Generic;

public partial class OpenCardsN : Node2D
{
	public List<CardScn> cardScns = new List<CardScn>();
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void renderCards()
	{
		Flexbox.alignLeft(new Rect2(0, 0, 800, 100), cardScns);
	}
	public void addCardScn(CardScn cardScn)
	{
		this.cardScns.Add(cardScn);
		Utils.reparentTo(cardScn, this);
		cardScn.setAllowInteraction(false);
		renderCards();
	}
}
