using Godot;
using System;

public partial class MouseRayCaster : RayCast2D
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		var mousePos = GetViewport().GetMousePosition();

		this.Position = Position with { X = mousePos.X, Y = mousePos.Y };
		//GD.Print(this.IsColliding());
		//GD.Print(this.GetColliderShape());
	}
}
