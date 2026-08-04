using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace MonoLib.Graphics.Passes;

public sealed class MaterialPass : Pass
{
    [JsonPropertyName("input")]
    public required string InputKey { get; init; }

    [JsonPropertyName("materials")]
    public required string[] MaterialKeyArray { get; init; }
    [JsonPropertyName("output")]
    public required string OutputKey { get; init; }

    public override void Load(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Dictionary<string, RenderTarget2D> passOutputDict)
    {
        if (MaterialKeyArray.Length == 0)
            throw new ArgumentException($"{nameof(MaterialKeyArray)} may not be empty.");
        
        base.Load(graphicsDevice, spriteBatch, passOutputDict);
    }
    public override void Render()
    {
        RenderTarget2D sourceRenderTarget = _passOutputDict[InputKey];
        RenderTarget2D destinationRenderTarget = RenderTargetCache.Get(sourceRenderTarget.Width, sourceRenderTarget.Height);

        _graphicsDevice.SetRenderTarget(destinationRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        foreach (string key in MaterialKeyArray)
        {
            Material material = GraphicsManager.MaterialDict[key];
            
            if (material == null)
                throw new NullReferenceException(nameof(material));

            _graphicsDevice.SetRenderTarget(destinationRenderTarget);

            _spriteBatch.Begin(blendState: GraphicsManager.BlendState, samplerState: GraphicsManager.SamplerState, effect: material.Effect);
            _spriteBatch.Draw(sourceRenderTarget, destinationRenderTarget.Bounds, Color.White);
            _spriteBatch.End();

            (sourceRenderTarget, destinationRenderTarget) = (destinationRenderTarget, sourceRenderTarget);
        }

        _passOutputDict.Add(OutputKey, sourceRenderTarget);
    }
    public override void Clear() {}
}