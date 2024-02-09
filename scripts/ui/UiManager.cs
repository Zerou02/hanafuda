using Godot;
using System.Collections.Generic;

public enum UiModes { PlayerTurn, DeckTurn, UiTurn };
public partial class UiManager : Node2D
{
    public DeckScn deck;
    public TableCards tableCards;
    public PlayerScn[] players = new PlayerScn[2];
    public PlayerScn activePlayer;
    public int ownID = -1;
    public int activePlayerId = 0;
    public UiModes uiMode = UiModes.PlayerTurn;

    public int amountKoiKois = 1;

    public int ownPoints = 30;
    public int enemyPoints = 30;
    public int currTurn = 0;
    public int maxTurn = 1;
    PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
    InputManager inputManager;
    public Lobby server;
    public Label activeTurnDisplay;
    public Label turnCounter;
    public Label ownPointsLabel;
    public Label enemyPointsLabel;
    KoiKoiSelection koiKoiSelection;
    Control uiRoot;
    Marker2D ownMarker;
    Marker2D enemyMarker;
    public Control loadingScreen;
    public Dictionary<Sets, List<Card>>? lastSet = null;
    public int outOfCards = 0;
    public int lastStartedId = 0;
    Label ownName;
    Label enemyName;
    public string playerName = "";
    public string enemyPlayerName = "";

    public int rematchPressed = 0;
    public GameOverScreen gameOverScreen;

    public override void _Ready()
    {
        inputManager = GetNode<InputManager>(Constants.inputManagerPath);
        uiRoot = GetNode<Control>("UiRoot");
        koiKoiSelection = GetNode<KoiKoiSelection>("UiRoot/KoiKoiSelection");
        activeTurnDisplay = GetNode<Label>("UiRoot/ActiveTurnDisplay");
        turnCounter = GetNode<Label>("UiRoot/TurnCounter");
        ownPointsLabel = GetNode<Label>("UiRoot/OwnPoints");
        enemyPointsLabel = GetNode<Label>("UiRoot/EnemyPoints");
        loadingScreen = GetNode<Control>("UiRoot/LoadingScreen");
        ownName = GetNode<Label>("UiRoot/OwnName");
        enemyName = GetNode<Label>("UiRoot/EnemyName");

        deck = GetNode<DeckScn>("DeckScn");
        tableCards = GetNode<TableCards>("TableCards");
        ownMarker = GetNode<Marker2D>("OwnPlayerMarker");
        enemyMarker = GetNode<Marker2D>("EnemyPlayerMarker");
        gameOverScreen = GetNode<GameOverScreen>("GameOverScreen");

        players[0] = GetNode<PlayerScn>("EnemyPlayerMarker/EnemyPlayer");
        players[1] = GetNode<PlayerScn>("OwnPlayerMarker/OwnPlayer");
        inputManager.uiManager = this;
        koiKoiSelection.Visible = false;
        koiKoiSelection.koiKoiPressed += () => handleKoiKoiPressed();
        koiKoiSelection.stopPressed += () => handleStop();
        //var example = new Dictionary<Sets, List<Card>>() { { Sets.Tsukimi, new List<Card>() { new Card(2, 0), new Card(2, 2) } }, { Sets.Hanami, new List<Card>() { new Card(3, 0), new Card(3, 2) } }, { Sets.Plain, new List<Card>() { new Card(4, 0), new Card(5, 0) } } };
        loadingScreen.Visible = true;
        gameOverScreen.QuitClicked += () => server.command(MessageType.Quit, new byte[] { });
        gameOverScreen.RematchClicked += () => server.command(MessageType.Rematch, new byte[] { });
    }

    public override void _Process(double delta)
    {
        turnCounter.Text = "Runde: " + currTurn.ToString() + " / " + maxTurn.ToString();
        ownPointsLabel.Text = "                  " + ownPoints.ToString();
        enemyPointsLabel.Text = "                  " + enemyPoints.ToString();
        activeTurnDisplay.Text = ownID == activePlayerId ? "Du bist am Zug" : "Der Gegner ist am Zug";
        ownName.Text = playerName;
        enemyName.Text = enemyPlayerName;
    }

