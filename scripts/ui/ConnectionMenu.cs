using Godot;
public partial class ConnectionMenu : Control
{
	public LineEdit serverAddr;
	public LineEdit playerName;
	Button host;
	Button connect;

	[Signal]
	public delegate void connectPressedEventHandler(string addr, string name);
	[Signal]
	public delegate void hostPressedEventHandler(string addr, string name);
	public override void _Ready()
	{
		serverAddr = GetNode<LineEdit>("ServerAddr");
		playerName = GetNode<LineEdit>("PlayerName");
		host = GetNode<Button>("Host");
		connect = GetNode<Button>("Connect");
		var text = OS.GetEnvironment("USER");
		if (OS.GetName() == "Windows")
		{
			text = OS.GetEnvironment("%USERNAME%");
		}
		playerName.PlaceholderText = text;

		connect.Pressed += () => EmitSignal(SignalName.connectPressed, serverAddr.Text, playerName.Text);
		host.Pressed += () => EmitSignal(SignalName.hostPressed, serverAddr.Text, playerName.Text);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
