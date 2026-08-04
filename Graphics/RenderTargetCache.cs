using Microsoft.Xna.Framework.Graphics;

namespace MonoLib.Graphics;

public static class RenderTargetCache
{
    private static GraphicsDevice _graphicsDevice;
    private static Dictionary<(int, int), Stack<RenderTarget2D>> _renderTargetPool = new();
    private static List<RenderTarget2D> _usedRenderTargetList = new();

    public static void Load(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public static RenderTarget2D Get(int width, int height)
    {
        if (_graphicsDevice == null)
            throw new NullReferenceException("RenderTargetCache has not been loaded.");

        var key = (width, height);

        if (!_renderTargetPool.ContainsKey(key))
            _renderTargetPool[key] = new();

        RenderTarget2D renderTarget;
        if (_renderTargetPool[key].Count > 0)
            renderTarget = _renderTargetPool[key].Pop();
        else
            renderTarget = new RenderTarget2D(_graphicsDevice, width, height);

        _usedRenderTargetList.Add(renderTarget);
        return renderTarget;
    }

    private static void Release(RenderTarget2D renderTarget)
    {
        var key = (renderTarget.Width, renderTarget.Height);

        if (!_renderTargetPool.ContainsKey(key))
            _renderTargetPool[key] = new();

        _renderTargetPool[key].Push(renderTarget);
    }

    public static void ReleaseUsed()
    {
        foreach (RenderTarget2D renderTarget in _usedRenderTargetList)
            Release(renderTarget);
        
        _usedRenderTargetList.Clear();
    }
}