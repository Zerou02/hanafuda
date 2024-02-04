using Godot;
using System.Collections.Generic;
public class Flexbox
{
	public static void alignLeft(Rect2 bounds, List<CardScn> cardScns)
	{

		if (cardScns.Count == 0) { return; }
		var cardSize = cardScns[0].getBounds().Size;
		var width = cardSize.X;
		if (bounds.Size.X < cardSize.X * cardScns.Count)
		{
			width = bounds.Size.X / cardScns.Count;
		}
		for (int i = 0; i < cardScns.Count; i++)
		{
			var child = cardScns[i];
			child.Position = new Vector2(bounds.Position.X + i * width, bounds.Position.Y);
		}
	}
}
