using Godot;

public partial class UiRoot : Control
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		this.Size = GetViewportRect().Size;
	}
}
