using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Lobby : Node2D
{
	Button host;
	Button connnect;
	PackedScene hanafudaScn = GD.Load<PackedScene>("scenes/Hanafuda.tscn");
	Hanafuda hanafuda;
	ENetMultiplayerPeer peer;
	bool isHost;
	public override void _Ready()
	{
		host = GetNode<Button>("Host");
		connnect = GetNode<Button>("Connect");

		host.Pressed += () => createHost();
		connnect.Pressed += () => createPeer();
	}

	void createPeer()
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateClient("127.0.0.1", 8080);
		GetTree().GetMultiplayer().MultiplayerPeer = peer;
		isHost = false;
		instantiateHanafuda(1);
		peer.PeerConnected += (x) => command(MessageType.SetActivePlayer, new byte[] { 0 });
		var deck = new Deck();
		peer.PeerConnected += (x) =>
		{
			command(MessageType.PeerConnected, new byte[] { });
			command(MessageType.InitDeck, Serializer.serializeCards(deck.cards));
		};
	}

	public void instantiateHanafuda(int playerId)
	{
		var hanafu = hanafudaScn.Instantiate<Hanafuda>();
		host.QueueFree();
		connnect.QueueFree();
		host = null;
		connnect = null;
		this.AddChild(hanafu);
		this.hanafuda = hanafu;
		hanafuda.uiManager.setOwnId(playerId);
		hanafuda.uiManager.server = this;
	}
	void createHost()
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateServer(8080, 2);
		GetTree().GetMultiplayer().MultiplayerPeer = peer;
		isHost = true;
		instantiateHanafuda(0);
		hanafuda.uiManager.server = this;
	}

	public override void _Process(double delta)
	{
	}
	public void switchPlayer()
	{
		GD.Print("switch from", hanafuda.uiManager.activePlayerId);
		var newActivePlayerID = (hanafuda.uiManager.activePlayerId + 1) % hanafuda.uiManager.players.Length;
		hanafuda.uiManager.unHighlightTableCards();
		hanafuda.uiManager.activePlayer.handCards.highlightCards(new List<Card>());
		hanafuda.uiManager.setActivePlayer(newActivePlayerID);
	}
	void initDeck(byte[] bytes)
	{
		hanafuda.uiManager.clearAll();
		hanafuda.uiManager.setDeck(Serializer.deserializeCards(bytes));
	}

	void moveCard(byte[] bytes)
	{
		var pos = Serializer.deSerializeCardMove(bytes);

		if ((CardPosition)pos.source == CardPosition.Deck)
		{
			if ((CardPosition)pos.dest == CardPosition.Hand)
			{
				hanafuda.uiManager.moveDeckToPlayerHand(pos.destIdx);
			}
			else if ((CardPosition)pos.dest == CardPosition.TableCard)
			{
				hanafuda.uiManager.moveDeckToTable();
			}
		}
	}

	public void matchTableCardDeck(byte[] bytes)
	{
		var cards = Serializer.deserializeCards(bytes);
		hanafuda.uiManager.matchTableCardDeck(cards[1]);
		hanafuda.uiManager.setOpenCardVis(false);
	}

	public void matchTableCard(byte[] bytes)
	{
		var cards = Serializer.deserializeCards(bytes);
		hanafuda.uiManager.matchTableCard(cards[0], cards[1]);
	}

	public void matchEmptyTableCard(byte[] bytes)
	{
		var cardBytes = new byte[Constants.serializedCardLength];
		for (int i = 0; i < Constants.serializedCardLength; i++)
		{
			cardBytes[i] = bytes[i];
		}
		var cards = Serializer.deserializeCards(cardBytes);
		var idx = bytes[Constants.serializedCardLength];
		hanafuda.uiManager.matchEmptyCard(cards[0], idx);
	}


	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 0, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void sendMessage(MessageType type, byte[] bytes)
	{
		if (type == MessageType.MatchTableCard)
		{
			matchTableCard(bytes);
		}
		else if (type == MessageType.MatchEmptyTableCard)
		{
			matchEmptyTableCard(bytes);
		}
		else if (type == MessageType.MatchTableCardWithDeck)
		{
			matchTableCardDeck(bytes);
		}
		else if (type == MessageType.SwitchPlayer)
		{
			switchPlayer();
		}
		else if (type == MessageType.InitDeck)
		{
			initDeck(bytes);
		}
		else if (type == MessageType.SetActivePlayer)
		{
			hanafuda.uiManager.setActivePlayer(bytes[0]);
		}
		else if (type == MessageType.MoveCard)
		{
			moveCard(bytes);
		}
		else if (type == MessageType.OpenDeckCard)
		{
			hanafuda.uiManager.setOpenCardVis(true);
		}
		else if (type == MessageType.DeckChoose)
		{
			handleDeckChoose();
		}
		else if (type == MessageType.UiModeSetPlayerTurn)
		{
			hanafuda.uiManager.uiMode = UiModes.PlayerTurn;
		}
		else if (type == MessageType.CheckHasSet)
		{
			checkHasSet();
		}
		else if (type == MessageType.KoiKoiPressed)
		{
			hanafuda.uiManager.handlerServerKoiKoiPressed();
		}
		else if (type == MessageType.PeerConnected)
		{
			command(MessageType.InitGame, Serializer.serializeGameConfig(0, 3, 30));
		}
		else if (type == MessageType.InitGame)
		{
			initGame(Serializer.deserializeGameConfig(bytes));
			command(MessageType.StartRound, new byte[] { (byte)hanafuda.uiManager.activePlayerId });
		}
		else if (type == MessageType.StartRound)
		{
			startRound(bytes[0]);
		}
		else if (type == MessageType.ChangePoints)
		{
			changePoints(bytes[0]);
		}
	}

	public void command(MessageType type, byte[] bytes)
	{
		GD.Print("commd", type, ",", hanafuda.uiManager.activePlayerId);
		if (type == MessageType.MoveCard)
		{
			moveCard(bytes);
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.MatchTableCard)
		{
			matchTableCard(bytes);
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.MatchEmptyTableCard)
		{
			matchEmptyTableCard(bytes);
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.StartDeckTurn)
		{
			GameManager.startDeckTurn(this, getDeckCard(), getTableCards());
		}
		else if (type == MessageType.MatchTableCardWithDeck)
		{
			matchTableCardDeck(bytes);
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.SwitchPlayer)
		{
			switchPlayer();
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.InitDeck)
		{
			GD.Print("init");
			Rpc("sendMessage", (int)type, bytes);
			initDeck(bytes);
			GameManager.handoutCardsAtStartOfGame(this, new List<int>() { 0, 1 });
		}
		else if (type == MessageType.SetActivePlayer)
		{
			hanafuda.uiManager.setActivePlayer(bytes[0]);
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.OpenDeckCard)
		{
			hanafuda.uiManager.setOpenCardVis(true);
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.DeckChoose)
		{
			handleDeckChoose();
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.UiModeSetPlayerTurn)
		{
			hanafuda.uiManager.uiMode = UiModes.PlayerTurn;
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.CheckHasSet)
		{
			checkHasSet();
		}
		else if (type == MessageType.DisplayKoiKoiMenu)
		{
			var set = Serializer.deserializeSet(bytes);
			hanafuda.uiManager.displayKoiKoiMenu(calcCardsToSet(getOpenCards(), set));
		}
		else if (type == MessageType.KoiKoiPressed)
		{
			hanafuda.uiManager.handlerServerKoiKoiPressed();
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.PeerConnected)
		{
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.InitGame)
		{
			initGame(Serializer.deserializeGameConfig(bytes));
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.StartRound)
		{
			startRound(bytes[0]);
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.RoundEnded)
		{
			command(MessageType.ChangePoints, bytes);
			if (hanafuda.uiManager.ownPoints == 0 || hanafuda.uiManager.enemyPoints == 0)
			{
				command(MessageType.GameEnded, new byte[] { });
				todog();
			}
			else
			{
				command(MessageType.InitDeck, Serializer.serializeCards(new Deck().cards));
				command(MessageType.StartRound, new byte[] { (byte)(hanafuda.uiManager.activePlayerId == 1 ? 0 : 1) });
			}
		}
		else if (type == MessageType.ChangePoints)
		{
			changePoints(bytes[0]);
			Rpc("sendMessage", (int)type, bytes);
		}
	}

	void changePoints(int points)
	{
		if (hanafuda.uiManager.ownID == hanafuda.uiManager.activePlayerId)
		{
			hanafuda.uiManager.ownPoints += points;
			hanafuda.uiManager.enemyPoints -= points;
		}
		else
		{
			hanafuda.uiManager.ownPoints -= points;
			hanafuda.uiManager.enemyPoints += points;
		}
	}
	void startRound(int playerId)
	{
		hanafuda.uiManager.setActivePlayer(playerId);
	}

	void initGame(GameConfig config)
	{
		hanafuda.uiManager.maxTurn = config.maxTurns;
		hanafuda.uiManager.ownPoints = config.startPoints;
		hanafuda.uiManager.enemyPoints = config.startPoints;
		hanafuda.uiManager.activePlayerId = config.startPlayerId;

	}
	void increment<T>(Dictionary<T, int> dict, T key) where T : notnull
	{

		if (!dict.TryAdd(key, 1))
		{
			dict[key] = dict[key] + 1;
		}
	}

	Dictionary<Sets, List<Card>> calcCardsToSet(List<Card> cards, bool[] set)
	{
		var retDict = new Dictionary<Sets, List<Card>>();
		var stringifiedSets = new string[] { "Plain,Scrolls,Animals,BlueScrolls,PoetryScrolls,InoShikaChou,Tsukimi,Hanami,Sankou,Ameshikou,Shikou,Gokou" };
		var setToTypeMap = new Dictionary<Sets, Types[]>(){
			{Sets.Plain,new Types[]{Types.Plain,Types.Sake}},
			{Sets.Scrolls,new Types[]{Types.Scroll,Types.BlueScroll,Types.PoetryScroll}},
			{Sets.Animals,new Types[]{Types.Animal,Types.Deer,Types.Boar,Types.Butterfly,Types.Sake}},
			{Sets.BlueScrolls,new Types[]{Types.BlueScroll}},
			{Sets.PoetryScrolls,new Types[]{Types.PoetryScroll}},
			{Sets.InoShikaChou,new Types[]{Types.Deer,Types.Boar,Types.Butterfly}},
			{Sets.Tsukimi,new Types[]{Types.Moon,Types.Sake}},
			{Sets.Hanami,new Types[]{Types.CherryBlossom,Types.Sake}},
			{Sets.Sankou,new Types[]{Types.Light,Types.Moon,Types.CherryBlossom}},
			{Sets.Ameshikou,new Types[]{Types.Light,Types.Moon,Types.RainMan,Types.CherryBlossom}},
			{Sets.Shikou,new Types[]{Types.Light,Types.Moon,Types.CherryBlossom}},
			{Sets.Gokou,new Types[]{Types.Light,Types.Moon,Types.RainMan,Types.CherryBlossom}}
		};

		for (int i = 0; i < set.Length; i++)
		{
			if (set[i])
			{
				var list = new List<Card>();
				cards.ForEach(x =>
				{
					if (setToTypeMap[(Sets)i].Contains(x.type))
					{
						list.Add(x);
					}
				});
				retDict.Add((Sets)i, list);
			}
		}
		return retDict;
	}

	Dictionary<Types, int> countCardTypes(List<Card> cards)
	{
		var dict = new Dictionary<Types, int>() { { Types.Plain, 0 }, { Types.Scroll, 0 }, { Types.BlueScroll, 0 }, { Types.PoetryScroll, 0 }, { Types.Sake, 0 }, { Types.Animal, 0 }, { Types.Boar, 0 }, { Types.Butterfly, 0 }, { Types.Deer, 0 }, { Types.CherryBlossom, 0 }, { Types.RainMan, 0 }, { Types.Moon, 0 }, { Types.Light, 0 } };
		cards.ForEach(x =>
		{
			increment(dict, x.type);
		});
		return dict;
	}


	void fillFoundSets(bool[] set, Dictionary<Types, int> cardCount)
	{
		void fillEntry(bool[] set, bool cond, Sets key) { set[(int)key] = cond; }
		fillEntry(set, cardCount[Types.Plain] + cardCount[Types.Sake] >= 10, Sets.Plain);
		fillEntry(set, cardCount[Types.Scroll] + cardCount[Types.BlueScroll] + cardCount[Types.PoetryScroll] >= 5, Sets.Scrolls);
		fillEntry(set, cardCount[Types.Animal] + cardCount[Types.Sake] + cardCount[Types.Deer] + cardCount[Types.Boar] + cardCount[Types.Butterfly] >= 5, Sets.Animals);
		fillEntry(set, cardCount[Types.BlueScroll] == 3, Sets.BlueScrolls);
		fillEntry(set, cardCount[Types.PoetryScroll] == 3, Sets.PoetryScrolls);
		fillEntry(set, cardCount[Types.Deer] + cardCount[Types.Boar] + cardCount[Types.Butterfly] == 3, Sets.PoetryScrolls);
		fillEntry(set, cardCount[Types.Moon] + cardCount[Types.Sake] == 2, Sets.Tsukimi);
		fillEntry(set, cardCount[Types.CherryBlossom] + cardCount[Types.Sake] == 2, Sets.Hanami);
		fillEntry(set, cardCount[Types.Light] + cardCount[Types.Moon] == 3, Sets.Sankou);
		fillEntry(set, cardCount[Types.Light] + cardCount[Types.Moon] == 4, Sets.Shikou);
		fillEntry(set, cardCount[Types.Light] + cardCount[Types.RainMan] + cardCount[Types.Moon] == 5, Sets.Gokou);
		fillEntry(set, cardCount[Types.Light] + cardCount[Types.RainMan] + cardCount[Types.Moon] == 4, Sets.Ameshikou);
	}

	bool[] calcSet()
	{
		var cards = Utils.cloneCards(hanafuda.uiManager.activePlayer.openCards.cardScns);
		var counts = countCardTypes(cards);
		var set = new bool[12];
		fillFoundSets(set, counts);
		return set;
	}

	void checkHasSet()
	{
		if (hanafuda.uiManager.ownID != hanafuda.uiManager.activePlayerId) { return; }
		var set = calcSet();
		if (set.Contains(true))
		{
			//	this.command(MessageType.DisplaySetsFlash, new byte[] { });
			this.command(MessageType.DisplayKoiKoiMenu, Serializer.serializeSet(set));
		}
		else
		{
			GD.Print("BEEVVY");
			this.command(MessageType.SwitchPlayer, new byte[] { });
		}
	}

	public void todog()
	{
		throw new Exception("TODOG!");
	}
	public void handleDeckChoose()
	{
		hanafuda.uiManager.handleDeckChoose();
	}

	public Card getDeckCard()
	{
		return hanafuda.uiManager.deck.cards[0].card.clone();
	}

	public List<Card> getTableCards()
	{
		return Utils.cloneCards(hanafuda.uiManager.tableCards.cardScns);
	}

	public List<Card> getOpenCards()
	{
		return Utils.cloneCards(hanafuda.uiManager.activePlayer.openCards.cardScns);
	}
}
