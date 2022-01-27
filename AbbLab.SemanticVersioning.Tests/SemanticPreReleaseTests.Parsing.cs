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
            // Strict Parsing (string)
            {
                bool success = SemanticPreRelease.TryParse(test.Input, out SemanticPreRelease preRelease);
                Assert.Equal(test.IsValid, success);
                if (success) Util.AssertPreRelease(preRelease, test.Value);
            }
            {
                bool success = SemanticPreRelease.TryParse(test.Input, SemanticOptions.Strict, out SemanticPreRelease preRelease);
                Assert.Equal(test.IsValid, success);
                if (success) Util.AssertPreRelease(preRelease, test.Value);
            }
            if (test.IsValid)
            {
                SemanticPreRelease preRelease = SemanticPreRelease.Parse(test.Input);
                Assert.Equal(test.PreRelease, preRelease);
                Util.AssertPreRelease(preRelease, test.Value);
            }
            else Assert.Throws<ArgumentException>(() => SemanticPreRelease.Parse(test.Input));
            if (test.IsValid)
            {
                SemanticPreRelease preRelease = SemanticPreRelease.Parse(test.Input, SemanticOptions.Strict);
                Assert.Equal(test.PreRelease, preRelease);
                Util.AssertPreRelease(preRelease, test.Value);
            }
            else Assert.Throws<ArgumentException>(() => SemanticPreRelease.Parse(test.Input, SemanticOptions.Strict));

            // Strict Parsing (span)
            {
                bool success = SemanticPreRelease.TryParse(test.Input.AsSpan(), out SemanticPreRelease preRelease);
                Assert.Equal(test.IsValid, success);
                if (success) Util.AssertPreRelease(preRelease, test.Value, false);
            }
            {
                bool success = SemanticPreRelease.TryParse(test.Input.AsSpan(), SemanticOptions.Strict, out SemanticPreRelease preRelease);
                Assert.Equal(test.IsValid, success);
                if (success) Util.AssertPreRelease(preRelease, test.Value, false);
            }
            if (test.IsValid)
            {
                SemanticPreRelease preRelease = SemanticPreRelease.Parse(test.Input.AsSpan());
                Assert.Equal(test.PreRelease, preRelease);
                Util.AssertPreRelease(preRelease, test.Value, false);
            }
            else Assert.Throws<ArgumentException>(() => SemanticPreRelease.Parse(test.Input.AsSpan()));
            if (test.IsValid)
            {
                SemanticPreRelease preRelease = SemanticPreRelease.Parse(test.Input.AsSpan(), SemanticOptions.Strict);
                Assert.Equal(test.PreRelease, preRelease);
                Util.AssertPreRelease(preRelease, test.Value, false);
            }
            else Assert.Throws<ArgumentException>(() => SemanticPreRelease.Parse(test.Input.AsSpan(), SemanticOptions.Strict));

            // Loose Parsing (string)
            {
                bool success = SemanticPreRelease.TryParse(test.Input, SemanticOptions.Loose, out SemanticPreRelease preRelease);
                Assert.Equal(test.IsValidLoose, success);
                if (success) Util.AssertPreRelease(preRelease, test.Value);
            }
            if (test.IsValidLoose)
            {
                SemanticPreRelease preRelease = SemanticPreRelease.Parse(test.Input, SemanticOptions.Loose);
                Assert.Equal(test.PreRelease, preRelease);
                Util.AssertPreRelease(preRelease, test.Value);
            }
            else Assert.Throws<ArgumentException>(() => SemanticPreRelease.Parse(test.Input, SemanticOptions.Loose));

            // Loose Parsing (span)
            {
                bool success = SemanticPreRelease.TryParse(test.Input.AsSpan(), SemanticOptions.Loose, out SemanticPreRelease preRelease);
                Assert.Equal(test.IsValidLoose, success);
                if (success) Util.AssertPreRelease(preRelease, test.Value, false);
            }
            if (test.IsValidLoose)
            {
                SemanticPreRelease preRelease = SemanticPreRelease.Parse(test.Input.AsSpan(), SemanticOptions.Loose);
                Assert.Equal(test.PreRelease, preRelease);
                Util.AssertPreRelease(preRelease, test.Value, false);
            }
            else Assert.Throws<ArgumentException>(() => SemanticPreRelease.Parse(test.Input.AsSpan(), SemanticOptions.Loose));

        }

        private static ParseFixture Strict(string input, int value)
            => new ParseFixture(input, true, true, value);
        private static ParseFixture Strict(string input, string value)
            => new ParseFixture(input, true, true, value);
        private static ParseFixture Loose(string input, int value)
            => new ParseFixture(input, false, true, value);
        private static ParseFixture Loose(string input, string value)
            => new ParseFixture(input, false, true, value);
        private static ParseFixture Invalid(string input)
            => new ParseFixture(input, false, false, default);

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
            public SemanticPreRelease PreRelease { get; }
            public object Value { get; }

            public ParseFixture(string input, bool isValid, bool isValidLoose, SemanticPreRelease preRelease)
            {
                Input = input;
                IsValid = isValid;
                IsValidLoose = isValidLoose;
                PreRelease = preRelease;
                Value = preRelease.IsNumeric ? preRelease.Number : preRelease.Text;
            }

        }

    }
}
