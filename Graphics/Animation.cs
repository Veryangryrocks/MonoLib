using System.Collections.Immutable;

namespace MonoLib.Graphics;

public sealed class Animation : IEquatable<Animation>
{
    private readonly Sprite[] _sprites;
    public int Length => _sprites.Length;
    public static readonly Dictionary<string, Animation> Cache = new();
    public Animation(Sprite[] sprites)
    {
        ArgumentNullException.ThrowIfNull(sprites);
        if (sprites.Length == 0)
            throw new ArgumentException(nameof(sprites));
        
        _sprites = sprites;
    }
    public bool TryGetSprite(int index, out Sprite sprite)
    {
        sprite = null;

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
  
    public int GetIndex(int elapsedFrames, int duration) => elapsedFrames / (duration / Length) % Length;
    public static int GetIndex(int elapsedFrames, int duration, int length) => elapsedFrames / (duration / length) % length;
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
}