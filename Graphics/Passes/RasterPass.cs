using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace MonoLib.Graphics.Passes;

public sealed class RasterPass : Pass
{
    [JsonPropertyName("layers")]
    public required string[] LayerKeyArray { get; init; }
    [JsonPropertyName("width")]
    public required int Width { get; init; }
    [JsonPropertyName("height")]
    public required int Height { get; init; }
    [JsonPropertyName("output")]
    public required string OutputKey { get; init; }

    private readonly Dictionary<string, Layer> _layerDict = new();
    public IReadOnlyDictionary<string, Layer> LayerDict => _layerDict;
    public Camera Camera { get; private set; }

    public override void Load(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Dictionary<string, RenderTarget2D> passOutputDict)
    {
        if (LayerKeyArray.Length == 0)
            throw new ArgumentException($"{nameof(LayerKeyArray)} may not be empty.");
        if (Width <= 0)
            throw new ArgumentOutOfRangeException(nameof(Width));
        if (Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(Height));
        
        base.Load(graphicsDevice, spriteBatch, passOutputDict);

        foreach (string layerKey in LayerKeyArray)
            _layerDict.Add(layerKey, new Layer());
        
        Camera = new Camera(new Vector2(0, 0), new Vector2(Width / 2, Height / 2));
    }
    public override void Render()
    {
        RenderTarget2D renderTarget = RenderTargetCache.Get(Width, Height);

        _graphicsDevice.SetRenderTarget(renderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        if (Camera == null)
            throw new NullReferenceException(nameof(Camera));

        foreach (string layerKey in LayerKeyArray)
            _layerDict[layerKey].Render(Camera.GetViewMatrix());
          
        _passOutputDict.Add(OutputKey, renderTarget);
    }
    public override void Clear()
    {
        foreach (string key in LayerKeyArray)
                _layerDict[key].Clear();
    }

    public Point ScreenToRP(Point screenPosition)
    {
        Matrix inverseView = Matrix.Invert(Camera.GetViewMatrix());
        Vector2 rpPosition = Vector2.Transform(screenPosition.ToVector2(), inverseView);
        return rpPosition.ToPoint();
    }

    public sealed class Layer
    {
        private List<RenderObject> _renderObjectList = new();
        public void Render(Matrix transform)
        {
            BatchThenFlush(_renderObjectList.OfType<RenderSprite>().ToList(), transform);
        }
        public void Clear()
        {
            _renderObjectList.Clear();
        }
        public void Add(RenderObject renderObject)
        {
            _renderObjectList.Add(renderObject);
        }
    }

    private static void BatchThenFlush(List<RenderSprite> renderSpriteList, Matrix transform)
    {
        Flush(Batch(renderSpriteList), transform);
    }
    private static List<List<RenderSprite>> Batch(List<RenderSprite> renderSpriteList)
    {
        if (renderSpriteList.Count == 0)
            return new();

        renderSpriteList.Sort((a, b) => b.Depth.CompareTo(a.Depth));

        Material currentMaterial = renderSpriteList[0].Material;

        List<List<RenderSprite>> batchList = new List<List<RenderSprite>>();
        List<RenderSprite> batch = new();

        foreach (RenderSprite renderSprite in renderSpriteList)
        {
            Material material = renderSprite.Material;

            if (material != currentMaterial)
            {
                batchList.Add(batch);
                batch = new();

                currentMaterial = material;
            }

            batch.Add(renderSprite);
        }

        batchList.Add(batch);
        return batchList;
    }
    private static void Flush(List<List<RenderSprite>> batchList, Matrix transform)
    {
        foreach (List<RenderSprite> batch in batchList)
        {
            Material material = batch[0].Material;

            _spriteBatch.Begin(blendState: GraphicsManager.BlendState, samplerState: GraphicsManager.SamplerState, effect: material?.Effect, transformMatrix: transform);

            foreach (RenderSprite renderSprite in batch)
            {
                Rectangle destRect = renderSprite.DestRect;
                Rectangle sourceRect = renderSprite.Sprite.Region.Rect;

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (renderSprite.FlipX)
                    spriteEffects |= SpriteEffects.FlipHorizontally;
                if (renderSprite.FlipY)
                    spriteEffects |= SpriteEffects.FlipVertically;

                Vector2 origin = new Vector2(renderSprite.Origin.Item1, renderSprite.Origin.Item2);

                _spriteBatch.Draw(renderSprite.Sprite.Texture2D, destRect, sourceRect, renderSprite.Color, renderSprite.Rotation.Value, origin, spriteEffects, 0);
            }

            _spriteBatch.End();
        }
    }
}