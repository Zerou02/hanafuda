using System.Collections.Generic;
using Godot;
public partial class KoiKoiSelection : Control
{
	public Button koiKoiButton;
	public Button stopButton;

	[Signal]
	public delegate void koiKoiPressedEventHandler();
	[Signal]
	public delegate void stopPressedEventHandler();

	List<Node> children = new List<Node>();
	PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
	public override void _Ready()
	{
		koiKoiButton = GetNode<Button>("KoiKoiBtn");
		stopButton = GetNode<Button>("StopBtn");
		koiKoiButton.Pressed += () => { EmitSignal(SignalName.koiKoiPressed); this.Visible = false; };
		stopButton.Pressed += () => { EmitSignal(SignalName.stopPressed); this.Visible = false; };
		var example = new Dictionary<Sets, List<Card>>() { { Sets.Tsukimi, new List<Card>() { new Card(2, 0), new Card(2, 2) } }, { Sets.Hanami, new List<Card>() { new Card(3, 0), new Card(3, 2) } }, { Sets.Plain, new List<Card>() { new Card(4, 0), new Card(5, 0) } } };
		setCards(example, 1);
	}

	public override void _Process(double delta)
	{
	}

	public void setCards(Dictionary<Sets, List<Card>> cards, int amountKoiKois)
	{
		var yCount = 0;
		var paddingY = 10;
		var length = this.Size.X * 0.6f;
		var textStart = this.Size.X * 0.8f;
		var pointStart = this.Size.X * 0.9f;
		var paddingTextY = 10;
		var sizeY = 50;
		var startX = this.Size.X * 0.1f;
		var pointMap = GameManager.calculatePointsArr(cards, amountKoiKois);
		this.Visible = true;
		foreach (var x in this.children)
		{
			x.QueueFree();
		}
		children = new List<Node>();

		foreach (var x in cards)
		{
			yCount++;
			var row = new List<CardScn>();
			var label = new Label();
			var pointLabel = new Label();
			AddChild(label);
			AddChild(pointLabel);
			this.children.Add(pointLabel);
			this.children.Add(label);
			label.Text = Localization.germanSetNames[(int)x.Key];
			pointLabel.Text = pointMap[(int)x.Key].ToString();
			foreach (var y in x.Value)
			{
				var scn = cardScn.Instantiate<CardScn>();
				row.Add(scn);
				this.children.Add(scn);
				this.AddChild(scn);
				scn.setCard(y);
				Flexbox.alignLeft(new Rect2(startX, yCount * sizeY + paddingY, length, sizeY), row);
			}
			label.Position = new Vector2(textStart, yCount * sizeY + paddingY);
			pointLabel.Position = new Vector2(pointStart, yCount * sizeY + 2.5f * paddingY);
		}
		yCount += 2;
		var totalPoints = new Label();
		totalPoints.Text = "Gesamtpunktzahl: " + GameManager.calculateTotalPoints(cards, amountKoiKois).ToString();
		AddChild(totalPoints);
		this.children.Add(totalPoints);
		totalPoints.Position = new Vector2(sizeY, yCount * sizeY);
	}
}
