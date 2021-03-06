using System;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    internal static class Util
    {
        public static bool IsDigit(int character) => character is >= '0' and <= '9';
        public static bool IsAllDigits(string identifier)
        {
            int length = identifier.Length;
            for (int i = 0; i < length; i++)
                if (identifier[i] is < '0' or > '9')
                    return false;
            return true;
        }
        public static bool IsAllDigits(ReadOnlySpan<char> identifier)
        {
            int length = identifier.Length;
            for (int i = 0; i < length; i++)
                if (identifier[i] is < '0' or > '9')
                    return false;
            return true;
        }
        public static bool IsLetter(int character) => character is >= 'A' and <= 'Z' or >= 'a' and <= 'z';

        public static bool IsValidCharacter(int character)
            => character is >= '0' and <= '9' or >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '-';
        public static bool ContainsOnlyValidCharacters(string identifier)
        {
            int length = identifier.Length;
            for (int i = 0; i < length; i++)
                if (!IsValidCharacter(identifier[i]))
                    return false;
            return true;
        }
        public static bool ContainsOnlyValidCharacters(ReadOnlySpan<char> identifier)
        {
            int length = identifier.Length;
            for (int i = 0; i < length; i++)
                if (!IsValidCharacter(identifier[i]))
                    return false;
            return true;
        }

        public static ReadOnlySpan<char> Trim(ReadOnlySpan<char> span, SemanticOptions options)
        {
            const SemanticOptions whiteMask = SemanticOptions.AllowLeadingWhite | SemanticOptions.AllowTrailingWhite;
            return (options & whiteMask) switch
            {
                whiteMask => span.Trim(),
                SemanticOptions.AllowLeadingWhite => span.TrimStart(),
                SemanticOptions.AllowTrailingWhite => span.TrimEnd(),
                _ => span,
            };
        }
        public static bool TryTrim(string text, SemanticOptions options, out ReadOnlySpan<char> result)
        {
            const SemanticOptions whiteMask = SemanticOptions.AllowLeadingWhite | SemanticOptions.AllowTrailingWhite;
            ReadOnlySpan<char> span;
            switch (options & whiteMask)
            {
                case whiteMask:
                    span = text.AsSpan().Trim();
                    break;
                case SemanticOptions.AllowLeadingWhite:
                    span = text.AsSpan().TrimStart();
                    break;
                case SemanticOptions.AllowTrailingWhite:
                    span = text.AsSpan().TrimEnd();
                    break;
                default:
                    result = default;
                    return false;
            }
            if (span.Length == text.Length)
            {
                result = default;
                return false;
            }
            result = span;
            return true;
        }

        public static bool TryParse(string identifier, out int result)
        {
            int length = identifier.Length;
            int res = 0;
            for (int i = 0; i < length; i++)
            {
                res = res * 10 + (identifier[i] - '0');
                if (res < 0) return Fail(out result);
            }
            result = res;
            return true;
        }
        public static bool TryParse(ReadOnlySpan<char> identifier, out int result)
        {
            int length = identifier.Length;
            int res = 0;
            for (int i = 0; i < length; i++)
            {
                res = res * 10 + (identifier[i] - '0');
                if (res < 0) return Fail(out result);
            }
            result = res;
            return true;
        }

        private static int CountDigits(int number) => number switch
        {
            < 10 => 1,
            < 100 => 2,
            < 1000 => 3,
            < 10000 => 4,
            < 100000 => 5,
            < 1000000 => 6,
            < 10000000 => 7,
            < 100000000 => 8,
            < 1000000000 => 9,
            _ => 10,
        };
        public static string ToString(int number)
        {
            int digits = CountDigits(number);
            return string.Create(digits, number, static (buffer, number) =>
            {
                for (int i = buffer.Length - 1; i >= 0; i--)
                {
                    int div = number / 10;
                    buffer[i] = (char)('0' + (number - div * 10));
                    number = div;
                }
            });
        }
        public static StringBuilder SimpleAppend(this StringBuilder sb, int number)
        {
            int digits = CountDigits(number);
            Span<char> buffer = stackalloc char[digits];
            for (int i = digits - 1; i >= 0; i--)
            {
                int div = number / 10;
                buffer[i] = (char)('0' + (number - div * 10));
                number = div;
            }
            return sb.Append(buffer);
        }
        public static StringBuilder SimpleAppend(this StringBuilder sb, SemanticPreRelease preRelease)
            => preRelease.text is null ? sb.SimpleAppend(preRelease.number) : sb.Append(preRelease.text);
        public static StringBuilder SimpleAppend(this StringBuilder sb, PartialComponent partialComponent)
        {
            if (partialComponent.IsNumeric)
                return sb.SimpleAppend(partialComponent.Value);
            return sb.Append(partialComponent.GetWildcardCharacter());
        }

        [MustUseReturnValue] public static bool Fail<TResult>(out TResult? result)
        {
            result = default;
            return false;
        }
        [MustUseReturnValue] public static TReturn Fail<TReturn, TResult>(TReturn returnValue, out TResult? result)
        {
            result = default;
            return returnValue;
        }

        public static string GetErrorMessage(this SemanticErrorCode code) => code switch
        {
            SemanticErrorCode.Success => "Success. You shouldn't see this message.",

            SemanticErrorCode.MajorNotFound => Exceptions.MajorNotFound,
            SemanticErrorCode.MajorLeadingZeroes => Exceptions.MajorLeadingZeroes,
            SemanticErrorCode.MajorTooBig => Exceptions.MajorTooBig,

            SemanticErrorCode.MinorNotFound => Exceptions.MinorNotFound,
            SemanticErrorCode.MinorLeadingZeroes => Exceptions.MinorLeadingZeroes,
            SemanticErrorCode.MinorTooBig => Exceptions.MinorTooBig,

            SemanticErrorCode.PatchNotFound => Exceptions.PatchNotFound,
            SemanticErrorCode.PatchLeadingZeroes => Exceptions.PatchLeadingZeroes,
            SemanticErrorCode.PatchTooBig => Exceptions.PatchTooBig,

            SemanticErrorCode.PreReleaseNotFound => Exceptions.PreReleaseNotFound,
            SemanticErrorCode.PreReleaseLeadingZeroes => Exceptions.PreReleaseLeadingZeroes,
            SemanticErrorCode.PreReleaseTooBig => Exceptions.PreReleaseTooBig,

            SemanticErrorCode.BuildMetadataNotFound => Exceptions.BuildMetadataNotFound,

            SemanticErrorCode.Leftovers => Exceptions.EncounteredInvalidCharacter,

            _ => throw new ArgumentException($"Invalid {nameof(SemanticErrorCode)}.", nameof(code)),
        };
    }
}