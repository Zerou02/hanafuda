using Godot;
using System;
using System.Collections.Generic;

public partial class SceneManager : Node2D
{
	List<PlayerScn> playerScns = new List<PlayerScn>();
	public override void _Ready()
	{
		playerScns.Add(GetNode<PlayerScn>("Player"));
		playerScns.Add(GetNode<PlayerScn>("Player2"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


}
