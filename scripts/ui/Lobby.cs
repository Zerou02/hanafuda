using System;
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
		GD.Print("connected?");
		instantiateHanafuda();

		peer.PeerConnected += (x) => { Rpc("sendMessage", (int)MessageType.InitGame); };
		hanafuda.uiManager.ownID = 1;

	}

	public void instantiateHanafuda()
	{
		var hanafu = hanafudaScn.Instantiate<Hanafuda>();
		GD.Print("A");
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
		gameManager = new GameManager(2);
		isHost = true;
		instantiateHanafuda();
		GD.Print("B");
		hanafuda.uiManager.ownID = 0;
		GD.Print("created");
	}
	public override void _Process(double delta)
	{
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 0, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void sendMessage(MessageType type)
	{
		if (type == MessageType.InitGame)
		{
			gameManager.startGame();
		}
		GD.Print("test", this.gameManager, type);
		if (this.isHost)
		{
			Rpc("sendMessage", 0x2);
		}
	}
}
