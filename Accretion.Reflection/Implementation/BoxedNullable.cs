using System;
using System.Diagnostics;

namespace Accretion.Intervals
{
    internal readonly struct BoxedNullable : IEquatable<BoxedNullable>
    {
        public BoxedNullable(Type type, object value)
        {
            Debug.Assert(value != null, "Value passed to the BoxedNullable constructor cannot be null.");
            Debug.Assert(type != null, "Type passed to the BoxedNullable constructor cannot be null.");
            Debug.Assert(Nullable.GetUnderlyingType(type) == value.GetType(), "Value passed to the BoxedNullable constructor must be of the underlying type of the nullable.");

            NullableType = type;
            UnderlyingType = Nullable.GetUnderlyingType(type);
            UnderlyingValue = value;
        }

        public Type NullableType { get; }
        public Type UnderlyingType { get; }
        public object UnderlyingValue { get; }

        public override bool Equals(object obj) => obj is BoxedNullable nullable && Equals(nullable);
        public bool Equals(BoxedNullable other) => NullableType == other.NullableType && UnderlyingType == other.UnderlyingType && UnderlyingValue.Equals(other.UnderlyingValue);
        public override int GetHashCode() => HashCode.Combine(NullableType, UnderlyingType, UnderlyingValue);
        public override string ToString() => $"{NullableType} - {UnderlyingValue}";

        public static bool operator ==(BoxedNullable left, BoxedNullable right) => left.Equals(right);
        public static bool operator !=(BoxedNullable left, BoxedNullable right) => !(left == right);
    }
}
