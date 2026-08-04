using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoLib.Graphics.Passes;

public sealed class PresentPass : Pass
{
    [JsonPropertyName("input")]
    public required string InputKey { get; init; }

    public override void Load(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Dictionary<string, RenderTarget2D> passOutputDict)
    {
        base.Load(graphicsDevice, spriteBatch, passOutputDict);
    }
    public override void Render()
    {
        RenderTarget2D renderTarget = _passOutputDict[InputKey];
        
        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.Clear(GraphicsManager.BarsColor);

        Rectangle dest = GraphicsManager.GetRenderDestination(renderTarget.Width, renderTarget.Height);
        
        Texture2D pixel = new Texture2D(_graphicsDevice, 1, 1);
        pixel.SetData([GraphicsManager.ClearColor]); 

        _spriteBatch.Begin(blendState: GraphicsManager.BlendState, samplerState: GraphicsManager.SamplerState);
        _spriteBatch.Draw(pixel, dest, Color.White);
        _spriteBatch.Draw(renderTarget, dest, Color.White);
        _spriteBatch.End();
    }
    public override void Clear() {}
}