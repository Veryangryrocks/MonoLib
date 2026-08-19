using System.Collections;
using System.Collections.Immutable;
using MonoLib.Util;

namespace MonoLib.Graphics;

public sealed class Animation : IEquatable<Animation>, IEnumerable<Sprite>
{
    private readonly Sprite[] _sprites;
    public int Length => _sprites.Length;
    private static readonly Dictionary<Sprite[], Animation> _cache = new(new ArrayEqualityComparer<Sprite>());
    private Animation(Sprite[] sprites)
    {
        ArgumentNullException.ThrowIfNull(sprites);

        if (sprites.Length == 0)
            throw new ArgumentException(nameof(sprites));
        
        _sprites = sprites;
    }
    public static Animation Get(Sprite[] sprites)
    {
        if (_cache.TryGetValue(sprites, out Animation cachedAnimation))
            return cachedAnimation;
        
        Animation animation = new Animation(sprites);
        _cache[sprites] = animation;
        return animation;
    }
    public bool TryGetSprite(int index, out Sprite sprite)
    {
        sprite = default;

        if (index < 0 || index >= Length)
            return false;
        
        sprite = _sprites[index];
        return true;
    }
    public Sprite GetSprite(int index)
    {
        if (index < 0 || index >= Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        return _sprites[index];
    }
    public static int GetIndex(int elapsedFrames, int duration, int length)
    {
        if (elapsedFrames <= 0)
            return 0;
        return elapsedFrames / (duration / length) % length;
    }
    public int GetIndex(int elapsedFrames, int duration) => GetIndex(elapsedFrames, duration, Length);
    public Sprite GetSprite(int elapsedFrames, int duration) => GetSprite(GetIndex(elapsedFrames, duration));
    public override string ToString() => "{Sprites:" + _sprites + " }";

    // IEquatable
    public bool Equals(Animation other)
    {
        if (other is null) 
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return _sprites == other._sprites;
    }
    public override bool Equals(object obj) => Equals(obj as Animation);
    public override int GetHashCode() => _sprites.GetHashCode();
    public static bool operator ==(Animation left, Animation right)
    {
        if (left is null) 
            return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(Animation left, Animation right) => !(left == right);

    // IEnumerable
    public IEnumerator<Sprite> GetEnumerator() => ((IEnumerable<Sprite>)_sprites).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}