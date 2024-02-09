using Godot;

public partial class UiRoot : Control
{
	TextureButton sumBtn;
	CardSummary cardSummary;
	public override void _Ready()
	{
		sumBtn = GetNode<TextureButton>("SumBtn");
		cardSummary = GetNode<CardSummary>("CardSummary");
		this.Size = GetViewportRect().Size;
		sumBtn.Pressed += () => { cardSummary.Visible = true; cardSummary.drawDeck(); };
	}

	public override void _Process(double delta)
	{
	}
}
