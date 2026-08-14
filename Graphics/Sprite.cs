using Microsoft.Xna.Framework.Graphics;
using MonoLib.IO;

namespace MonoLib.Graphics;

public sealed class Sprite : IEquatable<Sprite>
{
    public readonly Texture2D Texture2D;
    public readonly string RelativePath;
    public readonly SpriteRegion Region;
    private static Dictionary<(string, SpriteRegion), Sprite> _cache = new();
    private Sprite(Texture2D texture2D, string relativePath, SpriteRegion? spriteRegion = null)
    {
        Texture2D = texture2D;
        RelativePath = relativePath;
        Region = spriteRegion ?? new SpriteRegion(0, 0, Texture2D.Width, Texture2D.Height);
    }
    private Sprite(string relativePath, SpriteRegion? spriteRegion = null) : this(ContentCache.Get<Texture2D>(relativePath), relativePath, spriteRegion) {}
    public static Sprite Get(Texture2D texture2D, string relativePath, SpriteRegion? spriteRegion = null)
    {
        SpriteRegion safeSpriteRegion = spriteRegion is null ? SpriteRegion.FromTexture2D(texture2D) : (SpriteRegion)spriteRegion;

        if (_cache.TryGetValue((relativePath, safeSpriteRegion), out Sprite cachedSprite))
            return cachedSprite;
        
        Sprite sprite = new Sprite(relativePath, safeSpriteRegion);
        _cache[(relativePath, safeSpriteRegion)] = sprite;
        return sprite;
    }
    public  static Sprite Get(string relativePath, SpriteRegion? spriteRegion = null) => Get(ContentCache.Get<Texture2D>(relativePath), relativePath, spriteRegion);
    public Sprite Resize(SpriteRegion spriteRegion) => Get(Texture2D, RelativePath, spriteRegion);
    public override string ToString() => "{RelativePath:" + RelativePath + " SpriteRegion:" + Region + "}";

    // IEquatable
    public bool Equals(Sprite other) => RelativePath == other.RelativePath && Region == other.Region;
    public override bool Equals(object other) => other is Sprite sprite && Equals(sprite);
    public override int GetHashCode() => HashCode.Combine(RelativePath, Region);
    public static bool operator ==(Sprite left, Sprite right) => left.Equals(right);
    public static bool operator !=(Sprite left, Sprite right) => !(left == right);
}