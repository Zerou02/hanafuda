using System.Collections.Generic;
using Godot;
public partial class DeckScn : Node2D
{
	public CardScn upperCard;
	public CardScn floor;
	public List<CardScn> cards = new List<CardScn>();
	PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
	public override void _Ready()
	{
		upperCard = GetNode<CardScn>("UpperCard");
		floor = GetNode<CardScn>("Floor");
		floor.setCard(Card.GetEmpty());
		floor.setAllowInteraction(false);
		upperCard.setAllowHover(false);
		upperCard.setAllowSelectable(false);
		upperCard.Visible = false;
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
	}
	public void setUpperCard()
	{
		if (cards.Count == 0) { return; }
		upperCard = cards[0];
	}
	public CardScn draw()
	{
		setUpperCard();
		cards.RemoveAt(0);
		return upperCard;
	}
	public void setOpenCardVisibility(bool val)
	{
		upperCard.isOpen = val;
	}
}
