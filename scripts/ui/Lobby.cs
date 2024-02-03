using System;
using System.Collections.Generic;
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
		peer.PeerConnected += (x) => command(MessageType.InitDeck, Serializer.serializeCards(deck.cards));
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
		hanafuda.uiManager.ownID = playerId;
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

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 0, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void sendMessage(MessageType type, byte[] bytes)
	{
		GD.Print("whoever received");
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
	}

	public void switchPlayer()
	{
		var newActivePlayerID = (hanafuda.uiManager.activePlayerId + 1) % hanafuda.uiManager.players.Length;
		hanafuda.uiManager.setActivePlayer(newActivePlayerID);
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
			}
			else if ((CardPosition)pos.dest == CardPosition.TableCard)
			{
				// endOfCard
				if (pos.destIdx == 255)
				{
					hanafuda.uiManager.moveDeckToTable();
				}
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
		var cardBytes = new byte[3];
		for (int i = 0; i < 3; i++)
		{
			cardBytes[i] = bytes[i];
		}
		var cards = Serializer.deserializeCards(cardBytes);
		var idx = bytes[3];
		hanafuda.uiManager.matchEmptyCard(cards[0], idx);
	}
	public void command(MessageType type, byte[] bytes)
	{
		GD.Print("commd", type);
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
		//Erhält nur Host
		else if (type == MessageType.InitDeck)
		{
			initDeck(bytes);
			Rpc("sendMessage", (int)type, bytes);
			//TODO: Besser machen
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
}
