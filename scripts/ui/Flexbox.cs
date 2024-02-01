using Godot;
using System.Collections.Generic;
using System.Globalization;
public partial class Flexbox : Node2D
{
	List<CardScn> children = new List<CardScn>();
	Rect2 bounds = new Rect2(0, 0, 500, 100);
	FlexboxModes mode = FlexboxModes.LeftAligned;
	public override void _Ready()
	{
		foreach (var X in this.GetChildren())
		{
			addCard((X as CardScn)!);
		}
	}
	public override void _Process(double delta)
	{
		if (children.Count == 0) { return; }
		if (mode == FlexboxModes.LeftAligned) { alignLeft(bounds, children); }
	}

	public static void alignLeft(Rect2 bounds, List<CardScn> cardScns)
	{

		if (cardScns.Count == 0) { return; }
		int width = (int)cardScns[0].getBounds().Size.X;
		if (bounds.Size.X < width * cardScns.Count) { width = (int)bounds.Size.X / width; }
		for (int i = 0; i < cardScns.Count; i++)
		{
			var child = cardScns[i];
			child.Position = new Vector2(i * width, bounds.Position.Y);
		}
	}
	public void setCards(List<CardScn> cards)
	{
		this.children = cards;
	}

	public void addCard(CardScn card)
	{
		this.children.Add(card);
	}
}
