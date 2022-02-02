using System;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public partial struct SemanticPreRelease
    {
        // ReSharper disable once UnusedParameter.Local
        private SemanticPreRelease(string identifier, bool _)
        {
            text = identifier;
            number = default;
        }

        /// <summary>
        ///   <para>Converts the read-only span of characters representing a pre-release identifier to an equivalent <see cref="SemanticPreRelease"/> structure.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters representing a pre-release identifier.</param>
        /// <returns>A value that is equivalent to the pre-release identifier specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> does not represent a valid pre-release identifier.</exception>
        [Pure] public static SemanticPreRelease Parse(ReadOnlySpan<char> text)
            => Parse(text, SemanticOptions.Strict);
        /// <summary>
        ///   <para>Converts the read-only span of characters representing a pre-release identifier to an equivalent <see cref="SemanticPreRelease"/> structure using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters representing a pre-release identifier.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>A value that is equivalent to the pre-release identifier specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> does not represent a valid pre-release identifier.</exception>
        [Pure] public static SemanticPreRelease Parse(ReadOnlySpan<char> text, SemanticOptions options)
        {
            if (text.Length is 0) throw new ArgumentException(Exceptions.PreReleaseEmpty, nameof(text));
            SemanticErrorCode code = ParseInternal(text, options, out SemanticPreRelease preRelease);
            if (code != SemanticErrorCode.Success) throw new ArgumentException(code.GetErrorMessage(), nameof(text));
            if (!Util.ContainsOnlyValidCharacters(text))
                throw new ArgumentException(Exceptions.PreReleaseInvalid, nameof(text));
            return preRelease;
        }

        /// <summary>
        ///   <para>Converts the specified string representation of a pre-release identifier to an equivalent <see cref="SemanticPreRelease"/> structure.</para>
        /// </summary>
        /// <param name="text">The string representation of a pre-release identifier.</param>
        /// <returns>A value that is equivalent to the pre-release identifier specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> does not represent a valid pre-release identifier.</exception>
        [Pure] public static SemanticPreRelease Parse(string text)
            => Parse(text, SemanticOptions.Strict);
        /// <summary>
        ///   <para>Converts the specified string representation of a pre-release identifier to an equivalent <see cref="SemanticPreRelease"/> structure using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The string representation of a pre-release identifier.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>A value that is equivalent to the pre-release identifier specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> does not represent a valid pre-release identifier.</exception>
        [Pure] public static SemanticPreRelease Parse(string text, SemanticOptions options)
        {
            if (text.Length is 0) throw new ArgumentException(Exceptions.PreReleaseEmpty, nameof(text));
            SemanticErrorCode code = ParseInternal(text, options, out SemanticPreRelease preRelease);
            if (code != SemanticErrorCode.Success) throw new ArgumentException(code.GetErrorMessage(), nameof(text));
            if (!Util.ContainsOnlyValidCharacters(text))
                throw new ArgumentException(Exceptions.PreReleaseInvalid, nameof(text));
            return preRelease;
        }

        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a pre-release identifier to an equivalent <see cref="SemanticPreRelease"/> structure, and returns a value that indicates whether the conversion succeeded.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters representing a pre-release identifier.</param>
        /// <param name="preRelease">When this method returns, contains the <see cref="SemanticPreRelease"/> equivalent of the pre-release identifier that is specified in the <paramref name="text"/>, if the conversion succeeded, or <see cref="SemanticPreRelease.Zero"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the <paramref name="text"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out SemanticPreRelease preRelease)
            => TryParse(text, SemanticOptions.Strict, out preRelease);
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a pre-release identifier to an equivalent <see cref="SemanticPreRelease"/> structure using the specified parsing <paramref name="options"/>, and returns a value that indicates whether the conversion succeeded.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters representing a pre-release identifier.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="preRelease">When this method returns, contains the <see cref="SemanticPreRelease"/> equivalent of the pre-release identifier that is specified in the <paramref name="text"/>, if the conversion succeeded, or <see cref="SemanticPreRelease.Zero"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the <paramref name="text"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, SemanticOptions options, out SemanticPreRelease preRelease)
        {
            if (text.Length is 0) return Util.Fail(out preRelease);
            if (!Util.ContainsOnlyValidCharacters(text)) return Util.Fail(out preRelease);
            return ParseInternal(text, options, out preRelease) == SemanticErrorCode.Success;
        }

        /// <summary>
        ///   <para>Tries to convert the specified string representation of a pre-release identifier to an equivalent <see cref="SemanticPreRelease"/> structure, and returns a value that indicates whether the conversion succeeded.</para>
        /// </summary>
        /// <param name="text">The string representation of a pre-release identifier.</param>
        /// <param name="preRelease">When this method returns, contains the <see cref="SemanticPreRelease"/> equivalent of the pre-release identifier that is specified in the <paramref name="text"/>, if the conversion succeeded, or <see cref="SemanticPreRelease.Zero"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the <paramref name="text"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string text, out SemanticPreRelease preRelease)
            => TryParse(text, SemanticOptions.Strict, out preRelease);
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a pre-release identifier to an equivalent <see cref="SemanticPreRelease"/> structure using the specified parsing <paramref name="options"/>, and returns a value that indicates whether the conversion succeeded.</para>
        /// </summary>
        /// <param name="text">The string representation of a pre-release identifier.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="preRelease">When this method returns, contains the <see cref="SemanticPreRelease"/> equivalent of the pre-release identifier that is specified in the <paramref name="text"/>, if the conversion succeeded, or <see cref="SemanticPreRelease.Zero"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the <paramref name="text"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string text, SemanticOptions options, out SemanticPreRelease preRelease)
        {
            if (text.Length is 0) return Util.Fail(out preRelease);
            if (!Util.ContainsOnlyValidCharacters(text)) return Util.Fail(out preRelease);
            return ParseInternal(text, options, out preRelease) == SemanticErrorCode.Success;
        }

        internal static SemanticErrorCode ParseInternal(ReadOnlySpan<char> identifier, SemanticOptions options, out SemanticPreRelease preRelease)
        {
            identifier = Util.Trim(identifier, options);
            if (Util.IsAllDigits(identifier))
            {
                if (identifier[0] is '0' && identifier.Length > 1 && (options & SemanticOptions.AllowLeadingZeroes) is 0)
                    return Util.Fail(SemanticErrorCode.PreReleaseLeadingZeroes, out preRelease);
                if (!Util.TryParse(identifier, out int number))
                    return Util.Fail(SemanticErrorCode.PreReleaseTooBig, out preRelease);
                preRelease = new SemanticPreRelease(number);
            }
            else preRelease = new SemanticPreRelease(new string(identifier), false);
            return SemanticErrorCode.Success;
        }
        internal static SemanticErrorCode ParseInternal(string text, SemanticOptions options, out SemanticPreRelease preRelease)
        {
            if (Util.TryTrim(text, options, out ReadOnlySpan<char> trimmed))
            {
                if (Util.IsAllDigits(trimmed))
                {
                    if (trimmed[0] is '0' && trimmed.Length > 1 && (options & SemanticOptions.AllowLeadingZeroes) is 0)
                        return Util.Fail(SemanticErrorCode.PreReleaseLeadingZeroes, out preRelease);
                    if (!Util.TryParse(trimmed, out int number))
                        return Util.Fail(SemanticErrorCode.PreReleaseTooBig, out preRelease);
                    preRelease = new SemanticPreRelease(number);
                }
                else preRelease = new SemanticPreRelease(new string(trimmed), false);
                return SemanticErrorCode.Success;
            }
            if (Util.IsAllDigits(text))
            {
                if (text[0] is '0' && text.Length > 1 && (options & SemanticOptions.AllowLeadingZeroes) is 0)
                    return Util.Fail(SemanticErrorCode.PreReleaseLeadingZeroes, out preRelease);
                if (!Util.TryParse(text, out int number))
                    return Util.Fail(SemanticErrorCode.PreReleaseTooBig, out preRelease);
                preRelease = new SemanticPreRelease(number);
            }
            else preRelease = new SemanticPreRelease(text, false);
            return SemanticErrorCode.Success;
        }

    }
}
