using System.Drawing;
using Godot;
public partial class MinosGDSprite : Sprite2D
{
	public Vector2 size;
	Area2D area2D;
	CollisionShape2D collisionShape2D;
	Button button;

	[Signal]
	public delegate void pressedEventHandler();
	[Signal]
	public delegate void mouseEnteredEventHandler();
	[Signal]
	public delegate void mouseExitedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area2D = GetNode<Area2D>("Area2D");
		collisionShape2D = area2D.GetNode<CollisionShape2D>("CollisionShape2D");

		button = GetNode<Button>("Button");
		if (this.Texture != null)
		{
			if (this.RegionRect != new Rect2(0, 0, 0, 0))
			{
				this.size = new Vector2(RegionRect.Size.X, RegionRect.Size.Y);
			}
			else
			{
				this.size = this.GetRect().Size;
			}
			this.Offset = Offset with
			{
				X = this.size.X / 2,
				Y = this.size.Y / 2
			};

			//button.Position = this.Position;
			//button.Size = this.size;
			collisionShape2D.Position = this.Offset;
			(collisionShape2D.Shape as RectangleShape2D).Size = this.size;
			area2D.MouseEntered += () => EmitSignal(SignalName.mouseEntered);
			area2D.MouseExited += () => EmitSignal(SignalName.mouseExited);
			button.Pressed += () => EmitSignal(SignalName.pressed);
		}

	}


	/// <summary>
	/// In Abhängigkeit von der RegionGröße
	/// </summary>
	public void moveRegionRectSteps(float x, float y)
	{
		moveRegionRect(x * RegionRect.Size.X, y * RegionRect.Size.Y);
	}

	/// <summary>
	/// In absoluten Schritten
	/// </summary>
	public void moveRegionRect(float x, float y)
	{
		var newPos = RegionRect.Position + new Vector2(x, y);
		this.RegionRect = MinosUtils.createRec2(newPos.X, newPos.Y, RegionRect.Size);
	}

	public void setRegionPos(float x, float y)
	{
		this.RegionRect = MinosUtils.createRec2(x, y, RegionRect.Size);
	}
}
