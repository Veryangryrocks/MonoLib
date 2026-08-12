using System.Collections.Immutable;

namespace MonoLib.Graphics;

public sealed class Animation : IEquatable<Animation>
{
    private readonly ImmutableArray<Sprite> _spriteArray;
    public int Length => _spriteArray.Length;
    private static Dictionary<ImmutableArray<Sprite>, Animation> _animationCache = new();

    private Animation(Sprite[] spriteArray)
    {
        if (spriteArray.Length == 0)
            throw new ArgumentException($"{nameof(spriteArray)} must not be empty.");
        _spriteArray = spriteArray.ToImmutableArray();
    }

    public static Animation Get(Sprite[] spriteArray)
    {
        if (_animationCache.TryGetValue(spriteArray.ToImmutableArray(), out Animation cachedAnimation))
            return cachedAnimation;
        
        Animation animation = new Animation(spriteArray);
        _animationCache.Add(spriteArray.ToImmutableArray(), animation);
        return animation;
    }

    public bool TryGetSprite(int index, out Sprite sprite)
    {
        sprite = null;

        if (index < 0 || index >= Length)
            return false;
        
        sprite = _spriteArray[index];
        return true;
    }
    public Sprite GetSprite(int index)
    {
        if (index < 0 || index >= Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        return _spriteArray[index];
    }
    
    public int GetIndex(int elapsedFrames, int duration) => elapsedFrames / (duration / Length) % Length;
    public static int GetIndex(int elapsedFrames, int duration, int length) => elapsedFrames / (duration / length) % length;
    public Sprite GetSprite(int elapsedFrames, int duration) => GetSprite(GetIndex(elapsedFrames, duration));

    // IEquatable
    public bool Equals(Animation other)
    {
        if (other is null) 
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return _spriteArray == other._spriteArray;
    }
    public override bool Equals(object obj) => Equals(obj as Animation);
    public override int GetHashCode() => _spriteArray.GetHashCode();
    public static bool operator ==(Animation left, Animation right)
    {
        if (left is null) 
            return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(Animation left, Animation right) => !(left == right);
}