using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace MonoLib.Graphics.Passes;

public sealed class BlitPass : Pass
{
    [JsonPropertyName("input")]
    public required string InputKey { get; init; }
    [JsonPropertyName("width")]
    public required int TargetWidth { get; init; }
    [JsonPropertyName("height")]
    public required int TargetHeight  { get; init; }
    [JsonPropertyName("source_width")]
    public required int SourceWidth { get; init; }
    [JsonPropertyName("source_height")]
    public required int SourceHeight  { get; init; }
    [JsonPropertyName("source_x")]
    public required int SourceX { get; init; }
    [JsonPropertyName("source_y")]
    public required int SourceY { get; init; }
    [JsonPropertyName("dest_width")]
    public required int DestWidth { get; init; }
    [JsonPropertyName("dest_height")]
    public required int DestHeight  { get; init; }
    [JsonPropertyName("dest_x")]
    public required int DestX { get; init; }
    [JsonPropertyName("dest_y")]
    public required int DestY { get; init; }
    [JsonPropertyName("output")]
    public required string OutputKey { get; init; }

    public override void Load(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Dictionary<string, RenderTarget2D> passOutputDict)
    {
        if (TargetWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(TargetWidth));
        if (TargetHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(TargetHeight));
        if (SourceWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(SourceWidth));
        if (SourceHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(SourceHeight));
        if (DestWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(DestWidth));
        if (DestHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(DestHeight));
        
        base.Load(graphicsDevice, spriteBatch, passOutputDict);
    }
    public override void Render()
    {
        Rectangle sourceRect = new Rectangle(SourceX, SourceY, SourceWidth, SourceHeight);
        Rectangle destRect = new Rectangle(DestX, DestY, DestWidth, DestHeight);

        RenderTarget2D sourceRenderTarget = _passOutputDict[InputKey];
        RenderTarget2D destinationRenderTarget = RenderTargetCache.Get(TargetWidth, TargetHeight);

        _graphicsDevice.SetRenderTarget(destinationRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(blendState: GraphicsManager.BlendState, samplerState: GraphicsManager.SamplerState);
        _spriteBatch.Draw(sourceRenderTarget, destRect, sourceRect, Color.White);
        _spriteBatch.End();

        _passOutputDict.Add(OutputKey, destinationRenderTarget);
    }
    public override void Clear() {}
}