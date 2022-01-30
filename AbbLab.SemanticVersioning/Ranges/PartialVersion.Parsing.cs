using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AbbLab.Parsing;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public partial class PartialVersion
    {
        /// <summary>
        ///   <para>Converts the specified string representation of a partial version to an equivalent <see cref="PartialVersion"/> object.</para>
        /// </summary>
        /// <param name="text">The string representation of a partial version.</param>
        /// <returns>A value that is equivalent to the partial version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> does not represent a valid partial version.</exception>
        [Pure] public static PartialVersion Parse(string text)
            => Parse(text.AsSpan(), SemanticOptions.Strict);
        /// <summary>
        ///   <para>Converts the specified string representation of a partial version to an equivalent <see cref="PartialVersion"/> object using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The string representation of a partial version.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>A value that is equivalent to the partial version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> does not represent a valid partial version.</exception>
        [Pure] public static PartialVersion Parse(string text, SemanticOptions options)
            => Parse(text.AsSpan(), options);
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a partial version to an equivalent <see cref="PartialVersion"/> object.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters representing a partial version.</param>
        /// <returns>A value that is equivalent to the partial version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> does not represent a valid partial version.</exception>
        [Pure] public static PartialVersion Parse(ReadOnlySpan<char> text)
            => Parse(text, SemanticOptions.Strict);
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a partial version to an equivalent <see cref="PartialVersion"/> object using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters representing a partial version.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>A value that is equivalent to the partial version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> does not represent a valid partial version.</exception>
        [Pure] public static PartialVersion Parse(ReadOnlySpan<char> text, SemanticOptions options)
        {
            if (options == SemanticOptions.Strict) return Parse(text);

            StringParser parser = new StringParser(text);

            if ((options & SemanticOptions.AllowLeadingWhite) is not 0)
                parser.SkipWhitespaces();

            SemanticErrorCode code = ParseInternal(ref parser, options, out PartialVersion? version);
            if (code != SemanticErrorCode.Success) throw new ArgumentException(code.GetErrorMessage(), nameof(text));

            if ((options & SemanticOptions.AllowTrailingWhite) is not 0)
                parser.SkipWhitespaces();
            if (parser.CanRead() && (options & SemanticOptions.AllowLeftovers) is 0)
                throw new ArgumentException($"Encountered an unexpected character at index {parser.Position}.", nameof(text));

            return version!;
        }

        /// <summary>
        ///   <para>Tries to convert the specified string representation of a partial version to an equivalent <see cref="PartialVersion"/> object, and returns a value that indicates whether the conversion succeeded.</para>
        /// </summary>
        /// <param name="text">The string representation of a partial version.</param>
        /// <param name="version">When this method returns, contains the <see cref="PartialVersion"/> equivalent of the partial version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the <paramref name="text"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string text, [NotNullWhen(true)] out PartialVersion? version)
            => TryParse(text.AsSpan(), SemanticOptions.Strict, out version);
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a partial version to an equivalent <see cref="PartialVersion"/> object using the specified parsing <paramref name="options"/>, and returns a value that indicates whether the conversion succeeded.</para>
        /// </summary>
        /// <param name="text">The string representation of a partial version.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="version">When this method returns, contains the <see cref="PartialVersion"/> equivalent of the partial version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the <paramref name="text"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string text, SemanticOptions options, [NotNullWhen(true)] out PartialVersion? version)
            => TryParse(text.AsSpan(), options, out version);
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a partial version to an equivalent <see cref="PartialVersion"/> object, and returns a value that indicates whether the conversion succeeded.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters representing a partial version.</param>
        /// <param name="version">When this method returns, contains the <see cref="PartialVersion"/> equivalent of the partial version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the <paramref name="text"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out PartialVersion? version)
            => TryParse(text, SemanticOptions.Strict, out version);
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a partial version to an equivalent <see cref="PartialVersion"/> object using the specified parsing <paramref name="options"/>, and returns a value that indicates whether the conversion succeeded.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters representing a partial version.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="version">When this method returns, contains the <see cref="PartialVersion"/> equivalent of the partial version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the <paramref name="text"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, SemanticOptions options, [NotNullWhen(true)] out PartialVersion? version)
        {
            if (options == SemanticOptions.Strict) return TryParse(text, out version);

            StringParser parser = new StringParser(text);

            if ((options & SemanticOptions.AllowLeadingWhite) is not 0)
                parser.SkipWhitespaces();

            SemanticErrorCode code = ParseInternal(ref parser, options, out PartialVersion? result);
            if (code != SemanticErrorCode.Success) return Util.Fail(out version);

            if ((options & SemanticOptions.AllowTrailingWhite) is not 0)
                parser.SkipWhitespaces();
            if (parser.CanRead() && (options & SemanticOptions.AllowLeftovers) is 0)
                return Util.Fail(out version);

            version = result!;
            return true;
        }

        internal static unsafe SemanticErrorCode ParseInternal(ref StringParser parser, SemanticOptions options, out PartialVersion? version)
        {
            bool innerWhite = (options & SemanticOptions.AllowInnerWhite) is not 0;
            if ((options & SemanticOptions.AllowEqualsPrefix) is not 0)
            {
                if (parser.Skip('=') && innerWhite)
                    parser.SkipWhitespaces();
            }
            if ((options & SemanticOptions.AllowVersionPrefix) is not 0)
            {
                if (parser.SkipAny('v', 'V') && innerWhite)
                    parser.SkipWhitespaces();
            }
            bool noLeadingZeroes = (options & SemanticOptions.AllowLeadingZeroes) is 0;

            PartialComponent major = PartialComponent.Omitted;
            PartialComponent minor = PartialComponent.Omitted;
            PartialComponent patch = PartialComponent.Omitted;
            if (Util.IsDigit(parser.Peek()))
            {
                ReadOnlySpan<char> majorSpan = parser.ReadWhile(&Util.IsDigit);
                if (majorSpan[0] is '0' && majorSpan.Length > 1 && noLeadingZeroes)
                    return Util.Fail(SemanticErrorCode.MajorLeadingZeroes, out version);
                if (!Util.TryParse(majorSpan, out int majorNum))
                    return Util.Fail(SemanticErrorCode.MajorTooBig, out version);
                major = new PartialComponent(majorNum);
                if (innerWhite) parser.SkipWhitespaces();
            }
            else if (parser.SkipAny('x', 'X', '*', out int wildcard))
            {
                major = PartialComponent.GetWildcardComponent((char)wildcard);
                if (innerWhite) parser.SkipWhitespaces();
            }

            if (!major.IsOmitted && parser.Skip('.'))
            {
                if (Util.IsDigit(parser.Peek()))
                {
                    ReadOnlySpan<char> minorSpan = parser.ReadWhile(&Util.IsDigit);
                    if (minorSpan[0] is '0' && minorSpan.Length > 1 && noLeadingZeroes)
                        return Util.Fail(SemanticErrorCode.MinorLeadingZeroes, out version);
                    if (!Util.TryParse(minorSpan, out int minorNum))
                        return Util.Fail(SemanticErrorCode.MinorTooBig, out version);
                    minor = new PartialComponent(minorNum);
                    if (innerWhite) parser.SkipWhitespaces();
                }
                else if (parser.SkipAny('x', 'X', '*', out int wildcard))
                {
                    minor = PartialComponent.GetWildcardComponent((char)wildcard);
                    if (innerWhite) parser.SkipWhitespaces();
                }
            }

            if (!minor.IsOmitted && parser.Skip('.'))
            {
                if (Util.IsDigit(parser.Peek()))
                {
                    ReadOnlySpan<char> patchSpan = parser.ReadWhile(&Util.IsDigit);
                    if (patchSpan[0] is '0' && patchSpan.Length > 1 && noLeadingZeroes)
                        return Util.Fail(SemanticErrorCode.PatchLeadingZeroes, out version);
                    if (!Util.TryParse(patchSpan, out int patchNum))
                        return Util.Fail(SemanticErrorCode.PatchTooBig, out version);
                    patch = new PartialComponent(patchNum);
                    if (innerWhite) parser.SkipWhitespaces();
                }
                else if (parser.SkipAny('x', 'X', '*', out int wildcard))
                {
                    patch = PartialComponent.GetWildcardComponent((char)wildcard);
                    if (innerWhite) parser.SkipWhitespaces();
                }
            }

            SemanticPreRelease[]? preReleases = null;
            string[]? buildMetadata = null;
            if (!patch.IsOmitted && !char.IsWhiteSpace((char)parser.PeekBack()))
            {
                if (parser.Skip('-'))
                {
                    if (innerWhite) parser.SkipWhitespaces();
                    List<SemanticPreRelease> list = new List<SemanticPreRelease>();
                    do
                    {
                        ReadOnlySpan<char> identifier = parser.ReadWhile(&Util.IsValidCharacter);
                        if (identifier.Length is 0) return Util.Fail(SemanticErrorCode.PreReleaseNotFound, out version);
                        SemanticErrorCode code = SemanticPreRelease.ParseInternal(identifier, options, out SemanticPreRelease preRelease);
                        if (code != SemanticErrorCode.Success) return Util.Fail(code, out version);
                        list.Add(preRelease);
                        if (innerWhite) parser.SkipWhitespaces();
                    }
                    while (parser.Skip('.'));
                    preReleases = list.ToArray();
                }
                else if ((options & SemanticOptions.OptionalPreReleaseSeparator) is not 0 && Util.IsValidCharacter(parser.Peek()))
                {
                    List<SemanticPreRelease> list = new List<SemanticPreRelease>();
                    do
                    {
                        bool isNumeric = Util.IsDigit(parser.Peek());
                        ReadOnlySpan<char> identifier = parser.ReadWhile(isNumeric ? &Util.IsDigit : &Util.IsLetter);
                        SemanticErrorCode code = SemanticPreRelease.ParseInternal(identifier, options, out SemanticPreRelease preRelease);
                        if (code != SemanticErrorCode.Success) return Util.Fail(code, out version);
                        list.Add(preRelease);
                    }
                    while (Util.IsValidCharacter(parser.Peek()));
                    preReleases = list.ToArray();
                }

                if (parser.Skip('+'))
                {
                    if (innerWhite) parser.SkipWhitespaces();
                    List<string> list = new List<string>();
                    do
                    {
                        string identifier = parser.ReadStringWhile(&Util.IsValidCharacter);
                        if (identifier.Length is 0) return Util.Fail(SemanticErrorCode.BuildMetadataNotFound, out version);
                        list.Add(identifier);
                        if (innerWhite) parser.SkipWhitespaces();
                    }
                    while (parser.Skip('.'));
                    buildMetadata = list.ToArray();
                }
            }

            version = new PartialVersion(major, minor, patch, preReleases, buildMetadata);
            return SemanticErrorCode.Success;
        }

    }
}
