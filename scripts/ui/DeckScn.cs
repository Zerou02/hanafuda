using System.Collections.Generic;
using Godot;
public partial class DeckScn : Node2D
{
	public CardScn floor;
	public List<CardScn> cards = new List<CardScn>();
	PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
	public override void _Ready()
	{
		floor = GetNode<CardScn>("Floor");
		floor.setCard(Card.GetEmpty());
		floor.setAllowInteraction(false);
	}

	public override void _Process(double delta)
	{
	}

	public void addCard(CardScn cardScn)
	{
		AddChild(cardScn);
		this.cards.Add(cardScn);
		cardScn.setAllowInteraction(false);
		cardScn.isOpen = false;
		GD.Print(cards.Count);
	}
	public CardScn draw()
	{
		CardScn scn = cards[0];
		cards.RemoveAt(0);
		return scn;
	}
	public void setOpenCardVisibility(bool val)
	{
		if (cards.Count > 0)
		{
			cards[0].isOpen = val;
			cards[0].MoveToFront();
		}
	}
}