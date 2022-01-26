using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AbbLab.SemanticVersioning.Tests
{
    public class SemanticPreReleaseTests
    {
        private readonly ITestOutputHelper Output;
        public SemanticPreReleaseTests(ITestOutputHelper output) => Output = output;

        private static void AssertPreRelease(SemanticPreRelease preRelease, int number)
        {
            Assert.True(preRelease.IsNumeric);
            Assert.Equal(preRelease.Number, number);
            Assert.Equal((int)preRelease, number);
            Assert.Throws<InvalidOperationException>(() => preRelease.Text);
        }
        private static void AssertPreRelease(SemanticPreRelease preRelease, string text)
        {
            Assert.False(preRelease.IsNumeric);
            Assert.Equal(preRelease.Text, text);
            Assert.Equal((string)preRelease, text);
            Assert.Throws<InvalidOperationException>(() => preRelease.Number);
        }
        private static void AssertPreRelease(SemanticPreRelease preRelease, object value)
        {
            if (value is string str) AssertPreRelease(preRelease, str);
            else AssertPreRelease(preRelease, (int)value);
        }

        [Theory]
        [MemberData(nameof(ParseFixture))]
        public void ParseTests(PreReleaseTest info)
        {
            Output.WriteLine($"Parsing `{info.Semantic}` pre-release.");
            bool success = SemanticPreRelease.TryParse(info.Semantic, out SemanticPreRelease tryParseResult);
            try
            {
                SemanticPreRelease parseResult = SemanticPreRelease.Parse(info.Semantic);
                SemanticPreRelease ctorResult = new SemanticPreRelease(info.Semantic);
                Assert.True(success, "Parse() parsed what TryParse() couldn't.");
                Assert.True(info.IsValid, "Successfully parsed an invalid pre-release.");
                AssertPreRelease(tryParseResult, info.Value);

                AssertPreRelease(parseResult, info.Value);
                if (info.Value is string) // no need to allocate a new string
                    Assert.Same(info.Value, parseResult.Text);

                AssertPreRelease(ctorResult, info.Value);
                if (info.Value is string) // no need to allocate a new string
                    Assert.Same(info.Value, ctorResult.Text);
            }
            catch (ArgumentException)
            {
                Assert.False(success);
                Assert.False(info.IsValid, "Could not parse a valid pre-release.");
            }
            const SemanticOptions pseudoStrictMode = (SemanticOptions)int.MinValue;
            success = SemanticPreRelease.TryParse(info.Semantic, pseudoStrictMode, out tryParseResult);
            try
            {
                SemanticPreRelease parseResult = SemanticPreRelease.Parse(info.Semantic, pseudoStrictMode);
                SemanticPreRelease ctorResult = new SemanticPreRelease(info.Semantic, pseudoStrictMode);
                Assert.True(success, "Parse() parsed what TryParse() couldn't.");
                Assert.True(info.IsValid, "Successfully parsed an invalid pre-release.");
                AssertPreRelease(tryParseResult, info.Value);

                AssertPreRelease(parseResult, info.Value);
                if (info.Value is string) // no need to allocate a new string
                    Assert.Same(info.Value, parseResult.Text);

                AssertPreRelease(ctorResult, info.Value);
                if (info.Value is string) // no need to allocate a new string
                    Assert.Same(info.Value, ctorResult.Text);
            }
            catch (ArgumentException)
            {
                Assert.False(success);
                Assert.False(info.IsValid, "Could not parse a valid pre-release.");
            }
        }

        [Theory]
        [MemberData(nameof(ParseFixture))]
        public void LooseParseTests(PreReleaseTest info)
        {
            Output.WriteLine($"Loosely parsing `{info.Semantic}`.");
            bool success = SemanticPreRelease.TryParse(info.Semantic, SemanticOptions.Loose, out SemanticPreRelease tryParseResult);
            try
            {
                SemanticPreRelease parseResult = SemanticPreRelease.Parse(info.Semantic, SemanticOptions.Loose);
                SemanticPreRelease ctorResult = new SemanticPreRelease(info.Semantic, SemanticOptions.Loose);
                Assert.True(success, "Parse() loosely parsed what TryParse() couldn't.");
                Assert.True(info.IsValidLoose, "Successfully loosely parsed an invalid pre-release.");
                AssertPreRelease(tryParseResult, info.Value);

                AssertPreRelease(parseResult, info.Value);
                if (info.Value is string) // no need to allocate a new string
                    Assert.Same(info.Value, parseResult.Text);

                AssertPreRelease(ctorResult, info.Value);
                if (info.Value is string) // no need to allocate a new string
                    Assert.Same(info.Value, ctorResult.Text);
            }
            catch (ArgumentException)
            {
                Assert.False(success);
                Assert.False(info.IsValidLoose, "Could not loosely parse a valid pre-release.");
            }
        }

        [Theory]
        [MemberData(nameof(ParseFixture))]
        public void ParseTestsWithSpan(PreReleaseTest info)
        {
            Output.WriteLine($"Parsing `{info.Semantic}`.");
            bool success = SemanticPreRelease.TryParse(info.Semantic.AsSpan(), out SemanticPreRelease tryParseResult);
            try
            {
                SemanticPreRelease parseResult = SemanticPreRelease.Parse(info.Semantic.AsSpan());
                SemanticPreRelease ctorResult = new SemanticPreRelease(info.Semantic.AsSpan());
                Assert.True(success, "Parse() parsed what TryParse() couldn't.");
                Assert.True(info.IsValid, "Successfully parsed an invalid pre-release.");
                AssertPreRelease(tryParseResult, info.Value);

                AssertPreRelease(parseResult, info.Value);
                if (info.Value is string) // has to allocate a new string
                    Assert.NotSame(info.Value, parseResult.Text);

                AssertPreRelease(ctorResult, info.Value);
                if (info.Value is string) // has to allocate a new string
                    Assert.NotSame(info.Value, ctorResult.Text);
            }
            catch (ArgumentException)
            {
                Assert.False(success);
                Assert.False(info.IsValid, "Could not parse a valid pre-release.");
            }
        }

        [Theory]
        [MemberData(nameof(ParseFixture))]
        public void LooseParseTestsWithSpan(PreReleaseTest info)
        {
            Output.WriteLine($"Loosely parsing `{info.Semantic}`.");
            bool success = SemanticPreRelease.TryParse(info.Semantic.AsSpan(), SemanticOptions.Loose, out SemanticPreRelease tryParseResult);
            try
            {
                SemanticPreRelease parseResult = SemanticPreRelease.Parse(info.Semantic.AsSpan(), SemanticOptions.Loose);
                SemanticPreRelease ctorResult = new SemanticPreRelease(info.Semantic.AsSpan(), SemanticOptions.Loose);
                Assert.True(success, "Parse() loosely parsed what TryParse() couldn't.");
                Assert.True(info.IsValidLoose, "Successfully loosely parsed an invalid pre-release.");
                AssertPreRelease(tryParseResult, info.Value);

                AssertPreRelease(parseResult, info.Value);
                if (info.Value is string) // has to allocate a new string
                    Assert.NotSame(info.Value, parseResult.Text);

                AssertPreRelease(ctorResult, info.Value);
                if (info.Value is string) // has to allocate a new string
                    Assert.NotSame(info.Value, ctorResult.Text);
            }
            catch (ArgumentException)
            {
                Assert.False(success);
                Assert.False(info.IsValidLoose, "Could not loosely parse a valid pre-release.");
            }
        }

        [Fact]
        public void ConstructorTest()
        {
            AssertPreRelease(default, 0);
            AssertPreRelease(new SemanticPreRelease(), 0);
            AssertPreRelease(new SemanticPreRelease(0), 0);
            AssertPreRelease(new SemanticPreRelease(123), 123);
            AssertPreRelease(123, 123);
            AssertPreRelease(new SemanticPreRelease(2147483647), 2147483647);
            AssertPreRelease(2147483647, 2147483647);

            Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticPreRelease(-1));
            Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticPreRelease(-123));

            // string constructors are tested in ParseTests
            AssertPreRelease("0alpha-", "0alpha-");
            AssertPreRelease("0alpha-".AsSpan(), "0alpha-");

            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease(""));
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("$$$"));

        }

        public static readonly IEnumerable<object[]> ParseFixture = Util.Arrayify(new PreReleaseTest[]
        {
            new PreReleaseTest("0", 0),
            new PreReleaseTest("00", 0, true),
            new PreReleaseTest("0000", 0, true),
            new PreReleaseTest("1", 1),
            new PreReleaseTest("2", 2),
            new PreReleaseTest("12", 12),
            new PreReleaseTest("123", 123),
            new PreReleaseTest("0123", 123, true),
            new PreReleaseTest("-0123", "-0123"),
            new PreReleaseTest("00-", "00-"),
            new PreReleaseTest("00a", "00a"),
            new PreReleaseTest("01-23", "01-23"),
            new PreReleaseTest("00-120--", "00-120--"),
            new PreReleaseTest("alpha", "alpha"),
            new PreReleaseTest("alpha7beta", "alpha7beta"),
            new PreReleaseTest("--alpha-beta-", "--alpha-beta-"),
            new PreReleaseTest(""),
            new PreReleaseTest("$$"),
            new PreReleaseTest("2147483647", 2147483647),
            new PreReleaseTest("2147483648"),
            new PreReleaseTest("-2147483648", "-2147483648"),
        });

        [Fact]
        public void ComparisonTest()
        {
            List<SemanticPreRelease> preReleases = SortFixture.Select(SemanticPreRelease.Parse).ToList();
            List<SemanticPreRelease> sorted = new List<SemanticPreRelease>(preReleases);
            sorted.Sort();
            Assert.Equal(preReleases, sorted);

            int count = preReleases.Count;
            for (int i = 0; i < count; i++)
            {
                SemanticPreRelease me = preReleases[i];
                for (int j = 0; j < i; j++)
                {
                    SemanticPreRelease other = preReleases[j];

                    Assert.True(me.CompareTo(other) > 0);
                    Assert.True(((IComparable)me).CompareTo(other) > 0);
                    Assert.False(me.Equals(other));
                    Assert.False(me.Equals((object?)other));

                    Assert.True(me > other);
                    Assert.True(me >= other);
                    Assert.True(other < me);
                    Assert.True(other <= me);
                    Assert.True(me != other);
                    Assert.False(me == other);
                }
                for (int j = i + 1; j < count; j++)
                {
                    // less than any of the subsequent versions
                    SemanticPreRelease other = preReleases[j];

                    Assert.True(other.CompareTo(me) > 0);
                    Assert.True(((IComparable)other).CompareTo(me) > 0);
                    Assert.False(other.Equals(me));
                    Assert.False(other.Equals((object?)me));

                    Assert.True(other > me);
                    Assert.True(other >= me);
                    Assert.True(me < other);
                    Assert.True(me <= other);
                    Assert.True(me != other);
                    Assert.False(me == other);
                }
                // equal to self
                Assert.True(me.CompareTo(me) == 0);
                Assert.True(((IComparable)me).CompareTo(me) == 0);
                Assert.True(me.Equals(me));
                Assert.True(me.Equals((object)me));
                Assert.Equal(me.GetHashCode(), me.GetHashCode());

                Assert.Throws<ArgumentException>(() => ((IComparable)me).CompareTo(0.0));

#pragma warning disable CS1718 // Comparison made to same variable
                // ReSharper disable EqualExpressionComparison
                Assert.False(me > me);
                Assert.True(me >= me);
                Assert.False(me < me);
                Assert.True(me <= me);
                Assert.False(me != me);
                Assert.True(me == me);
#pragma warning restore CS1718 // Comparison made to same variable
                // ReSharper restore EqualExpressionComparison

            }
        }

        public static readonly string[] SortFixture =
        {
            "0",
            "1",
            "2",
            "10",
            "100",
            "2147483647",
            "a",
            "alpha",
            "b",
            "beta",
            "dev",
            "nightly",
            "rc",
            "zzz",
        };
    }
    public struct PreReleaseTest
    {
        public string Semantic { get; }
        public object Value { get; }
        public bool IsValid { get; }
        public bool IsValidLoose { get; }

        public PreReleaseTest(string semantic, int number, bool looseOnly = false)
        {
            Semantic = semantic;
            Value = number;
            IsValid = !looseOnly;
            IsValidLoose = true;
        }
        public PreReleaseTest(string semantic, string value, bool looseOnly = false)
        {
            Semantic = semantic;
            Value = value;
            IsValid = !looseOnly;
            IsValidLoose = true;
        }
        public PreReleaseTest(string semantic)
        {
            Semantic = semantic;
            Value = null!;
            IsValid = false;
            IsValidLoose = false;
        }

    }
}
