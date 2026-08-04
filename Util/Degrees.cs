using System.Numerics;

namespace MonoLib.Util;

public readonly struct Degrees : IEquatable<Degrees>, IAdditionOperators<Degrees, Degrees, Degrees>, ISubtractionOperators<Degrees, Degrees, Degrees>, IMultiplyOperators<Degrees, Degrees, Degrees>, IDivisionOperators<Degrees, Degrees, Degrees>, IComparisonOperators<Degrees, Degrees, bool>
{
    public readonly float Value;
    public Degrees(float value)
    {
        Value = Normalize(value);
    }

    private static float Normalize(float value)
    {
        value %= 360;

        if (value < 0)
            value += 360;

        return value;
    }

    public Radians ToRadians() => new Radians(Value * MathF.PI / 180);

    public override string ToString() => $"{Value} degrees";

    // IEquatable
    public bool Equals(Degrees other) => Value == other.Value;
    public override bool Equals(object obj) => obj is Degrees other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Value);
    public static bool operator ==(Degrees left, Degrees right) => left.Value == right.Value;
    public static bool operator !=(Degrees left, Degrees right) => !(left.Value == right.Value);

    // IAdditionOperators
    public static Degrees operator +(Degrees a, Degrees b) => new Degrees(a.Value + b.Value);

    // ISubtractionOperators
    public static Degrees operator -(Degrees a, Degrees b) => new Degrees(a.Value - b.Value);

    // IMultiplyOperators
    public static Degrees operator *(Degrees a, Degrees b) => new Degrees(a.Value * b.Value);

    // IDivisionOperators
    public static Degrees operator /(Degrees a, Degrees b) => new Degrees(a.Value / b.Value);

    // IComparisonOperators
    public static bool operator >(Degrees a, Degrees b) => a.Value > b.Value;
    public static bool operator >=(Degrees a, Degrees b) => a.Value >= b.Value;
    public static bool operator <(Degrees a, Degrees b) => a.Value < b.Value;
    public static bool operator <=(Degrees a, Degrees b) => a.Value <= b.Value;
}