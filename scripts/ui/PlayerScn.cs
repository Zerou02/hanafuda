using System.Collections.Generic;
using Godot;
public partial class PlayerScn : Node2D
{
	public Player player;
	public HandScn handCards;
	public OpenCardsN openCards;
	bool isActive = false;

	[Signal]
	public delegate void cardSelectedEventHandler(CardScn cardScn);
	public override void _Ready()
	{
		handCards = GetNode<HandScn>("HandScn");
		openCards = GetNode<OpenCardsN>("OpenCards");

		//handCards.cardSelected += (x) => EmitSignal(SignalName.cardSelected, x);
	}

	public void setPlayer(Player player)
	{
		this.player = player;
		handCards.player = player;
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

	public void update()
	{
		//		handCards.setCards(player.handCards);
		//		openCards.setCards(player.openCards);
	}

	/* public CardScn? getSelectedCard()
	{
		//return handCards.selectedCard;
	} */

	public void addHandCard(CardScn cardScn)
	{
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
		//	handCards.selectedCard = null;
	}
}
