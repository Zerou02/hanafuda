using System.Collections.Generic;
using Godot;
public partial class InputManager : Node
{
	public UiManager uiManager;
	CardScn? selectedCard;
	public void init(UiManager uiManager)
	{
		this.uiManager = uiManager;
	}

	public void handCardSelected(CardScn cardScn)
	{
		selectedCard = cardScn;
		uiManager.selectHandCard(cardScn);
		uiManager.highlightTableCards(cardScn);
	}

	public void emptyTableCardPressed(int idx)
	{
		if (selectedCard == null) { return; }
		var cardBytes = Card.serialize(selectedCard.card);
		var bytes = new byte[4];
		cardBytes.CopyTo(bytes, 0);
		bytes[3] = (byte)idx;
		uiManager.server.command(MessageType.MatchEmptyTableCard, bytes);
		selectedCard = null;
		uiManager.unHighlightTableCards();
		uiManager.server.command(MessageType.StartDeckTurn, new byte[] { });
	}

	public void flowerTableCardPressed(CardScn cardScn)
	{
		if (selectedCard == null) { return; }
		if (cardScn.type != CardType.Table || cardScn.card.month != selectedCard.card.month) { return; }
		uiManager.server.command(MessageType.MatchTableCard, Serializer.serializeCards(new List<Card>() { selectedCard.card, cardScn.card }));
		selectedCard = null;
		uiManager.unHighlightTableCards();
		uiManager.server.command(MessageType.StartDeckTurn, new byte[] { });
	}

	public void mouseEnteredOnCard(CardScn cardScn)
	{
		if (cardScn.type == CardType.Hand && uiManager.ownID == uiManager.activePlayerId)
		{
			cardScn.setHover(true);
		}
	}

	public void mouseExitedOnCard(CardScn card)
	{
		card.setHover(false);
	}

	public void cardPressed(CardScn cardScn)
	{
		if (uiManager.ownID != uiManager.activePlayerId) { return; }
		if (cardScn.type == CardType.Hand)
		{
			handCardSelected(cardScn);
		}
		else if (cardScn.type == CardType.Table)
		{
			if (cardScn.card.isValid())
			{
				flowerTableCardPressed(cardScn);
			}
		}
	}
}
