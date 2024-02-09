using System.Collections.Generic;
using Godot;
public partial class PlayerScn : Node2D
{
	public Player player;
	public HandScn handCards;
	public OpenCards openCards;
	bool isActive = false;
	bool isEnemy = false;

	[Signal]
	public delegate void cardSelectedEventHandler(CardScn cardScn);
	public override void _Ready()
	{
		handCards = GetNode<HandScn>("HandScn");
		openCards = GetNode<OpenCards>("OpenCards");

	}

	public void toggleIsEnemy()
	{
		isEnemy = !isEnemy;
		var handPos = handCards.Position;
		handCards.Position = openCards.Position;
		openCards.Position = handPos;
		openCards.Position = new Vector2(-100, openCards.Position.Y);
		handCards.Position = new Vector2(0, handCards.Position.Y);

	}

	public void setPlayer(Player player)
	{
		this.player = player;
	}

	public void highlightHandCards(List<Card> cards)
	{
		handCards.highlightCards(cards);
	}
	public void unHighlightHandCards()
	{
		handCards.unHighlightAll();
	}
	public void setActive(bool val)
	{
		isActive = val;
		foreach (var x in handCards.cardScns)
		{
			x.setAllowInteraction(val);
		}
	}
	public void addHandCard(CardScn cardScn)
	{
		if (isEnemy)
		{
			cardScn.isOpen = false;
		}
		handCards.addCardScn(cardScn);
	}

	public void removeHandCard(CardScn cardScn)
	{
		handCards.removeCard(cardScn);
	}
	public void addOpenCard(CardScn cardScn)
	{
		openCards.addCardScn(cardScn);
	}

	public void setSelectedCard(CardScn cardScn)
	{
		handCards.setSelected(cardScn);
	}
	public void unselectSelectedCard()
	{
	}
}
