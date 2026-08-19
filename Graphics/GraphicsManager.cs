

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoLib.Graphics.Passes;
using MonoLib.IO;

namespace MonoLib.Graphics;

public static class GraphicsManager
{
    private static GraphicsDevice _graphicsDevice;
    private static SpriteBatch _spriteBatch;
    private static List<Pass> _passList;
    private static Dictionary<string, RenderTarget2D> _passOutputDict = new();

    private static Dictionary<string, RasterPass> _rasterPassDict = new();
    public static IReadOnlyDictionary<string, RasterPass> RasterPassDict => _rasterPassDict;
    public static readonly Dictionary<string, Material> MaterialDict = new();

    public static Color BarsColor = Color.Black;
    public static Color ClearColor = Color.CornflowerBlue;
    public static BlendState BlendState = BlendState.AlphaBlend;
    public static SamplerState SamplerState = SamplerState.PointClamp;

    public static void Load(GraphicsDevice graphicsDevice, List<Pass> passList)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _passList = passList;
        
        foreach (Pass pass in _passList)
        {
            pass.Load(_graphicsDevice, _spriteBatch, _passOutputDict);
            if (pass is RasterPass rasterPass)
                _rasterPassDict.Add(rasterPass.OutputKey, rasterPass);
        }
    }
    public static void Render()
    {
        foreach (Pass pass in _passList)
            pass.Render();
        
        Clear();
    }
    public static void Clear()
    {
        foreach (Pass pass in _passList)
            pass.Clear();

        _passOutputDict.Clear();
        RenderTargetCache.ReleaseUsed();
    }

    public static void Draw(string rasterPassKey, string layerKey, RenderObject renderObject)
    {
        if (!_rasterPassDict.TryGetValue(rasterPassKey, out RasterPass rasterPass))
            throw new KeyNotFoundException(nameof(rasterPassKey));
        
        if (rasterPass is null)
            throw new NullReferenceException(nameof(rasterPass));
        
        if (!rasterPass.LayerDict.TryGetValue(layerKey, out RasterPass.Layer layer))
            throw new KeyNotFoundException(nameof(layerKey));
        
        layer.Add(renderObject);
    }
    public static Camera GetCamera(string rasterPassKey)
    {
        if (!_rasterPassDict.TryGetValue(rasterPassKey, out RasterPass rasterPass))
            throw new KeyNotFoundException(nameof(rasterPassKey));
        
        return rasterPass.Camera;
    }

    public static Rectangle GetRenderDestination(int nativeWidth, int nativeHeight)
    {
        float scale = MathF.Min(
            (float)_graphicsDevice.Viewport.Width / nativeWidth, 
            (float)_graphicsDevice.Viewport.Height / nativeHeight);

        Rectangle dest = new(
            (int)((_graphicsDevice.Viewport.Width - nativeWidth * scale) * 0.5f),
            (int)((_graphicsDevice.Viewport.Height - nativeHeight * scale) * 0.5f),
            (int)(nativeWidth * scale),
            (int)(nativeHeight * scale));

        return dest;
    }

    
}