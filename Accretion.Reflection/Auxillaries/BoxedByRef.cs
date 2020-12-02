using System;

namespace Accretion.Reflection
{
    internal readonly struct BoxedByRef : IEquatable<BoxedByRef>
    {
        public BoxedByRef(Type elementType, object rawValue)
        {
            ElementType = elementType;
            RawValue = rawValue;
        }

        public Type ElementType { get; }
        public object RawValue { get; }

        public override bool Equals(object? obj) => obj is BoxedByRef @ref && Equals(@ref);
        public bool Equals(BoxedByRef other) => ElementType == other.ElementType && Equals(RawValue, other.RawValue);
        public override int GetHashCode() => HashCode.Combine(ElementType, RawValue);
        public override string ToString() => $"{ElementType}& - {RawValue}";

        public static bool operator ==(BoxedByRef left, BoxedByRef right) => left.Equals(right);
        public static bool operator !=(BoxedByRef left, BoxedByRef right) => !(left == right);
    }
}
