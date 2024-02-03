using Godot;
public partial class Hanafuda : Node2D
{
	public UiManager uiManager;
	public override void _Ready()
	{
		uiManager = GetNode<UiManager>("UiManager");
	}

	public override void _Process(double delta)
	{
	}


}
