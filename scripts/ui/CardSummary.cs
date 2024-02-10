using System;
using System.Collections.Generic;
using Godot;

public partial class CardSummary : Node2D
{
	PackedScene cardScn = GD.Load<PackedScene>("scenes/Card.tscn");
	AnimationManager animationManager;
	public override void _Ready()
	{
		animationManager = GetNode<AnimationManager>(Constants.animationManagerPath);
	}

	public override void _Process(double delta)
	{
	}

	enum Mode { Deck, Combinations }

	void drawTopBar(Mode mode)
	{
		var bg = new TextureRect();
		bg.SetSize(GetViewportRect().Size);
		bg.Position = new Vector2(0, 0);
		bg.Texture = ResourceLoader.Load<CompressedTexture2D>("res://assets/mountains.png");

		bg.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		this.AddChild(bg);
		var combBtn = new Button();
		var deckBtn = new Button();
		var escapeButton = new Button();
		combBtn.Text = "Kombinationen";
		deckBtn.Text = "Deck";
		escapeButton.Text = "X";
		this.AddChild(combBtn);
		this.AddChild(deckBtn);
		this.AddChild(escapeButton);
		combBtn.Position = new Vector2(300, 0);
		deckBtn.Position = new Vector2(450, 0);
		escapeButton.Position = new Vector2(1920 / 2 - escapeButton.Size.X, 0);
		escapeButton.Modulate = Color.FromHtml("#ff0000ff");
		if (mode == Mode.Deck)
		{
			deckBtn.Modulate = Color.FromHtml("#00ff00ff");
		}
		else
		{
			combBtn.Modulate = Color.FromHtml("#00ff00ff");
		}
		deckBtn.Pressed += () => drawDeck();
		combBtn.Pressed += () => drawSets();
		escapeButton.Pressed += () => this.Visible = false;
	}

	public void drawDeck()
	{
		foreach (var x in GetChildren()) { x.QueueFree(); }
		drawTopBar(Mode.Deck);
		var paddingY = 30;
		var columns = 3;
		var rowWidth = 4 * Constants.cardWidth;
		var paddingTop = 50;
		var paddingX = 100;
		for (int i = 0; i < 12; i++)
		{
			var row = new List<CardScn>();
			for (int j = 0; j < 4; j++)
			{
				var c = new Card(i, j);
				var card = cardScn.Instantiate<CardScn>();
				row.Add(card);
				AddChild(card);
				card.Position = new Vector2(-Constants.cardHeight, -Constants.cardWidth);
				card.setCard(c);
			}
			Flexbox.alignLeftAnimated(new Rect2(i % columns * (rowWidth + paddingX), (Constants.cardHeight + paddingY) * (i / columns) + paddingTop, rowWidth, Constants.cardHeight), row, animationManager);
			var label = new Label();
			label.Text = Localization.germanMonthNames[i];
			label.Position = new Vector2(i % columns * ((rowWidth + paddingX)), (Constants.cardHeight + paddingY) * (i / columns) + Constants.cardHeight + paddingTop);
			label.Size = new Vector2(rowWidth, Constants.cardHeight);
			label.HorizontalAlignment = HorizontalAlignment.Center;
			AddChild(label);
		}
	}

