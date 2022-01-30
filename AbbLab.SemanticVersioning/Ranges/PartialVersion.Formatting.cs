using System;
using System.Text;
using AbbLab.Parsing;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public partial class PartialVersion : IFormattable
    {
        /// <summary>
        ///   <para>Returns the string representation of this partial version.</para>
        /// </summary>
        /// <returns>The string representation of this partial version.</returns>
        [Pure] public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!Major.IsOmitted)
            {
                sb.SimpleAppend(Major);
                if (!Minor.IsOmitted)
                {
                    sb.Append('.').SimpleAppend(Minor);
                    if (!Patch.IsOmitted)
                        sb.Append('.').SimpleAppend(Patch);
                }
            }

            if (_preReleases.Length > 0)
            {
                sb.Append('-').SimpleAppend(_preReleases[0]);
                for (int i = 1, length = _preReleases.Length; i < length; i++)
                    sb.Append('.').SimpleAppend(_preReleases[i]);
            }
            if (_buildMetadata.Length > 0)
            {
                sb.Append('-').Append(_buildMetadata[0]);
                for (int i = 1, length = _buildMetadata.Length; i < length; i++)
                    sb.Append('.').Append(_buildMetadata[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        ///   <para>Converts this partial version to its equivalent string representation, using the specified <paramref name="format"/>.</para>
        ///   <para>
        ///     <paramref name="format"/> replaces the following patterns:
        ///     <c>'M'</c> - major version component;
        ///     <c>'MM'</c> - major version component, but only if it's greater than zero;
        ///     the same for minor and patch components with <c>'m'</c>/<c>'mm'</c> and <c>'p'</c>/<c>'pp'</c> accordingly;
        ///     <c>ppp</c> and <c>'mmm'</c> - pre-release or build metadata identifiers accordingly separated by dots (<c>'.'</c>).
        ///   </para>
        ///   <para>
        ///     Partial version components can be formatted by appending <c>:format</c>. For example, <c>M:0_</c> is equivalent to <c>MM</c>,
        ///     and <c>M:wx,_x</c> replaces all wildcards and omitted components with the <c>'x'</c> wildcard.
        ///     See <see cref="PartialComponent.ToString(string?)"/> for more information.
        ///   </para>
        ///   <para>
        ///     Separators, such as dots, hyphens, pluses and spaces (<c>'.'</c>, <c>'-'</c>, <c>'+'</c> and <c>' '</c>) are removed,
        ///     if the subsequent pattern is omitted (<c>'MM'</c>, <c>'mm'</c>, <c>'pp'</c>, or if there are no pre-release or build
        ///     metadata identifiers in the version).
        ///     The patterns can be escaped by inserting <c>'\'</c> before the pattern.
        ///     Note that you need to insert multiple backslashes in patterns like <c>'mm'</c>, otherwise the method will recognize
        ///     the last <c>'m'</c> as a pattern.
        ///   </para>
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The string representation of this partial version, as specified by <paramref name="format"/>.</returns>
        [Pure] public unsafe string ToString(string? format)
        {
            if (format is null or "G" or "g") return ToString();

            StringBuilder sb = new StringBuilder();
            StringParser parser = new StringParser(format);

            char separator = default;
            static void FlushSeparator(StringBuilder sb, ref char separator)
            {
                if (separator != default)
                {
                    sb.Append(separator);
                    separator = default;
                }
            }

            static bool IsPartialComponentFormatCharacter(int c)
                => c is '0' or 'x' or 'X' or '*' or '_' or 'w' or ',';

            while (parser.CanRead())
            {
                if (parser.Skip('\\'))
                {
                    // treat the next character as a normal character
                    sb.Append(parser.TryRead(out char read) ? read : '\\');
                }
                else if (parser.SkipAny('.', '-', '+', ' ', out int readSeparator))
                {
                    // separators are stored, in case the next identifier is omitted
                    FlushSeparator(sb, ref separator);
                    separator = (char)readSeparator;
                }
                else if (parser.Skip('M'))
                {
                    // 'M' - major version component
                    bool omitIfZero = parser.Skip('M'); // 'MM' - omits if zero
                    string majorString;
                    if (parser.Skip(':'))
                    {
                        // format: 'M:wx,_0'
                        ReadOnlySpan<char> majorFormat = parser.ReadWhile(&IsPartialComponentFormatCharacter);
                        majorString = Major.ToString(majorFormat);
                    }
                    else majorString = Major.ToString(omitIfZero ? "0_" : null);

                    if (majorString.Length > 0)
                    {
                        // append if not omitted
                        FlushSeparator(sb, ref separator);
                        sb.Append(majorString);
                    }
                    else separator = default;
                }
                else if (parser.Skip('m'))
                {
                    // 'm' - minor version component
                    bool omitIfZero = parser.Skip('m'); // 'mm' - omits if zero
                    if (omitIfZero && parser.Skip('m'))
                    {
                        // 'mmm' - build metadata identifiers, if any
                        int length = _buildMetadata.Length;
                        if (length > 0)
                        {
                            FlushSeparator(sb, ref separator);
                            sb.Append('-').Append(_buildMetadata[0]);
                            for (int i = 1; i < length; i++)
                                sb.Append('.').Append(_buildMetadata[i]);
                        }
                        else separator = default;
                    }
                    else
                    {
                        string minorString;
                        if (parser.Skip(':'))
                        {
                            // format: 'm:wx,_0'
                            ReadOnlySpan<char> minorFormat = parser.ReadWhile(&IsPartialComponentFormatCharacter);
                            minorString = Minor.ToString(minorFormat);
                        }
                        else minorString = Minor.ToString(omitIfZero ? "0_" : null);

                        if (minorString.Length > 0)
                        {
                            // append if not omitted
                            FlushSeparator(sb, ref separator);
                            sb.Append(minorString);
                        }
                        else separator = default;
                    }
                }
                else if (parser.Skip('p'))
                {
                    // 'p' - patch version component
                    bool omitIfZero = parser.Skip('p'); // 'pp' - omit if zero
                    if (omitIfZero && parser.Skip('p'))
                    {
                        // 'ppp' - pre-release identifiers, if any
                        int length = _preReleases.Length;
                        if (length > 0)
                        {
                            FlushSeparator(sb, ref separator);
                            sb.Append('-').SimpleAppend(_preReleases[0]);
                            for (int i = 1; i < length; i++)
                                sb.Append('.').SimpleAppend(_preReleases[i]);
                        }
                        else separator = default;
                    }
                    else
                    {
                        string patchString;
                        if (parser.Skip(':'))
                        {
                            // format: 'p:wx,_0'
                            ReadOnlySpan<char> patchFormat = parser.ReadWhile(&IsPartialComponentFormatCharacter);
                            patchString = Patch.ToString(patchFormat);
                        }
                        else patchString = Patch.ToString(omitIfZero ? "0_" : null);

                        if (patchString.Length > 0)
                        {
                            // append if not omitted
                            FlushSeparator(sb, ref separator);
                            sb.Append(patchString);
                        }
                        else separator = default;
                    }
                }
                else
                {
                    // non-pattern character
                    FlushSeparator(sb, ref separator);
                    sb.Append(parser.Read());
                }
            }

            return sb.ToString();
        }
        string IFormattable.ToString(string? format, IFormatProvider? _) => ToString(format);
    }
}
