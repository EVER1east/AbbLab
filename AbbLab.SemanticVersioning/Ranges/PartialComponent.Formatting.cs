using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public partial struct PartialComponent : IFormattable
    {
        /// <summary>
        ///   <para>Converts the value of this partial version component to its equivalent string representation.</para>
        /// </summary>
        /// <returns>The string representation of this partial version component.</returns>
        [Pure] public override string ToString() => _value switch
        {
            -4 => "x", -3 => "X", -2 => "*", -1 => string.Empty,
            _ => Util.ToString(_value),
        };
		/// <summary>
		///   <para>Converts the value of this partial version component to its equivalent string representation, using the specified <paramref name="format"/>.</para>
        ///   <para>
        ///     <paramref name="format"/> is a collection of two-character <b>rules</b> joined with commas (<c>','</c>).
        ///     The first character defines the condition of the rule, and the second character defines the return value.
        ///     Possible values: <c>'0'</c> (is zero), <c>'x'</c>, <c>'X'</c>, <c>'*'</c> (wildcards), <c>'_'</c> (omitted).
        ///     The first character can also have the value of <c>'w'</c>, which matches any wildcard
        ///     (note: it must be specified after any other wildcard specifiers).
        ///     If the component has a positive numeric value, the method ignores the formatting rules.
        ///   </para>
        ///   <para>
        ///     Examples:
        ///     <c>"0_"</c> - omits if zero;
        ///     <c>"0_,wx"</c> - omits if zero, replaces wildcards with <c>'x'</c>;
        ///     <c>"x0,_*"</c> - replaces <c>'x'</c> with <c>'0'</c>, instead of omitting uses <c>'*'</c>.
        ///   </para>
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <returns>The string representation of this partial version component, as specified by <paramref name="format"/>.</returns>
		/// <exception cref="ArgumentException"><paramref name="format"/> is not a valid format string.</exception>
        [Pure] public string ToString(string? format)
        {
			// BNF:
			// <format>    ::= <rule> ( ',' <rule> )* | ''
			// <rule>      ::= <specifier> <replacer>
			// <specifier> ::= <replacer> | 'w'
			// <replacer>  ::= '0' | 'x' | 'X' | '*' | '_'

            if (_value > -1) return Util.ToString(_value);
            if (format is null or "G" or "g") return ToString();

            char mySpecifier = _value == 0 ? '0' : _value == -1 ? '_' : GetWildcardCharacter();
            bool isWildcard = IsWildcard;

            int length = format.Length;
            if ((length - 2) % 3 != 0) throw new ArgumentException("Invalid format string.", nameof(format));
            int position = 0;

            while (position < length)
            {
                char specifier = format[position++];
                char replacer = format[position++];
                if (position < length && format[position++] != ',')
                    throw new ArgumentException("Expected a comma between rules.", nameof(format));

                if (specifier == mySpecifier || specifier == 'w' && isWildcard)
                    return replacer.ToString();
            }
            return mySpecifier == '_' ? string.Empty : mySpecifier.ToString();
        }
        string IFormattable.ToString(string? format, IFormatProvider? _)
            => ToString(format);

    }
}
