using System.Numerics;
using Microsoft.Xna.Framework;

namespace MonoLib.Util;

public readonly struct Radians : IEquatable<Radians>, IAdditionOperators<Radians, Radians, Radians>, ISubtractionOperators<Radians, Radians, Radians>, IMultiplyOperators<Radians, Radians, Radians>, IDivisionOperators<Radians, Radians, Radians>, IComparisonOperators<Radians, Radians, bool>
{
    public readonly float Value;
    public Radians(float value)
    {
        Value = Normalize(value);
    }

    private static float Normalize(float value)
    {
        value %= MathHelper.TwoPi;

        if (value < 0)
            value += MathHelper.TwoPi;
        
        return value;
    }

    public Degrees ToDegrees() => new Degrees(Value * 180 / MathF.PI);

    public override string ToString() => $"{Value} radians";

    // IEquatable
    public bool Equals(Radians other) => Value == other.Value;
    public override bool Equals(object obj) => obj is Radians other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Value);
    public static bool operator ==(Radians left, Radians right) => left.Value == right.Value;
    public static bool operator !=(Radians left, Radians right) => !(left.Value == right.Value);

    // IAdditionOperators
    public static Radians operator +(Radians a, Radians b) => new Radians(a.Value + b.Value);

    // ISubtractionOperators
    public static Radians operator -(Radians a, Radians b) => new Radians(a.Value - b.Value);

    // IMultiplyOperators
    public static Radians operator *(Radians a, Radians b) => new Radians(a.Value * b.Value);

    // IDivisionOperators
    public static Radians operator /(Radians a, Radians b) => new Radians(a.Value / b.Value);

    // IComparisonOperators
    public static bool operator >(Radians a, Radians b) => a.Value > b.Value;
    public static bool operator >=(Radians a, Radians b) => a.Value >= b.Value;
    public static bool operator <(Radians a, Radians b) => a.Value < b.Value;
    public static bool operator <=(Radians a, Radians b) => a.Value <= b.Value;
}