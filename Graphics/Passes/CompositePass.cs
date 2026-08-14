using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoLib.Graphics.Passes;

public sealed class CompositePass : Pass
{
    [JsonPropertyName("input_back")]
    public required string InputKeyBack{ get; init; }
    [JsonPropertyName("input_front")]
    public required string InputKeyFront { get; init; }
    [JsonPropertyName("output")]
    public required string OutputKey { get; init; }

    public override void Load(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Dictionary<string, RenderTarget2D> passOutputDict)
    {
        base.Load(graphicsDevice, spriteBatch, passOutputDict);
    }
    public override void Render()
    {
        RenderTarget2D backRenderTarget = _passOutputDict[InputKeyBack];
        RenderTarget2D frontRenderTarget = _passOutputDict[InputKeyFront];

        if (backRenderTarget.Width != frontRenderTarget.Width || backRenderTarget.Height != frontRenderTarget.Height)
            throw new ArgumentOutOfRangeException("Input dimensions did not match.");

        RenderTarget2D destinationRenderTarget = RenderTargetCache.Get(backRenderTarget.Width, backRenderTarget.Height);

        _graphicsDevice.SetRenderTarget(destinationRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(blendState: GraphicsManager.BlendState, samplerState: GraphicsManager.SamplerState);
        _spriteBatch.Draw(backRenderTarget, destinationRenderTarget.Bounds, Color.White);
        _spriteBatch.Draw(frontRenderTarget, destinationRenderTarget.Bounds, Color.White);
        _spriteBatch.End();

        _passOutputDict.Add(OutputKey, destinationRenderTarget);
    }
    public override void Clear() {}
}