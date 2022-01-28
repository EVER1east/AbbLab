using System;
using System.Collections.Generic;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticPreReleaseTests
    {
        [Theory]
        [MemberData(nameof(ParseFixtures))]
        public void ParseTests(ParseFixture test)
        {
            Output.WriteLine($"Parsing `{test.Input}` pre-release identifier.");
            const SemanticOptions pseudoStrictMode = (SemanticOptions)int.MinValue;

            // Strict TryParse
            AssertEx.Identical(test.IsValid, new Parser<SemanticPreRelease>[]
            {
                (out SemanticPreRelease p) => SemanticPreRelease.TryParse(test.Input, out p),
                (out SemanticPreRelease p) => SemanticPreRelease.TryParse(test.Input, SemanticOptions.Strict, out p),
                (out SemanticPreRelease p) => SemanticPreRelease.TryParse(test.Input, pseudoStrictMode, out p),
            }, p => AssertEx.PreRelease(p, test.Value));
            // String TryParse (span)
            AssertEx.Identical(test.IsValid, new Parser<SemanticPreRelease>[]
            {
                (out SemanticPreRelease p) => SemanticPreRelease.TryParse(test.Input.AsSpan(), out p),
                (out SemanticPreRelease p) => SemanticPreRelease.TryParse(test.Input.AsSpan(), SemanticOptions.Strict, out p),
                (out SemanticPreRelease p) => SemanticPreRelease.TryParse(test.Input.AsSpan(), pseudoStrictMode, out p),
            }, p => AssertEx.PreRelease(p, test.Value, true));

            // Strict Parse
            AssertEx.Identical<SemanticPreRelease, ArgumentException>(test.IsValid, new Func<SemanticPreRelease>[]
            {
                () => SemanticPreRelease.Parse(test.Input),
                () => SemanticPreRelease.Parse(test.Input, SemanticOptions.Strict),
                () => SemanticPreRelease.Parse(test.Input, pseudoStrictMode),
                () => new SemanticPreRelease(test.Input),
                () => new SemanticPreRelease(test.Input, SemanticOptions.Strict),
                () => new SemanticPreRelease(test.Input, pseudoStrictMode),
                () => test.Input, // implicit
            }, p => AssertEx.PreRelease(p, test.Value));
            // Strict Parse (span)
            AssertEx.Identical<SemanticPreRelease, ArgumentException>(test.IsValid, new Func<SemanticPreRelease>[]
            {
                () => SemanticPreRelease.Parse(test.Input.AsSpan()),
                () => SemanticPreRelease.Parse(test.Input.AsSpan(), SemanticOptions.Strict),
                () => SemanticPreRelease.Parse(test.Input.AsSpan(), pseudoStrictMode),
                () => new SemanticPreRelease(test.Input.AsSpan()),
                () => new SemanticPreRelease(test.Input.AsSpan(), SemanticOptions.Strict),
                () => new SemanticPreRelease(test.Input.AsSpan(), pseudoStrictMode),
                () => test.Input.AsSpan(), // implicit
            }, p => AssertEx.PreRelease(p, test.Value, true));

            // Loose TryParse
            AssertEx.Identical(test.IsValidLoose, new Parser<SemanticPreRelease>[]
            {
                (out SemanticPreRelease p) => SemanticPreRelease.TryParse(test.Input.AsSpan(), SemanticOptions.Loose, out p),
            }, p => AssertEx.PreRelease(p, test.Value, true));
            // Loose Parse
            AssertEx.Identical<SemanticPreRelease, ArgumentException>(test.IsValidLoose, new Func<SemanticPreRelease>[]
            {
                () => SemanticPreRelease.Parse(test.Input.AsSpan(), SemanticOptions.Loose),
                () => new SemanticPreRelease(test.Input.AsSpan(), SemanticOptions.Loose),
            }, p => AssertEx.PreRelease(p, test.Value, true));

        }

        private static ParseFixture Strict(string input, object value)
            => new ParseFixture(input, true, true, value);
        private static ParseFixture Loose(string input, object value)
            => new ParseFixture(input, false, true, value);
        private static ParseFixture Invalid(string input)
            => new ParseFixture(input, false, false, null);

        public static readonly IEnumerable<object[]> ParseFixtures = Util.Arrayify(new ParseFixture[]
        {
            Strict("0", 0),
            Loose("00", 0),
            Loose("0000", 0),
            Strict("1", 1),
            Strict("2", 2),
            Strict("12", 12),
            Strict("123", 123),
            Loose("0123", 123),
            Strict("-0123", "-0123"),
            Strict("00-", "00-"),
            Strict("00a", "00a"),
            Strict("01-23", "01-23"),
            Strict("00-120--", "00-120--"),
            Strict("alpha", "alpha"),
            Strict("alpha7beta", "alpha7beta"),
            Strict("--alpha-beta-", "--alpha-beta-"),
            Invalid(""),
            Invalid("$$"),
            Strict("2147483647", 2147483647),
            Strict("-2147483648", "-2147483648"),
            Invalid("2147483648"),
        });

        public readonly struct ParseFixture
        {
            public string Input { get; }
            public bool IsValid { get; }
            public bool IsValidLoose { get; }
            public object? Value { get; }

            public ParseFixture(string input, bool isValid, bool isValidLoose, object? value)
            {
                Input = input;
                IsValid = isValid;
                IsValidLoose = isValidLoose;
                Value = value;
            }

        }

    }
}
