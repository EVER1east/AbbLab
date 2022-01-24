using System;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public readonly partial struct PartialComponent : IEquatable<PartialComponent>, IComparable<PartialComponent>, IComparable
    {
        private readonly int _value;

        public bool IsWildcard => _value < -1;
        public bool IsOmitted => _value is -1;
        public bool IsNumeric => _value > -1;
        public int Value => _value > -1 ? _value : throw new InvalidOperationException("The version component is not numeric.");
        [Pure] public char GetWildcardCharacter() => _value switch
        {
            -4 => 'x', -3 => 'X', -2 => '*',
            _ => throw new InvalidOperationException("The version component is not a wildcard."),
        };

        public PartialComponent(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, "The version component cannot be less than 0.");
            _value = value;
        }

        // The constructor used to set up special values
        // ReSharper disable once UnusedParameter.Local
        private PartialComponent(int value, bool _) => _value = value;

        [Pure] public static implicit operator PartialComponent(int value) => new PartialComponent(value);
        [Pure] public static explicit operator int(PartialComponent component) => component.Value;

        public static readonly PartialComponent X = new PartialComponent(-4, false);
        public static readonly PartialComponent CapitalX = new PartialComponent(-3, false);
        public static readonly PartialComponent Star = new PartialComponent(-2, false);
        public static readonly PartialComponent Omitted = new PartialComponent(-1, false);
        public static readonly PartialComponent Zero = new PartialComponent(0);

        public static PartialComponent GetWildcardComponent(char character) => character switch
        {
            'x' => X, 'X' => CapitalX, '*' => Star,
            _ => throw new ArgumentException("The specified character is not a wildcard character.", nameof(character)),
        };

        [Pure] public bool Equals(PartialComponent other) => _value < -1 ? other._value < -1 : _value == other._value;
        [Pure] public override bool Equals(object? obj) => obj is PartialComponent other && Equals(other);
        [Pure] public override int GetHashCode() => _value < 0 ? -1 : _value;

        [Pure] public int CompareTo(PartialComponent other)
        {
            if (_value < 0 && other._value < 0) return 0;
            return _value.CompareTo(other._value);
        }
        int IComparable.CompareTo(object? obj)
        {
            if (obj is PartialComponent other) return CompareTo(other);
            throw new ArgumentException($"The object must be of type {nameof(PartialComponent)}.", nameof(obj));
        }

        [Pure] public static bool operator ==(PartialComponent a, PartialComponent b) => a.Equals(b);
        [Pure] public static bool operator !=(PartialComponent a, PartialComponent b) => !a.Equals(b);

        [Pure] public static bool operator >(PartialComponent a, PartialComponent b) => a.CompareTo(b) > 0;
        [Pure] public static bool operator <(PartialComponent a, PartialComponent b) => a.CompareTo(b) < 0;
        [Pure] public static bool operator >=(PartialComponent a, PartialComponent b) => a.CompareTo(b) >= 0;
        [Pure] public static bool operator <=(PartialComponent a, PartialComponent b) => a.CompareTo(b) <= 0;

        [Pure] public int GetValueOrZero() => _value > -1 ? _value : 0;

    }
}
