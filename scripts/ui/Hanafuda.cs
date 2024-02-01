using Godot;
using System;

public partial class Hanafuda : Node2D
{
	public UiManager uiManager;
	public override void _Ready()
	{
		//		var inputManger = GetNode<InputManager>("/root/InputManager");
		uiManager = GetNode<UiManager>("UiManager");
		//		gameManager = new GameManager(2);
		//		uiManager.init(gameManager.deck);
		//		uiManager.initPlayers(gameManager.players[0], gameManager.players[1]);
		//		inputManger.init(uiManager);
		//		uiManager.gameManager = gameManager;
		//		gameManager.uiManager = uiManager;
		//		gameManager.startGame();
	}

	public override void _Process(double delta)
	{
	}
}