    public void setActivePlayer(int activeId)
    {
        uiMode = UiModes.PlayerTurn;
        foreach (var x in players)
        {
            x.setActive(false);
            x.unselectSelectedCard();
        }
        activePlayerId = activeId;
        activePlayer = players[activeId];
        activePlayer.setActive(true);
        highlightHandCards();
    }

    public void clearAll()
    {
        lastSet = null;
        tableCards.cardScns = Utils.clearCardsScns(tableCards.cardScns);
        deck.cards = Utils.clearCardsScns(deck.cards);
        foreach (var x in players)
        {
            x.handCards.cardScns = Utils.clearCardsScns(x.handCards.cardScns);
            x.openCards.cardScns = Utils.clearCardsScns(x.openCards.cardScns);
        }
    }
    public void moveDeckToPlayerHand(int playerId)
    {
        var card = deck.draw();
        card.type = CardType.Hand;
        card.isOpen = true;
        card.setAllowInteraction(playerId == ownID && activePlayerId == ownID);
        players[playerId].addHandCard(card);
    }

    public void moveDeckToTable()
    {
        var card = deck.draw();
        card.type = CardType.Table;
        card.isOpen = true;
        tableCards.addCard(card);
        highlightHandCards();

    }

    public void setDeck(List<Card> cards)
    {
        foreach (var x in cards)
        {
            var scn = cardScn.Instantiate<CardScn>();
            scn.type = CardType.Deck;
            deck.addCard(scn);
            scn.setCard(x.clone());
        }
    }
    List<Card> getValidHandCards()
    {
        var retList = new List<Card>();
        foreach (var x in activePlayer.handCards.cardScns)
        {
            foreach (var y in tableCards.cardScns)
            {
                if (y.card.isValid() && y.card.month == x.card.month)
                {
                    retList.Add(x.card);
                    break;
                }
            }
        }
        return retList;
    }

    public void unHighlightTableCards()
    {
        tableCards.highlightCards(new List<Card>());
    }
    public void highlightTableCards(CardScn cardScn)
    {
        var highlightList = new List<Card>();
        foreach (var x in tableCards.cardScns)
        {
            if (!x.card.isValid()) { continue; }
            if (cardScn.card.month == x.card.month)
            {
                highlightList.Add(x.card);
                continue;
            }
        }
        tableCards.highlightCards(highlightList);
    }

    void highlightHandCards()
    {
        if (ownID != activePlayerId) { return; }
        var cards = getValidHandCards();
        activePlayer.highlightHandCards(cards);
    }

    public void setUiMode(UiModes uiMode)
    {
        this.uiMode = uiMode;
        if (uiMode == UiModes.DeckTurn)
        {
            activePlayer.setActive(false);
        }
        else
        {

        }
    }
    public void setOpenCardVis(bool val)
    {
        deck.setOpenCardVisibility(val);
    }

    public void addToEmptyTableCard(CardScn handCard, int idx)
    {
        handCard.type = CardType.Table;
        activePlayer.removeHandCard(handCard);
        handCard.setSelected(false);
        handCard.setAllowInteraction(false);
        tableCards.addCard(handCard);
    }

    public CardScn findCard(List<CardScn> cardScns, Card toFind)
    {
        CardScn retScn = null;
        foreach (var x in cardScns)
        {
            if (x.card.isEqual(toFind))
            {
                retScn = x;
                break;
            }
        }
        return retScn;
    }

    public void matchTableCardDeck(Card tableCard)
    {
        var deckScn = deck.draw();
        deckScn.isOpen = true;
        var tableCardScn = findCard(tableCards.cardScns, tableCard);
        deckScn.type = CardType.Open;
        tableCardScn.type = CardType.Open;
        deckScn.setSelected(false);
        activePlayer.addOpenCard(tableCardScn);
        activePlayer.addOpenCard(deckScn);
        tableCards.removeCard(tableCardScn);
    }

