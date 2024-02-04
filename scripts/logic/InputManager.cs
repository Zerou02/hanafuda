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
		if (uiManager.uiMode != UiModes.PlayerTurn) { return; }
		selectedCard = cardScn;
		uiManager.selectHandCard(cardScn);
		uiManager.highlightTableCards(cardScn);
	}

	public void emptyTableCardPressed(int idx)
	{
		if (selectedCard == null) { return; }
		var cardBytes = Card.serialize(selectedCard.card);
		var bytes = new byte[Constants.serializedCardLength + 1];
		cardBytes.CopyTo(bytes, 0);
		bytes[Constants.serializedCardLength] = (byte)idx;
		uiManager.server.command(MessageType.MatchEmptyTableCard, bytes);
		selectedCard = null;
		uiManager.unHighlightTableCards();
		uiManager.activePlayer.unHighlightHandCards();
		uiManager.server.command(MessageType.StartDeckTurn, new byte[] { });
	}

	public void flowerTableCardPressed(CardScn cardScn)
	{
		if (!cardScn.card.isValid()) { return; }
		if (uiManager.uiMode == UiModes.DeckTurn)
		{
			var card = uiManager.deck.cards[0].card.clone();
			if (cardScn.card.month != card.month) { return; }
			uiManager.activePlayer.unHighlightHandCards();
			uiManager.server.command(MessageType.MatchTableCardWithDeck, Serializer.serializeCards(new List<Card>() { card, cardScn.card }));
			uiManager.server.command(MessageType.UiModeSetPlayerTurn, new byte[] { });
			uiManager.server.command(MessageType.SwitchPlayer, new byte[] { });

		}
		else
		{
			if (selectedCard == null) { return; }
			if (cardScn.type != CardType.Table || cardScn.card.month != selectedCard.card.month) { return; }
			uiManager.activePlayer.unHighlightHandCards();
			uiManager.server.command(MessageType.MatchTableCard, Serializer.serializeCards(new List<Card>() { selectedCard.card, cardScn.card }));
			selectedCard = null;
			uiManager.unHighlightTableCards();
			uiManager.server.command(MessageType.StartDeckTurn, new byte[] { });
		}
	}

	public void mouseEnteredOnCard(CardScn cardScn)
	{
		if (cardScn.type == CardType.Hand && uiManager.ownID == uiManager.activePlayerId && uiManager.belongsToActivePlayer(cardScn))
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
		if (cardScn.type == CardType.Hand && uiManager.belongsToActivePlayer(cardScn))
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
