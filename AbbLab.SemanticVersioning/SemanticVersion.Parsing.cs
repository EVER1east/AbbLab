﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AbbLab.Parsing;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public partial class SemanticVersion
    {
        [Pure] public static SemanticVersion Parse(string text)
            => Parse(text.AsSpan());
        [Pure] public static SemanticVersion Parse(ReadOnlySpan<char> text)
        {
            SemanticErrorCode code = StrictParse(text, out SemanticVersion? version);
            if (code != SemanticErrorCode.Success) throw new ArgumentException(code.GetErrorMessage(), nameof(text));
            return version!;
        }
        [Pure] public static bool TryParse(string text, [NotNullWhen(true)] out SemanticVersion? version)
            => TryParse(text.AsSpan(), out version);
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out SemanticVersion? version)
        {
            SemanticErrorCode code = StrictParse(text, out SemanticVersion? result);
            if (code != SemanticErrorCode.Success) return Util.Fail(out version);
            version = result!;
            return true;
        }

        private static SemanticErrorCode StrictParse(ReadOnlySpan<char> text, out SemanticVersion? version)
        {
            int length = text.Length;
            int position = 0;
            while (position < length && Util.IsDigit(text[position]))
                position++;
            if (position == 0) return Util.Fail(SemanticErrorCode.MajorNotFound, out version);

            ReadOnlySpan<char> majorSpan = text[..position];
            if (majorSpan[0] is '0' && majorSpan.Length > 1)
                return Util.Fail(SemanticErrorCode.MajorLeadingZeroes, out version);
            if (!Util.TryParse(majorSpan, out int major))
                return Util.Fail(SemanticErrorCode.MajorTooBig, out version);

            if (position >= length || text[position] != '.') return Util.Fail(SemanticErrorCode.MinorNotFound, out version);
            int minorStart = ++position;
            while (position < length && Util.IsDigit(text[position]))
                position++;
            if (position == minorStart) return Util.Fail(SemanticErrorCode.MinorNotFound, out version);
            ReadOnlySpan<char> minorSpan = text[minorStart..position];
            if (minorSpan[0] is '0' && minorSpan.Length > 1)
                return Util.Fail(SemanticErrorCode.MinorLeadingZeroes, out version);
            if (!Util.TryParse(minorSpan, out int minor))
                return Util.Fail(SemanticErrorCode.MinorTooBig, out version);

            if (position >= length || text[position] != '.') return Util.Fail(SemanticErrorCode.PatchNotFound, out version);
            int patchStart = ++position;
            while (position < length && Util.IsDigit(text[position]))
                position++;
            if (position == patchStart) return Util.Fail(SemanticErrorCode.PatchNotFound, out version);
            ReadOnlySpan<char> patchSpan = text[patchStart..position];
            if (patchSpan[0] is '0' && patchSpan.Length > 1)
                return Util.Fail(SemanticErrorCode.PatchLeadingZeroes, out version);
            if (!Util.TryParse(patchSpan, out int patch))
                return Util.Fail(SemanticErrorCode.PatchTooBig, out version);

            SemanticPreRelease[]? preReleases;
            if (position < length && text[position] is '-')
            {
                List<SemanticPreRelease> list = new List<SemanticPreRelease>();
                do
                {
                    int identifierStart = ++position;
                    while (position < length && Util.IsValidCharacter(text[position]))
                        position++;
                    if (position == identifierStart) return Util.Fail(SemanticErrorCode.PreReleaseNotFound, out version);

                    ReadOnlySpan<char> identifier = text[identifierStart..position];
                    SemanticErrorCode code = SemanticPreRelease.ParseInternal(
                        identifier, SemanticOptions.Strict, out SemanticPreRelease preRelease);
                    if (code != SemanticErrorCode.Success) return Util.Fail(code, out version);

                    list.Add(preRelease);
                }
                while (position < length && text[position] is '.');
                preReleases = list.ToArray();
            }
            else preReleases = null;

            string[]? buildMetadata;
            if (position < length && text[position] is '+')
            {
                List<string> list = new List<string>();
                do
                {
                    int identifierStart = ++position;
                    while (position < length && Util.IsValidCharacter(text[position]))
                        position++;
                    if (position == identifierStart) return Util.Fail(SemanticErrorCode.PreReleaseNotFound, out version);

                    list.Add(new string(text[identifierStart..position]));
                }
                while (position < length && text[position] is '.');
                buildMetadata = list.ToArray();
            }
            else buildMetadata = null;

            if (position < length) return Util.Fail(SemanticErrorCode.Leftovers, out version);
            version = new SemanticVersion(major, minor, patch, preReleases, buildMetadata);
            return SemanticErrorCode.Success;
        }

        [Pure] public static SemanticVersion Parse(string text, SemanticOptions options)
            => Parse(text.AsSpan(), options);
        [Pure] public static SemanticVersion Parse(ReadOnlySpan<char> text, SemanticOptions options)
        {
            if (options == SemanticOptions.Strict) return Parse(text);

            StringParser parser = new StringParser(text);

            if ((options & SemanticOptions.AllowLeadingWhite) is not 0)
                parser.SkipWhitespaces();

            SemanticErrorCode code = ParseInternal(ref parser, options, out SemanticVersion? version);
            if (code != SemanticErrorCode.Success) throw new ArgumentException(code.GetErrorMessage(), nameof(text));

            if ((options & SemanticOptions.AllowTrailingWhite) is not 0)
                parser.SkipWhitespaces();
            if (parser.CanRead() && (options & SemanticOptions.AllowLeftovers) is 0)
                throw new ArgumentException($"Encountered an unexpected character at index {parser.Position}.", nameof(text));

            return version!;
        }
        [Pure] public static bool TryParse(string text, SemanticOptions options, [NotNullWhen(true)] out SemanticVersion? version)
            => TryParse(text.AsSpan(), options, out version);
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, SemanticOptions options, [NotNullWhen(true)] out SemanticVersion? version)
        {
            if (options == SemanticOptions.Strict) return TryParse(text, out version);

            StringParser parser = new StringParser(text);

            if ((options & SemanticOptions.AllowLeadingWhite) is not 0)
                parser.SkipWhitespaces();

            SemanticErrorCode code = ParseInternal(ref parser, options, out SemanticVersion? result);
            if (code != SemanticErrorCode.Success) return Util.Fail(out version);

            if ((options & SemanticOptions.AllowTrailingWhite) is not 0)
                parser.SkipWhitespaces();
            if (parser.CanRead() && (options & SemanticOptions.AllowLeftovers) is 0)
                return Util.Fail(out version);

            version = result!;
            return true;
        }

        internal static unsafe SemanticErrorCode ParseInternal(ref StringParser parser, SemanticOptions options,
                                                               out SemanticVersion? version)
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

            ReadOnlySpan<char> majorSpan = parser.ReadWhile(&Util.IsDigit);
            if (majorSpan.Length is 0) return Util.Fail(SemanticErrorCode.MajorNotFound, out version);
            if (majorSpan[0] is '0' && majorSpan.Length > 1 && noLeadingZeroes)
                return Util.Fail(SemanticErrorCode.MajorLeadingZeroes, out version);
            if (!Util.TryParse(majorSpan, out int major))
                return Util.Fail(SemanticErrorCode.MajorTooBig, out version);
            if (innerWhite) parser.SkipWhitespaces();

            int minor, patch;
            if (parser.Skip('.'))
            {
                if (innerWhite) parser.SkipWhitespaces();

                ReadOnlySpan<char> minorSpan = parser.ReadWhile(&Util.IsDigit);
                if (minorSpan.Length is 0) return Util.Fail(SemanticErrorCode.MinorNotFound, out version);
                if (minorSpan[0] is '0' && minorSpan.Length > 1 && noLeadingZeroes)
                    return Util.Fail(SemanticErrorCode.MinorLeadingZeroes, out version);
                if (!Util.TryParse(minorSpan, out minor))
                    return Util.Fail(SemanticErrorCode.MinorTooBig, out version);

                if (innerWhite) parser.SkipWhitespaces();
                if (parser.Skip('.'))
                {
                    if (innerWhite) parser.SkipWhitespaces();

                    ReadOnlySpan<char> patchSpan = parser.ReadWhile(&Util.IsDigit);
                    if (patchSpan.Length is 0) return Util.Fail(SemanticErrorCode.PatchNotFound, out version);
                    if (patchSpan[0] is '0' && patchSpan.Length > 1 && noLeadingZeroes)
                        return Util.Fail(SemanticErrorCode.PatchLeadingZeroes, out version);
                    if (!Util.TryParse(patchSpan, out patch))
                        return Util.Fail(SemanticErrorCode.PatchTooBig, out version);

                    if (innerWhite) parser.SkipWhitespaces();
                }
                else if ((options & SemanticOptions.OptionalPatch) is 0)
                    return Util.Fail(SemanticErrorCode.PatchNotFound, out version);
                else patch = 0;
            }
            else if ((options & SemanticOptions.OptionalMinor) is 0)
                return Util.Fail(SemanticErrorCode.MinorNotFound, out version);
            else
            {
                minor = 0;
                patch = 0;
            }

            SemanticPreRelease[]? preReleases;
            if (parser.Skip('-'))
            {
                List<SemanticPreRelease> list = new List<SemanticPreRelease>();
                do
                {
                    ReadOnlySpan<char> identifier = parser.ReadWhile(&Util.IsValidCharacter);
                    if (identifier.Length is 0) return Util.Fail(SemanticErrorCode.PreReleaseNotFound, out version);
                    SemanticErrorCode code = SemanticPreRelease.ParseInternal(identifier, options, out SemanticPreRelease preRelease);
                    if (code != SemanticErrorCode.Success) return Util.Fail(code, out version);
                    list.Add(preRelease);
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
            else preReleases = null;

            string[]? buildMetadata;
            if (parser.Skip('+'))
            {
                List<string> list = new List<string>();
                do
                {
                    string identifier = parser.ReadStringWhile(&Util.IsValidCharacter);
                    if (identifier.Length is 0) return Util.Fail(SemanticErrorCode.BuildMetadataNotFound, out version);
                    list.Add(identifier);
                }
                while (parser.Skip('.'));
                buildMetadata = list.ToArray();
            }
            else buildMetadata = null;

            version = new SemanticVersion(major, minor, patch, preReleases, buildMetadata);
            return SemanticErrorCode.Success;
        }

    }
}
