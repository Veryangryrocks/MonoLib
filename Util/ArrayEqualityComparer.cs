using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoLib.Util;

public sealed class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
{
    public bool Equals(T[] x, T[] y) => StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
    public int GetHashCode(T[] obj) => StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
}