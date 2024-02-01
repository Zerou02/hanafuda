using Godot;
public partial class InputManager : Node
{
	UiManager uiManager;
	CardScn? selectedCard;
	public void init(UiManager uiManager)
	{
		this.uiManager = uiManager;
	}

	public void handCardSelected(CardScn cardScn, int playerId)
	{
		GD.Print("a");
		selectedCard = cardScn;
		uiManager.selectHandCard(cardScn, playerId);
		uiManager.highlightTableCards(cardScn);
	}

	public void emptyTableCardPressed(int idx)
	{
		if (selectedCard == null) { return; }
		uiManager.addToEmptyTableCard(selectedCard, idx);
		selectedCard = null;
	}

	public void flowerTableCardPressed(CardScn cardScn)
	{
		if (selectedCard == null) { return; }
		if (cardScn.card.month != selectedCard.card.month) { return; }
		uiManager.matchTableCard(selectedCard, cardScn);
		selectedCard = null;
	}
}
