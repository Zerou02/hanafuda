using Godot;
public partial class EmptyCard : Node2D
{
	MinosGDSprite sprite2d;
	[Signal]
	public delegate void pressedEventHandler();
	public override void _Ready()
	{
		sprite2d = GetNode<MinosGDSprite>("Sprite2D");
		sprite2d.pressed += () => EmitSignal(SignalName.pressed);
	}
	public override void _Process(double delta)
	{
	}
}
