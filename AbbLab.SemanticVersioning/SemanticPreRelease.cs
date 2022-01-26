using System;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    /// <summary>
    ///   <para>Represents a semantic version pre-release identifier.</para>
    /// </summary>
    public readonly partial struct SemanticPreRelease : IEquatable<SemanticPreRelease>, IComparable<SemanticPreRelease>, IComparable
    {
        internal readonly string? text;
        internal readonly int number;

        /// <summary>
        ///   <para>Determines whether the pre-release identifier is numeric.</para>
        /// </summary>
        public bool IsNumeric => text is null;
        /// <summary>
        ///   <para>Gets the alphanumeric value of the pre-release, if it is alphanumeric; otherwise, throws an exception.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">The pre-release identifier is not alphanumeric.</exception>
        public string Text => text ?? throw new InvalidOperationException("The pre-release identifier is not alphanumeric.");
        /// <summary>
        ///   <para>Gets the numeric value of the pre-release, if it is numeric; otherwise, throws an exception.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">The pre-release identifier is not numeric.</exception>
        public int Number => text is null ? number : throw new InvalidOperationException("The pre-release identifier is not numeric.");

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticPreRelease"/> structure with the specified <paramref name="identifier"/>.</para>
        /// </summary>
        /// <param name="identifier">The string representation of the pre-release identifier.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        public SemanticPreRelease(string identifier) => this = Parse(identifier);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticPreRelease"/> structure with the specified <paramref name="identifier"/> using the specified <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="identifier">The string representation of the pre-release identifier.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        public SemanticPreRelease(string identifier, SemanticOptions options) => this = Parse(identifier, options);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticPreRelease"/> structure with the specified <paramref name="identifier"/>.</para>
        /// </summary>
        /// <param name="identifier">The read-only span of characters representing the pre-release identifier.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        public SemanticPreRelease(ReadOnlySpan<char> identifier) => this = Parse(identifier);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticPreRelease"/> structure with the specified <paramref name="identifier"/> using the specified <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="identifier">The read-only span of characters representing the pre-release identifier.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        public SemanticPreRelease(ReadOnlySpan<char> identifier, SemanticOptions options) => this = Parse(identifier, options);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticPreRelease"/> structure with the specified <paramref name="identifier"/>.</para>
        /// </summary>
        /// <param name="identifier">The numeric value of the pre-release identifier.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="identifier"/> is less than 0.</exception>
        public SemanticPreRelease(int identifier)
        {
            if (identifier < 0) throw new ArgumentOutOfRangeException(nameof(identifier), identifier, Exceptions.PreReleaseNegative);
            text = null;
            number = identifier;
        }

        /// <summary>
        ///   <para>Defines an implicit conversion of a string to a <see cref="SemanticPreRelease"/> value.</para>
        /// </summary>
        /// <param name="identifier">The string to convert to a <see cref="SemanticPreRelease"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        [Pure] public static implicit operator SemanticPreRelease(string identifier) => Parse(identifier);
        /// <summary>
        ///   <para>Defines an implicit conversion of a read-only span of characters to a <see cref="SemanticPreRelease"/> value.</para>
        /// </summary>
        /// <param name="identifier">The read-only span of characters to convert to a <see cref="SemanticPreRelease"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        [Pure] public static implicit operator SemanticPreRelease(ReadOnlySpan<char> identifier) => Parse(identifier);
        /// <summary>
        ///   <para>Defines an implicit conversion of a 32-bit signed integer to a <see cref="SemanticPreRelease"/> value.</para>
        /// </summary>
        /// <param name="identifier">The 32-bit signed integer to convert to a <see cref="SemanticPreRelease"/> value.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="identifier"/> is less than 0.</exception>
        [Pure] public static implicit operator SemanticPreRelease(int identifier) => new SemanticPreRelease(identifier);
        /// <summary>
        ///   <para>Defines an explicit conversion of a <see cref="SemanticPreRelease"/> value to a string.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to convert to a string.</param>
        [Pure] public static explicit operator string(SemanticPreRelease preRelease) => preRelease.ToString();
        /// <summary>
        ///   <para>Defines an explicit conversion of a <see cref="SemanticPreRelease"/> value to a 32-bit signed integer.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to convert to a 32-bit signed integer.</param>
        /// <exception cref="InvalidOperationException"><paramref name="preRelease"/> is not a numeric pre-release identifier.</exception>
        [Pure] public static explicit operator int(SemanticPreRelease preRelease) => preRelease.Number;

        /// <summary>
        ///   <para>The pre-release identifier with a numeric value of <c>0</c>.</para>
        /// </summary>
        public static readonly SemanticPreRelease Zero = new SemanticPreRelease(0);
        internal static readonly SemanticPreRelease[] ZeroArray = { Zero };

        /// <inheritdoc/>
        [Pure] public bool Equals(SemanticPreRelease other)
        {
            bool isNumeric = text is null;
            if (isNumeric != other.text is null) return false;
            return isNumeric ? number == other.number : text == other.text;
        }
        /// <inheritdoc/>
        [Pure] public override bool Equals(object? obj) => obj is SemanticPreRelease other && Equals(other);
        /// <inheritdoc/>
        [Pure] public override int GetHashCode() => text?.GetHashCode() ?? number;

        /// <inheritdoc/>
        [Pure] public int CompareTo(SemanticPreRelease other)
        {
            bool isNumeric = text is null;
            if (isNumeric != other.text is null) return isNumeric ? -1 : 1;
            return isNumeric ? number.CompareTo(other.number) : string.CompareOrdinal(text, other.text);
        }
        int IComparable.CompareTo(object? obj)
        {
            if (obj is SemanticPreRelease other) return CompareTo(other);
            throw new ArgumentException($"The object must be of type {nameof(SemanticPreRelease)}.", nameof(obj));
        }

        /// <summary>
        ///   <para>Determines whether two instances of <see cref="SemanticPreRelease"/> have equal precedence.</para>
        /// </summary>
        /// <param name="a">The first <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <param name="b">The second <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> and <paramref name="b"/> are equal; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(SemanticPreRelease a, SemanticPreRelease b) => a.Equals(b);
        /// <summary>
        ///   <para>Determines whether two instances of <see cref="SemanticPreRelease"/> don't have equal precedence.</para>
        /// </summary>
        /// <param name="a">The first <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <param name="b">The second <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> and <paramref name="b"/> are not equal; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(SemanticPreRelease a, SemanticPreRelease b) => !a.Equals(b);

        /// <summary>
        ///   <para>Determines whether the precedence of one instance of <see cref="SemanticPreRelease"/> is greater than the precedence of another.</para>
        /// </summary>
        /// <param name="a">The first <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <param name="b">The second <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if the precedence of <paramref name="a"/> is greater than the precedence of <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator >(SemanticPreRelease a, SemanticPreRelease b) => a.CompareTo(b) > 0;
        /// <summary>
        ///   <para>Determines whether the precedence of one instance of <see cref="SemanticPreRelease"/> is less than the precedence of another.</para>
        /// </summary>
        /// <param name="a">The first <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <param name="b">The second <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if the precedence of <paramref name="a"/> is less than the precedence of <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator <(SemanticPreRelease a, SemanticPreRelease b) => a.CompareTo(b) < 0;
        /// <summary>
        ///   <para>Determines whether the precedence of one instance of <see cref="SemanticPreRelease"/> is greater than or equal to the precedence of another.</para>
        /// </summary>
        /// <param name="a">The first <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <param name="b">The second <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if the precedence of <paramref name="a"/> is greater than or equal to the precedence of <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator >=(SemanticPreRelease a, SemanticPreRelease b) => a.CompareTo(b) >= 0;
        /// <summary>
        ///   <para>Determines whether the precedence of one instance of <see cref="SemanticPreRelease"/> is less than or equal to the precedence of another.</para>
        /// </summary>
        /// <param name="a">The first <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <param name="b">The second <see cref="SemanticPreRelease"/> structure to compare.</param>
        /// <returns><see langword="true"/>, if the precedence of <paramref name="a"/> is less than or equal to the precedence of <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator <=(SemanticPreRelease a, SemanticPreRelease b) => a.CompareTo(b) <= 0;

        /// <summary>
        ///   <para>Converts the value of this pre-release identifier to its equivalent string representation.</para>
        /// </summary>
        /// <returns>The string representation of this pre-release identifier.</returns>
        [Pure] public override string ToString() => text ?? Util.ToString(number);

    }
}