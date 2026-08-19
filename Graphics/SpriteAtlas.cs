using System.Collections;
using System.Collections.Immutable;
using MonoLib.Util;

namespace MonoLib.Graphics;

public sealed class SpriteAtlas
{
    public readonly Sprite Sprite;
    public readonly Dictionary<string, SpriteRegion> SpriteRegions;
    public int Count => SpriteRegions.Count;
    public SpriteAtlas(Sprite sprite, Dictionary<string, SpriteRegion> spriteRegions)
    {
        Sprite = sprite;
        SpriteRegions = spriteRegions;
    }
    public bool TryGetSprite(string key, out Sprite sprite)
    {
        sprite = default;

        if (!SpriteRegions.TryGetValue(key, out SpriteRegion spriteRegion))
            return false;
            
        sprite = Sprite.Resize(spriteRegion);
        return true;
    }
    public Sprite GetSprite(string key)
    {
        SpriteRegion spriteRegion = SpriteRegions[key];
        return Sprite.Resize(spriteRegion);
    }
    public bool TryGetSprite(int x, int y, out Sprite sprite)
    {
        sprite = default;

        foreach (SpriteRegion spriteRegion in SpriteRegions.Values)
        {
            if (spriteRegion.Intersects(x, y))
            {
                sprite = Sprite.Resize(spriteRegion);
                return true;
            }
        }
        return false;
    }
    public Sprite GetSprite(int x, int y)
    {
        foreach (SpriteRegion spriteRegion in SpriteRegions.Values)
            if (spriteRegion.Intersects(x, y))
                return Sprite.Resize(spriteRegion);

        throw new ArgumentException($"No sprite was found at ({x}, {y}).");
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
    public override string ToString() => "{Sprite:" + Sprite + " SpriteRegions:" + SpriteRegions + "}";
}