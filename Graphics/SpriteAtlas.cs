using System.Collections.Immutable;
using MonoLib.Util;

namespace MonoLib.Graphics;

public sealed class SpriteAtlas
{
    public readonly Sprite Sprite;
    private readonly Dictionary<string, SpriteRegion> _spriteRegions;
    public SpriteAtlas(Sprite sprite, Dictionary<string, SpriteRegion> spriteRegions)
    {
        Sprite = sprite;
        _spriteRegions = spriteRegions;
    }
    public bool TryGetSprite(string key, out Sprite sprite)
    {
        sprite = default;

        if (!_spriteRegions.TryGetValue(key, out SpriteRegion spriteRegion))
            return false;
            
        sprite = Sprite.Resize(spriteRegion);
        return true;
    }
    public Sprite GetSprite(string key)
    {
        if (!_spriteRegions.ContainsKey(key))
            throw new KeyNotFoundException(nameof(key));
        
        SpriteRegion spriteRegion = _spriteRegions[key];
        return Sprite.Resize(spriteRegion);
    }
    public bool TryGetAnimation(string key, out Animation animation, int start = 0, int? end = null, string suffix = "_*", char wildcard = '*')
    {   
        animation = default;

        if (end.HasValue && end <= start)
            return false;

        List<Sprite> sprites = new();

        for (int i = start; ; i++)
        {
            if (end.HasValue && i > end.Value)
                break;
            
            string newSuffix = suffix.Replace($"{wildcard}", $"{i}");

            if (!TryGetSprite($"{key}{newSuffix}", out Sprite sprite))
                break;
            
            sprites.Add(sprite);
        }
        
        if (sprites.Count == 0)
            return false;

        animation = Animation.Get(sprites.ToArray());
        return true;
    }
    public Animation GetAnimation(string key, int start = 0, int? end = null, string suffix = "_*", char wildcard = '*')
    {
        if (end.HasValue && end <= start)
            throw new ArgumentOutOfRangeException(nameof(end));

        List<Sprite> sprites = new();

        for (int i = start; ; i++)
        {
            if (end.HasValue && i > end.Value)
                break;
            
            string newSuffix = suffix.Replace($"{wildcard}", $"{i}");

            if (!TryGetSprite($"{key}{newSuffix}", out Sprite sprite))
                break;
            
            sprites.Add(sprite);
        }
        
        if (sprites.Count == 0)
            throw new InvalidOperationException("No sprites were found.");

        return Animation.Get(sprites.ToArray());
    }
    public override string ToString() => "{Sprite:" + Sprite + " SpriteRegions:" + _spriteRegions + "}";
}