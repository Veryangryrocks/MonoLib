using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using MonoLib.Graphics.Passes;

namespace MonoLib.Graphics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RasterPass), "raster")]
[JsonDerivedType(typeof(MaterialPass), "material")]
[JsonDerivedType(typeof(CompositePass), "composite")]
[JsonDerivedType(typeof(BlitPass), "blit")]
[JsonDerivedType(typeof(DuplicatePass), "duplicate")]
[JsonDerivedType(typeof(PresentPass), "present")]

public abstract class Pass
{
    protected static GraphicsDevice _graphicsDevice { get; private set; }
    protected static SpriteBatch _spriteBatch { get; private set; }
    protected static Dictionary<string, RenderTarget2D> _passOutputDict { get; private set; }
    public virtual void Load(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Dictionary<string, RenderTarget2D> passOutputDict)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _passOutputDict = passOutputDict;
    }
    public abstract void Render();
    public abstract void Clear();
}