    public void matchTableCard(Card handCard, Card tableCard)
    {
        var handCardScn = findCard(activePlayer.handCards.cardScns, handCard);
        var tableCardScn = findCard(tableCards.cardScns, tableCard);
        handCardScn.type = CardType.Open;
        tableCardScn.type = CardType.Open;
        handCardScn.setSelected(false);
        activePlayer.addOpenCard(tableCardScn);
        activePlayer.addOpenCard(handCardScn);
        tableCards.removeCard(tableCardScn);
        activePlayer.removeHandCard(handCardScn);
    }

    public void selectHandCard(CardScn cardScn)
    {
        activePlayer.setSelectedCard(cardScn);
    }

    public void setOwnId(int id)
    {
        ownID = id;
        if (ownID == 0)
        {
            Utils.reparentTo(players[0], ownMarker);
            Utils.reparentTo(players[1], enemyMarker);
            players[0].handCards.Position = new Vector2(0, 70);
            players[1].handCards.Position = new Vector2(0, 90);

            players[1].toggleIsEnemy();
        }
        else
        {
            Utils.reparentTo(players[1], ownMarker);
            Utils.reparentTo(players[0], enemyMarker);
            players[1].handCards.Position = new Vector2(0, 70);
            players[0].handCards.Position = new Vector2(0, 90);

            players[0].toggleIsEnemy();
        }
        players[0].Position = new Vector2(0, 0);
        players[1].Position = new Vector2(0, 0);
    }

    public void matchEmptyCard(Card card, int idx)
    {
        var cardScn = findCard(activePlayer.handCards.cardScns, card);
        activePlayer.removeHandCard(cardScn);
        cardScn.type = CardType.Table;
        cardScn.setAllowInteraction(false);
        tableCards.overwriteEmptyCard(cardScn, idx);
    }

    public bool belongsToActivePlayer(CardScn card)
    {
        var retVal = false;
        foreach (var x in activePlayer.handCards.cardScns)
        {
            if (x.card.isEqual(card.card))
            {
                retVal = true;
            }
        }
        return retVal;
    }

    public bool isNewKoiKoi(Dictionary<Sets, List<Card>> toCheck)
    {
        if (lastSet == null || lastSet.Count != toCheck.Count) { return true; }
        var retVal = false;
        foreach (var x in lastSet)
        {
            if (x.Key == Sets.Plain || x.Key == Sets.Scrolls || x.Key == Sets.Animals) { continue; }
            if (!Utils.cardListsSame(x.Value, toCheck[x.Key]))
            {
                retVal = true;
                break;
            }
        }
        return retVal;
    }

    public void displayKoiKoiMenu(Dictionary<Sets, List<Card>> cards)
    {
        var isEmpty = activePlayer.handCards.cardScns.Count == 0;
        if (!isNewKoiKoi(cards))
        {
            if (isEmpty)
            {
                server.command(MessageType.OutOfCards, new byte[] { });
            }
            else
            {
                server.command(MessageType.SwitchPlayer, new byte[] { });
            }
            return;
        }
        koiKoiSelection.koiKoiButton.Visible = !isEmpty;
        lastSet = cards;
        koiKoiSelection.setCards(cards, amountKoiKois);
        koiKoiSelection.MoveToFront();
        uiMode = UiModes.UiTurn;
    }

    public void handleKoiKoiPressed()
    {
        server.command(MessageType.KoiKoiPressed, new byte[] { });
        server.command(MessageType.SwitchPlayer, new byte[] { });
    }

    public void handleStop()
    {
        if (lastSet == null) { return; }
        var points = GameManager.calculateTotalPoints(lastSet, amountKoiKois);
        server.command(MessageType.RoundEnded, new byte[] { (byte)points });
    }
    public void handlerServerKoiKoiPressed()
    {
        this.amountKoiKois += 1;
    }

    public void handleDeckChoose()
    {
        setOpenCardVis(true);
        if (activePlayerId != ownID) { return; }
        highlightTableCards(deck.cards[0]);
        uiMode = UiModes.DeckTurn;
    }
}