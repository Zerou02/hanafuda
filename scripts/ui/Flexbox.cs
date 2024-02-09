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
			width = (bounds.Size.X - cardSize.X) / (cardScns.Count - 1);
		}
		for (int i = 0; i < cardScns.Count; i++)
		{
			var child = cardScns[i];
			child.Position = new Vector2(bounds.Position.X + i * width, bounds.Position.Y);
		}
	}

	/// <summary>
	/// Deprecated
	/// </summary>
	/// <param name="bounds"></param>
	/// <param name="label"></param>
	public static void centerLabel(Rect2 bounds, Label label)
	{
		var spacing = 6;
		var letterWidth = 8;
		var sizeX = label.Text.Length * letterWidth + (label.Text.Length - 1) * spacing;

	}
}
