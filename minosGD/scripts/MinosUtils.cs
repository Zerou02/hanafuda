using Godot;
public partial class MinosUtils
{
    public static Vector2 createVec2I(int x, int y)
    {
        return new Vector2((float)x, (float)y);
    }

    public static Rect2 createRec2I(int x, int y, Vector2 size)
    {
        return createRec2((float)x, (float)y, size);
    }
    public static Rect2 createRec2(float x, float y, Vector2 size)
    {
        return new Rect2(x, y, size);
    }

    public static void removeChildren(Node node)
    {
        foreach (var x in node.GetChildren())
        {
            x.QueueFree();
        }
    }
}