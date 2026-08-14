using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoLib.Graphics;

public readonly struct SpriteRegion : IEquatable<SpriteRegion>
{
    [JsonPropertyName("x")]
    public int X { get; init; }
    [JsonPropertyName("y")]
    public int Y { get; init; }
    [JsonPropertyName("width")]
    public int Width { get; init; }
    [JsonPropertyName("height")]
    public int Height { get; init; }
    public Rectangle Rect => new Rectangle(X, Y, Width, Height);
    public SpriteRegion(int x, int y, int width, int height)
    {
        if (x < 0)
            throw new ArgumentOutOfRangeException($"{nameof(x)} '{x}'");
        if (y < 0)
            throw new ArgumentOutOfRangeException($"{nameof(y)} '{y}'");

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
    public static SpriteRegion FromTexture2D(Texture2D texture2D) => new SpriteRegion(0, 0, texture2D.Width, texture2D.Height);
    public override string ToString() => "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + "}";

    // IEquatable
    public bool Equals(SpriteRegion other) => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    public override bool Equals(object other) => other is SpriteRegion && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);
    public static bool operator ==(SpriteRegion left, SpriteRegion right) => left.Equals(right);
    public static bool operator !=(SpriteRegion left, SpriteRegion right) => !(left == right);
}