using Godot;
public partial class CardScn : Node2D
{
	public Sprite2D sprite2D;
	Sprite2D selectTexture;
	Sprite2D hoverTexture;
	Sprite2D validTexture;
	Area2D area2D;
	public Card card;

	public bool isSelected = false;
	public bool isOpen = true;
	public bool allowHover = true;
	public bool allowSelectable = true;

	[Signal]
	public delegate void pressedEventHandler(CardScn card);

	Texture2D closedTex = ResourceLoader.Load<CompressedTexture2D>("res://assets/smallerCardBack.png");
	Texture2D openTex = ResourceLoader.Load<CompressedTexture2D>("res://assets/cards.png");

	public override void _Ready()
	{
		sprite2D = GetNode<Sprite2D>("Sprite2D");
		area2D = GetNode<Area2D>("Area2D");
		selectTexture = GetNode<Sprite2D>("SelectTexture");
		hoverTexture = GetNode<Sprite2D>("HoverTexture");
		validTexture = GetNode<Sprite2D>("ValidTexture");

		setCard(new Card(0, 0));
		setSelected(false);
		setHover(false);
		area2D.MouseEntered += () => { if (allowHover) { setHover(true); } };
		area2D.MouseExited += () => { if (allowSelectable) { setHover(false); } };
	}

	public void setCard(Card card)
	{
		this.card = card;
		setImage();
	}

	void setImage()
	{
		if (!this.isOpen || !card.isValid())
		{
			this.sprite2D.Texture = closedTex;
		}
		else
		{
			const float paddingX = 2;
			const float paddingY = 2;
			const int dayPerMonth = 4;
			int idx = (int)card.month * dayPerMonth + card.day;
			int row = (int)(idx / 10);
			int column = idx % 10;
			this.sprite2D.Texture = openTex;
			setRegionPos(0, 0);
			moveRegionRectSteps(column, row);
			moveRegionRect(paddingX * column, paddingY * row);

			//selectTexture.Position = sprite2D.Offset;
			//hoverTexture.Position = sprite2D.Offset;
			//validTexture.Position = sprite2D.Offset;
		}
	}

	public override void _Process(double delta)
	{
		if (this.isOpen && card.isValid())
		{
			this.sprite2D.Texture = openTex;
		}
		else
		{
			this.sprite2D.Texture = closedTex;
		}
		this.selectTexture.Visible = this.isSelected;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseButton)
		{
			var mouseEvent = @event as InputEventMouseButton;
			var pos = ToLocal(mouseEvent!.Position);
			//var rect = this.sprite2D.GetRect();
			//rect = new Rect2(rect.Position, rect.Size * sprite2D.Scale);
			if (mouseEvent.IsPressed() && sprite2D.GetScaledRect().HasPoint(pos))
			{
				EmitSignal(SignalName.pressed, this);
			}
		}
	}

	public void setSelected(bool val)
	{
		if (!allowSelectable) { return; }
		this.isSelected = val;
		selectTexture.Visible = val;
	}

	public void setHover(bool val)
	{
		if (!allowHover) { return; }
		hoverTexture.Visible = val;
	}

	public void setAllowInteraction(bool val)
	{
		setAllowHover(val);
		setAllowSelectable(val);
		if (!val) { setHover(false); setValid(false); setSelected(false); }
	}
	public void setAllowHover(bool val)
	{
		allowHover = val;
	}

	public void setAllowSelectable(bool val)
	{
		allowSelectable = val;
	}

	public void setValid(bool val)
	{
		this.validTexture.Visible = val;
	}
	public bool getValid()
	{
		return this.validTexture.Visible;
	}

	/// <summary>
	/// In Abhängigkeit von der RegionGröße
	/// </summary>
	public void moveRegionRectSteps(float x, float y)
	{
		moveRegionRect(x * sprite2D.RegionRect.Size.X, y * sprite2D.RegionRect.Size.Y);
	}

	/// <summary>
	/// In absoluten Schritten
	/// </summary>
	public void moveRegionRect(float x, float y)
	{
		var newPos = sprite2D.RegionRect.Position + new Vector2(x, y);
		this.sprite2D.RegionRect = MinosUtils.createRec2(newPos.X, newPos.Y, sprite2D.RegionRect.Size);
	}

	public void setRegionPos(float x, float y)
	{
		this.sprite2D.RegionRect = MinosUtils.createRec2(x, y, sprite2D.RegionRect.Size);
	}

	public Rect2 getBounds()
	{
		var bounds = sprite2D.GetRect();
		return new Rect2(bounds.Position, bounds.Size * sprite2D.Scale);
	}
}
