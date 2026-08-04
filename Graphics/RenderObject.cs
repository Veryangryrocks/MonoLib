namespace MonoLib.Graphics;

public abstract class RenderObject
{
    public readonly float Depth;
    public RenderObject(float depth)
    {
        Depth = depth;
    }
    public override string ToString() => "{Depth:" + Depth + " }";
}