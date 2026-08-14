using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoLib.Graphics.Passes;

public sealed class DuplicatePass : Pass
{
    [JsonPropertyName("input")]
    public required string InputKey { get; init; }
    [JsonPropertyName("outputs")]
    public required string[] OutputKeyArray { get; init; }

    public override void Load(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Dictionary<string, RenderTarget2D> passOutputDict)
    {
        if (OutputKeyArray.Length == 0)
            throw new ArgumentException($"{nameof(OutputKeyArray)} may not be empty.");
        
        base.Load(graphicsDevice, spriteBatch, passOutputDict);
    }
    public override void Render()
    {
        RenderTarget2D sourceRenderTarget = _passOutputDict[InputKey];

        foreach (string key in OutputKeyArray)
        {
            RenderTarget2D destinationRenderTarget = RenderTargetCache.Get(sourceRenderTarget.Width, sourceRenderTarget.Height);

            _graphicsDevice.SetRenderTarget(destinationRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(blendState: GraphicsManager.BlendState, samplerState: GraphicsManager.SamplerState);
            _spriteBatch.Draw(sourceRenderTarget, destinationRenderTarget.Bounds, Color.White);
            _spriteBatch.End();

            _passOutputDict.Add(key, destinationRenderTarget);
        }
    }
    public override void Clear() {}
}