	void drawSets()
	{
		foreach (var x in GetChildren()) { x.QueueFree(); }
		drawTopBar(Mode.Combinations);
		var deck = Deck.OrderedCards();
		var sets = new bool[12];
		Array.Fill(sets, true);
		var cards = GameManager.calcCardsToSet(deck, sets);
		var amountCard = new int[] { 10, 5, 5, 3, 3, 3, 2, 2, 3, 4, 4, 5 };
		var text = new string[]{
			"10 Ebenen + 1 Punkt für jede zusätzliche",
			"5 Schriftrollen Karten + 1 Punkt für jede zusätzliche",
			"5 Tiere + 1 Punkt für jedes zusätzliche",
			"Alle 3 blauen Schriftrollen",
			"Alle 3 Schriftrollen mit Text",
			"Eber,Reh und Schmetterlinge",
			"Mond und Becher",
			"Kirschblüten und Becher",
			"3 Lichter (ohne den Regenmann)",
			"4 Lichter mit dem Regenmann",
			"4 Lichter (ohne den Regenmann)",
			"Alle 5 Lichter"
		};
		var points = new int[] { 1, 1, 1, 6, 6, 6, 5, 5, 6, 8, 9, 12 };
		var i = -1;

		var paddingY = 50;
		var columns = 3;
		var rowWidth = 200;
		var paddingTop = 50;
		var paddingX = 120;
		foreach (var x in cards)
		{
			i += 1;
			var row = new List<CardScn>();
			var count = -1;
			foreach (var y in x.Value)
			{
				count += 1;
				if (count == amountCard[i]) { break; }
				var z = cardScn.Instantiate<CardScn>();
				AddChild(z);
				if ((x.Key == Sets.Plain && count == 9) || (x.Key == Sets.Animals && count == 4))
				{
					z.setCard(new Card(8, 3));
				}
				else
				{
					z.setCard(y);
				}
				row.Add(z);
				Flexbox.alignLeft(new Rect2(i % columns * (rowWidth + paddingX), (i / columns) * (Constants.cardHeight + paddingY) + paddingTop, rowWidth, Constants.cardHeight), row);
			}
			var label = new RichTextLabel();
			label.Text = text[i];
			label.Position = new Vector2(i % columns * (rowWidth + paddingX), (Constants.cardHeight + paddingY) * (i / columns) + Constants.cardHeight + paddingTop);
			label.Size = new Vector2(rowWidth, 2 * Constants.cardHeight);
			AddChild(label);
			var setNameLabel = new Label();
			setNameLabel.Text = Localization.germanSetNames[(int)x.Key];
			setNameLabel.Modulate = Color.FromHtml("#ffcc00ff");
			setNameLabel.Position = new Vector2(i % columns * (rowWidth + paddingX) + Math.Min(5, row.Count) * Constants.cardWidth, (i / columns) * (Constants.cardHeight + paddingY) + paddingTop);
			var pointLabel = new Label();
			pointLabel.Text = "Punkte:" + points[i].ToString();
			pointLabel.Modulate = Color.FromHtml("#ffcc00ff");
			pointLabel.Position = new Vector2(i % columns * (rowWidth + paddingX) + Math.Min(5, row.Count) * Constants.cardWidth, (i / columns) * (Constants.cardHeight + paddingY) + 20 + paddingTop);
			AddChild(setNameLabel);
			AddChild(pointLabel);
		}

		var sake = cardScn.Instantiate<CardScn>();
		this.AddChild(sake);
		sake.setCard(new Card(8, 3));
		sake.Position = new Vector2(100, 470);
		var sakeLabel = new RichTextLabel();
		this.AddChild(sakeLabel);
		sakeLabel.Position = new Vector2(100 + Constants.cardWidth + 10, 470);
		sakeLabel.Size = new Vector2(300, 200);
		sakeLabel.Text = "Der Becher ist der Joker. Er kann mit Tieren, Ebenen, dem Mond und Kirschblüten kombiniert werden";

		var rainMan = cardScn.Instantiate<CardScn>();
		this.AddChild(rainMan);
		rainMan.setCard(new Card(10, 3));
		rainMan.Position = new Vector2(500, 470);
		var rainLabel = new RichTextLabel();
		this.AddChild(rainLabel);
		rainLabel.Position = new Vector2(500 + Constants.cardWidth + 10, 470);
		rainLabel.Size = new Vector2(300, 200);
		rainLabel.Text = "Der Regenmann ist ein Licht, kann aber nur mit mindestens 3 anderen Lichtern kombiniert werden";
	}
}
