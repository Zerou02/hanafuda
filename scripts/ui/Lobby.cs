using Godot;

public partial class Lobby : Node2D
{
	Button host;
	Button connnect;

	PackedScene hanafudaScn = GD.Load<PackedScene>("scenes/Hanafuda.tscn");
	GameManager? gameManager;
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
		instantiateHanafuda();

		peer.PeerConnected += (x) => { hanafuda.uiManager.setActivePlayer(0); Rpc("sendMessage", (int)MessageType.InitGame, new byte[] { }); };
		hanafuda.uiManager.ownID = 1;
		hanafuda.uiManager.server = this;
	}

	public void instantiateHanafuda()
	{
		var hanafu = hanafudaScn.Instantiate<Hanafuda>();
		host.QueueFree();
		connnect.QueueFree();
		host = null;
		connnect = null;
		this.AddChild(hanafu);
		this.hanafuda = hanafu;
	}
	void createHost()
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateServer(8080, 2);
		GetTree().GetMultiplayer().MultiplayerPeer = peer;
		//	peer.PeerConnected += (x) => sendSignal();
		gameManager = new GameManager(2, this);
		isHost = true;
		instantiateHanafuda();
		hanafuda.uiManager.ownID = 0;
		hanafuda.uiManager.server = this;
	}
	public override void _Process(double delta)
	{
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 0, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void sendMessage(MessageType type, byte[] bytes)
	{
		if (isHost)
		{
			GD.Print("Host received", (MessageType)type);
			if (type == MessageType.InitGame)
			{
				hanafuda.uiManager.setActivePlayer(0);
				hanafuda.uiManager.activePlayerId = 0;
				gameManager!.startGame();
			}
		}
		else
		{
			GD.Print("Client received", (MessageType)type);
			if (type == MessageType.InitDeck)
			{
				initDeck(bytes);
			}
			else if (type == MessageType.MoveCard)
			{
				moveCard(bytes);
			}
		}
		GD.Print("whoever received");
		if (type == MessageType.MatchTableCard)
		{
			matchTableCard(bytes);
		}
		else if (type == MessageType.MatchEmptyTableCard)
		{
			matchEmptyTableCard(bytes);
		}
		else if (type == MessageType.StartDeckTurn)
		{
			if (gameManager != null)
			{
				gameManager.startDeckTurn();
			}
		}
	}

	void initDeck(byte[] bytes)
	{
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
				gameManager?.moveDeckToPlayerHand();
			}
			else if ((CardPosition)pos.dest == CardPosition.TableCard)
			{
				// endOfCard
				if (pos.destIdx == 255)
				{
					hanafuda.uiManager.moveDeckToTable();
					gameManager?.moveDeckToTable();
				}
			}
		}
	}

	public void matchTableCard(byte[] bytes)
	{
		GD.Print("MATCHTABLECARD");
		var cards = Serializer.deserializeCards(bytes);
		if (gameManager != null)
		{
			gameManager.tableCards = Utils.removeCard(gameManager.tableCards, cards[0]);
			gameManager.tableCards = Utils.removeCard(gameManager.tableCards, cards[1]);
			gameManager.activePlayer.openCards.Add(cards[0].clone());
			gameManager.activePlayer.openCards.Add(cards[1].clone());
		}
		hanafuda.uiManager.matchTableCard(cards[0], cards[1]);
	}

	public void matchEmptyTableCard(byte[] bytes)
	{
		var cardBytes = new byte[3];
		for (int i = 0; i < 3; i++)
		{
			cardBytes[i] = bytes[i];
		}
		var cards = Serializer.deserializeCards(cardBytes);
		var idx = bytes[3];
		GD.Print(cards[0].month, cards[0].day, idx);
		if (gameManager != null)
		{
			gameManager.tableCards.Add(cards[0].clone());
		}
		hanafuda.uiManager.matchEmptyCard(cards[0], idx);
	}
	public void command(MessageType type, byte[] bytes)
	{
		GD.Print("commd", type);
		if (type == MessageType.InitDeck)
		{
			initDeck(bytes);
			Rpc("sendMessage", (int)type, bytes);
		}
		else if (type == MessageType.MoveCard)
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
			if (gameManager != null)
			{
				gameManager.startDeckTurn();
				Rpc("sendMessage", (int)type, bytes);
			}
		}
	}
}
