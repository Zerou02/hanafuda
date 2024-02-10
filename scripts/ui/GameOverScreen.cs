using System;
using System.Threading;
using Godot;

public partial class GameOverScreen : Node2D
{
	[Signal]
	public delegate void RematchClickedEventHandler();
	[Signal]
	public delegate void QuitClickedEventHandler();

	public Button textButton;
	public override void _Ready()
	{
		Visible = false;
	}
	public override void _Process(double delta)
	{
	}

	public void showScreen(bool isWinner, int points)
	{
		foreach (var x in GetChildren()) { x.QueueFree(); }
		this.Visible = true;
		var tex = new TextureRect();
		AddChild(tex);
		tex.Size = GetViewportRect().Size;
		var label = new Label();
		this.AddChild(label);
		var pointLabel = new Label();
		this.AddChild(pointLabel);
		pointLabel.Set("theme_override_font_sizes/font_size", 25);
		pointLabel.Text = "Punkte: " + points.ToString();
		label.Size = GetViewportRect().Size;
		label.Set("theme_override_font_sizes/font_size", 30);
		label.Modulate = Color.FromHtml("#ff0000ff");
		var rematchBtn = new Button();
		rematchBtn.Text = "Nur noch eine Runde!";
		textButton = rematchBtn;
		var quitBtn = new Button();
		quitBtn.Text = "Aufhören";
		rematchBtn.Position = new Vector2(370, 250);
		quitBtn.Position = new Vector2(370, 350);
		rematchBtn.Size = new Vector2(200, 50);
		quitBtn.Size = new Vector2(200, 50);
		rematchBtn.Pressed += () => EmitSignal(SignalName.RematchClicked);
		quitBtn.Pressed += () => EmitSignal(SignalName.QuitClicked);
		AddChild(rematchBtn);
		AddChild(quitBtn);
		label.Position = new Vector2(0, 200);
		pointLabel.Size = label.Size;
		pointLabel.Position = new Vector2(0, 150);
		pointLabel.HorizontalAlignment = HorizontalAlignment.Center;
		pointLabel.Modulate = Color.FromHtml("#000000ff");
		label.HorizontalAlignment = HorizontalAlignment.Center;
		if (isWinner)
		{
			tex.Texture = ResourceLoader.Load<CompressedTexture2D>("res://assets/double_moons.png");
			label.Text = "Du hast gewonnen!";
		}
		else
		{
			tex.Texture = ResourceLoader.Load<CompressedTexture2D>("res://assets/sinking_ship.png");
			label.Text = "Du hast verloren!";
		}
	}
}
