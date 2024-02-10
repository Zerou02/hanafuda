using System;
using Godot;

public enum CardType { Deck, Hand, Open, Table, None };
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
	public bool allowSelectable = true;
	bool inAnimation = false;
	bool shouldFree = false;

	[Signal]
	public delegate void pressedEventHandler(CardScn card);

	Texture2D closedTex = ResourceLoader.Load<CompressedTexture2D>("res://assets/smallerCardBack.png");
	Texture2D openTex = ResourceLoader.Load<CompressedTexture2D>("res://assets/cards.png");

	public CardType type = CardType.None;
	public InputManager inputManager;
	public override void _Ready()
	{
		sprite2D = GetNode<Sprite2D>("Sprite2D");
		area2D = GetNode<Area2D>("Area2D");
		selectTexture = GetNode<Sprite2D>("SelectTexture");
		hoverTexture = GetNode<Sprite2D>("HoverTexture");
		validTexture = GetNode<Sprite2D>("ValidTexture");
		inputManager = GetNode<InputManager>(Constants.inputManagerPath);
		setCard(new Card(0, 0));
		area2D.MouseEntered += () => inputManager.mouseEnteredOnCard(this);
		area2D.MouseExited += () => inputManager.mouseExitedOnCard(this);
	}

	public void setCard(Card card)
	{
		this.card = card;
		setImage();
	}

	void setImage()
	{
		this.Visible = true;
		if (!this.isOpen || !card.isValid())
		{
			this.sprite2D.Texture = closedTex;
			setRegionPos(0, 0);
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
		}
	}

	public override void _Process(double delta)
	{
		if (shouldFree && !inAnimation)
		{
			QueueFree();
		}
		setImage();
		this.selectTexture.Visible = this.isSelected;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseButton)
		{
			var mouseEvent = @event as InputEventMouseButton;
			var pos = ToLocal(mouseEvent!.Position);
			if (mouseEvent.IsPressed() && sprite2D.GetScaledRect().HasPoint(pos))
			{
				inputManager.cardPressed(this);
				EmitSignal(SignalName.pressed, this);
			}
		}
	}

	public void setSelected(bool val)
	{
		if (!allowSelectable && val) { return; }
		this.isSelected = val;
		selectTexture.Visible = val;
	}

	public void setHover(bool val)
	{
		hoverTexture.Visible = val;
	}

	public void setAllowInteraction(bool val)
	{
		setAllowSelectable(val);
		if (!val)
		{
			setHover(false);
			setValid(false);
			setSelected(false);
		}
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
		var bounds = sprite2D.GetScaledRect();
		return new Rect2(bounds.Position, bounds.Size);
	}

	public void setSize(Vector2 size)
	{
		var baseSize = this.sprite2D.GetScaledRect().Size;
		this.Scale = size / baseSize;
	}

	public void setQueueFree()
	{
		shouldFree = true;
	}

	public void animateMove(Vector2 to, float duration)
	{
		if (this.shouldFree) { return; }
		inAnimation = true;
		{
			var tween = GetTree().CreateTween().SetParallel();
			tween.TweenProperty(this, "position", to, duration);
			tween.Finished += () => { inAnimation = false; };
		}
	}
}