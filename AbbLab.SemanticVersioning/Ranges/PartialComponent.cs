using System;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    /// <summary>
    ///   <para>Represents a partial version component – either a non-negative numeric value, omitted entirely, or one of the wildcards: <c>'x'</c>, <c>'X'</c>, <c>'*'</c>.</para>
    ///   <para>Note: non-numeric values are considered to be equal to each other and less than any numeric value.</para>
    /// </summary>
    public readonly partial struct PartialComponent : IEquatable<PartialComponent>, IComparable<PartialComponent>, IComparable
    {
        private readonly int _value;

        /// <summary>
        ///   <para>Determines whether the partial version component is a wildcard.</para>
        /// </summary>
        public bool IsWildcard => _value < -1;
        /// <summary>
        ///   <para>Determines whether the partial version component is omitted.</para>
        /// </summary>
        public bool IsOmitted => _value is -1;
        /// <summary>
        ///   <para>Determines whether the partial version component is numeric.</para>
        /// </summary>
        public bool IsNumeric => _value > -1;
        /// <summary>
        ///   <para>Gets the numeric value of the partial version component, if it is numeric; otherwise, throws an exception.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">The partial version component is not numeric.</exception>
        public int Value => _value > -1 ? _value : throw new InvalidOperationException("The partial version component is not numeric.");
        /// <summary>
        ///   <para>Gets the wildcard character representing the partial version component, if it is a wildcard; otherwise, throws an exception.</para>
        /// </summary>
        /// <returns>The wildcard character representing the partial version component.</returns>
        /// <exception cref="InvalidOperationException">The partial version component is not a wildcard.</exception>
        [Pure] public char GetWildcardCharacter() => _value switch
        {
            -4 => 'x', -3 => 'X', -2 => '*',
            _ => throw new InvalidOperationException("The partial version component is not a wildcard."),
        };

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialComponent"/> structure with the specified numeric <paramref name="value"/>.</para>
        /// </summary>
        /// <param name="value">The non-negative numeric value of the partial version component.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public PartialComponent(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, "The version component cannot be less than 0.");
            _value = value;
        }

        // The constructor used to set up special values
        // ReSharper disable once UnusedParameter.Local
        private PartialComponent(int value, bool _) => _value = value;

        /// <summary>
        ///   <para>Defines an implicit conversion of a 32-bit signed integer to a <see cref="PartialComponent"/> value.</para>
        /// </summary>
        /// <param name="value">The non-negative numeric value of the partial version component.</param>
        [Pure] public static implicit operator PartialComponent(int value) => new PartialComponent(value);
        /// <summary>
        ///   <para>Defines an explicit conversion of a <see cref="PartialComponent"/> value to a 32-bit signed integer.</para>
        /// </summary>
        /// <param name="component">The partial version component to get the numeric value of.</param>
        /// <exception cref="InvalidOperationException"><paramref name="component"/> is not numeric.</exception>
        [Pure] public static explicit operator int(PartialComponent component) => component.Value;

        /// <summary>
        ///   <para>The partial version component that represents the <c>'x'</c> wildcard character.</para>
        /// </summary>
        public static readonly PartialComponent X = new PartialComponent(-4, false);
        /// <summary>
        ///   <para>The partial version component that represents the <c>'X'</c> wildcard character.</para>
        /// </summary>
        public static readonly PartialComponent CapitalX = new PartialComponent(-3, false);
        /// <summary>
        ///   <para>The partial version component that represents the <c>'*'</c> wildcard character.</para>
        /// </summary>
        public static readonly PartialComponent Star = new PartialComponent(-2, false);
        /// <summary>
        ///   <para>The partial version component that represents an omitted value.</para>
        /// </summary>
        public static readonly PartialComponent Omitted = new PartialComponent(-1, false);
        /// <summary>
        ///   <para>The partial version component with a numeric value of 0.</para>
        /// </summary>
        public static readonly PartialComponent Zero = new PartialComponent(0);

        /// <summary>
        ///   <para>Gets the partial version component that represents the specified wildcard <paramref name="character"/>.</para>
        /// </summary>
        /// <param name="character">The partial version component wildcard: <c>'x'</c>, <c>'X'</c> or <c>'*'</c>.</param>
        /// <returns>The partial version component that represents the specified wildcard <paramref name="character"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="character"/> is not a wildcard character.</exception>
        [Pure] public static PartialComponent GetWildcardComponent(char character) => character switch
        {
            'x' => X, 'X' => CapitalX, '*' => Star,
            _ => throw new ArgumentException("The specified character is not a wildcard character.", nameof(character)),
        };

        /// <inheritdoc/>
        [Pure] public bool Equals(PartialComponent other) => _value < -1 ? other._value < -1 : _value == other._value;
        /// <inheritdoc/>
        [Pure] public override bool Equals(object? obj) => obj is PartialComponent other && Equals(other);
        /// <inheritdoc/>
        [Pure] public override int GetHashCode() => _value < 0 ? -1 : _value;

        /// <inheritdoc/>
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

        /// <summary>
        ///   <para>Determines whether two instances of <see cref="PartialComponent"/> have equal precedence.</para>
        /// </summary>
        /// <param name="a">The first <see cref="PartialComponent"/> structure to compare.</param>
        /// <param name="b">The second <see cref="PartialComponent"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> and <paramref name="b"/> are equal; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(PartialComponent a, PartialComponent b) => a.Equals(b);
        /// <summary>
        ///   <para>Determines whether two instances of <see cref="PartialComponent"/> don't have equal precedence.</para>
        /// </summary>
        /// <param name="a">The first <see cref="PartialComponent"/> structure to compare.</param>
        /// <param name="b">The second <see cref="PartialComponent"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> and <paramref name="b"/> are equal; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(PartialComponent a, PartialComponent b) => !a.Equals(b);

        /// <summary>
        ///   <para>Determines whether the precedence of one instance of <see cref="PartialComponent"/> is greater than the precedence of another.</para>
        /// </summary>
        /// <param name="a">The first <see cref="PartialComponent"/> structure to compare.</param>
        /// <param name="b">The second <see cref="PartialComponent"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if the precedence of <paramref name="a"/> is greater than the precedence of <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator >(PartialComponent a, PartialComponent b) => a.CompareTo(b) > 0;
        /// <summary>
        ///   <para>Determines whether the precedence of one instance of <see cref="PartialComponent"/> is less than the precedence of another.</para>
        /// </summary>
        /// <param name="a">The first <see cref="PartialComponent"/> structure to compare.</param>
        /// <param name="b">The second <see cref="PartialComponent"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if the precedence of <paramref name="a"/> is less than the precedence of <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator <(PartialComponent a, PartialComponent b) => a.CompareTo(b) < 0;
        /// <summary>
        ///   <para>Determines whether the precedence of one instance of <see cref="PartialComponent"/> is greater than or equal to the precedence of another.</para>
        /// </summary>
        /// <param name="a">The first <see cref="PartialComponent"/> structure to compare.</param>
        /// <param name="b">The second <see cref="PartialComponent"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if the precedence of <paramref name="a"/> is greater than or equal to the precedence of <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator >=(PartialComponent a, PartialComponent b) => a.CompareTo(b) >= 0;
        /// <summary>
        ///   <para>Determines whether the precedence of one instance of <see cref="PartialComponent"/> is less than or equal to the precedence of another.</para>
        /// </summary>
        /// <param name="a">The first <see cref="PartialComponent"/> structure to compare.</param>
        /// <param name="b">The second <see cref="PartialComponent"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if the precedence of <paramref name="a"/> is less than or equal to the precedence of <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator <=(PartialComponent a, PartialComponent b) => a.CompareTo(b) <= 0;

        /// <summary>
        ///   <para>Returns the numeric value of the partial version component, if it's numeric; otherwise, returns 0.</para>
        /// </summary>
        /// <returns>The numeric value of the partial version component, if it's numeric; otherwise, 0.</returns>
        [Pure] public int GetValueOrZero() => _value > -1 ? _value : 0;

    }
}
