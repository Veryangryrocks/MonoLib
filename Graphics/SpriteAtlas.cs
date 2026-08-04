using System.Collections.Immutable;
using MonoGameLibrary.Graphics;

namespace MonoLib.Graphics;

public sealed class SpriteAtlas
{
    public readonly Sprite Sprite;
    private readonly ImmutableDictionary<string, SpriteRegion> _spriteRegionDict;
    private static Dictionary<Sprite, SpriteAtlas> _spriteAtlasCache = new();
    public SpriteAtlas(Sprite sprite, Dictionary<string, SpriteRegion> spriteRegionDict)
    {
        Sprite = sprite;
        _spriteRegionDict = spriteRegionDict.ToImmutableDictionary();
    }

    public static void Add(Sprite sprite, Dictionary<string, SpriteRegion> spriteRegionDict)
    {
        _spriteAtlasCache.Add(sprite, new SpriteAtlas(sprite, spriteRegionDict));
    }

    public static bool TryGet(Sprite sprite, out SpriteAtlas spriteAtlas)
    {
        spriteAtlas = default;

        if (_spriteAtlasCache.TryGetValue(sprite, out SpriteAtlas cachedSpriteAtlas))
        {
            spriteAtlas = cachedSpriteAtlas;
            return true;
        }
        return false;
    }
    public static SpriteAtlas Get(Sprite sprite)
    {
        if (!_spriteAtlasCache.ContainsKey(sprite))
            throw new KeyNotFoundException(nameof(sprite));
        
        return _spriteAtlasCache[sprite];
    }

    public bool TryGetSprite(string key, out Sprite sprite)
    {
        sprite = default;

        if (!_spriteRegionDict.TryGetValue(key, out SpriteRegion spriteRegion))
            return false;
            
        sprite = Sprite.Resize(spriteRegion);
        return true;
    }
    public Sprite GetSprite(string key)
    {
        if (!_spriteRegionDict.ContainsKey(key))
            throw new KeyNotFoundException(nameof(key));
        
        SpriteRegion spriteRegion = _spriteRegionDict[key];
        return Sprite.Resize(spriteRegion);
    }

    public override string ToString() => "{Sprite:" + Sprite + " SpriteRegionDict:" + _spriteRegionDict + "}";
}