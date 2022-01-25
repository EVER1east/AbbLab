using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        private readonly ITestOutputHelper Output;
        public SemanticVersionTests(ITestOutputHelper output) => Output = output;

        [Theory]
        [MemberData(nameof(ParseFixture))]
        public void ParseTests(VersionTest info)
        {
            Output.WriteLine($"Parsing `{info.Semantic}`.");
            bool success = SemanticVersion.TryParse(info.Semantic, out SemanticVersion? tryParseResult);
            bool successWithOptions = SemanticVersion.TryParse(
                info.Semantic, SemanticOptions.Strict, out SemanticVersion? tryParseResultWithOptions);
            Assert.Equal(success, successWithOptions);
            Assert.Equal(tryParseResult, tryParseResultWithOptions, BuildMetadataComparer.Instance!);

            try
            {
                SemanticVersion parseResult = SemanticVersion.Parse(info.Semantic);
                SemanticVersion parseResultWithOptions = SemanticVersion.Parse(info.Semantic, SemanticOptions.Strict);
                Assert.Equal(parseResult, parseResultWithOptions, BuildMetadataComparer.Instance);

                Assert.True(success, "Parse() parsed what TryParse() couldn't.");
                if (!info.IsValid) throw new InvalidOperationException("Successfully parsed an invalid version.");
                Assert.Equal(tryParseResult, parseResult, BuildMetadataComparer.Instance!);
                info.Assert(tryParseResult!);
            }
            catch (ArgumentException)
            {
                Assert.False(success);
                if (info.IsValid) throw;
            }

            // TryParse() uses StrictParse(), if options are Strict, so we're using this flag to bypass that
            const SemanticOptions pseudoStrictMode = (SemanticOptions)int.MinValue;
            success = SemanticVersion.TryParse(info.Semantic, pseudoStrictMode, out tryParseResult);
            try
            {
                SemanticVersion parseResult = SemanticVersion.Parse(info.Semantic, pseudoStrictMode);
                Assert.True(success, "Parse() parsed what TryParse() couldn't.");
                if (!info.IsValid) throw new InvalidOperationException("Successfully parsed an invalid version.");
                Assert.Equal(tryParseResult, parseResult, BuildMetadataComparer.Instance!);
                info.Assert(tryParseResult!);
            }
            catch (ArgumentException)
            {
                Assert.False(success);
                if (info.IsValid) throw;
            }
        }

        [Theory]
        [MemberData(nameof(ParseFixture))]
        public void LooseParseTests(VersionTest info)
        {
            Output.WriteLine($"Loosely parsing `{info.Semantic}`.");
            bool success = SemanticVersion.TryParse(info.Semantic, SemanticOptions.Loose, out SemanticVersion? tryParseResult);
            try
            {
                SemanticVersion parseResult = SemanticVersion.Parse(info.Semantic, SemanticOptions.Loose);
                Assert.True(success, "Parse() loosely parsed what TryParse() couldn't.");
                if (!info.IsValidLoose) throw new InvalidOperationException("Successfully loosely parsed an invalid version.");
                Assert.Equal(tryParseResult, parseResult, BuildMetadataComparer.Instance!);
                info.Assert(tryParseResult!);
            }
            catch (ArgumentException)
            {
                Assert.False(success);
                if (info.IsValidLoose) throw;
            }
        }

        [Theory]
        [MemberData(nameof(ParseFixture))]
        public void ToStringTests(VersionTest info)
        {
            if (info.IsValid)
            {
                Output.WriteLine($"Formatting `{info.Semantic}`.");
                SemanticVersion version = SemanticVersion.Parse(info.Semantic);

                Assert.Equal(info.Semantic, version.ToString());
                Assert.Equal(info.Semantic, version.ToString(null));
                Assert.Equal(info.Semantic, version.ToString("G"));
                Assert.Equal(info.Semantic, version.ToString("g"));
                Assert.Equal(info.Semantic, version.ToString("M.m.p-ppp+mmm"));

                IFormattable format = version;
                Assert.Equal(info.Semantic, format.ToString(null, null));
                Assert.Equal(info.Semantic, format.ToString("G", null));
                Assert.Equal(info.Semantic, format.ToString("g", null));
                Assert.Equal(info.Semantic, format.ToString("M.m.p-ppp+mmm", null));
                Assert.Equal(info.Semantic, format.ToString(null, CultureInfo.InvariantCulture));
                Assert.Equal(info.Semantic, format.ToString("G", CultureInfo.InvariantCulture));
                Assert.Equal(info.Semantic, format.ToString("g", CultureInfo.InvariantCulture));
                Assert.Equal(info.Semantic, format.ToString("M.m.p-ppp+mmm", CultureInfo.InvariantCulture));
            }
        }

        [Theory]
        [MemberData(nameof(FormatFixture))]
        public void ToStringFormatTests(VersionFormatTest info)
        {
            Output.WriteLine($"Formatting `{info.Semantic}` using `{info.Format}`.");
            SemanticVersion version = SemanticVersion.Parse(info.Semantic);

            Assert.Equal(info.Expected, version.ToString(info.Format));
            IFormattable format = version;
            Assert.Equal(info.Expected, format.ToString(info.Format, null));
            Assert.Equal(info.Expected, format.ToString(info.Format, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ComparisonTest()
        {
            List<SemanticVersion?> versions = new List<SemanticVersion?>(
                SortFixture.Select(static v => v is null ? null : SemanticVersion.Parse(v)));
            List<SemanticVersion?> sorted = new List<SemanticVersion?>(versions);
            sorted.Sort();
            Assert.Equal(versions, sorted);

            int count = versions.Count;
            for (int i = 0; i < count; i++)
            {
                SemanticVersion? me = versions[i];
                for (int j = 0; j < i; j++)
                {
                    // greater than any of the previous versions
                    SemanticVersion? other = versions[j];
                    if (me is not null)
                    {
                        Assert.True(me.CompareTo(other) > 0);
                        Assert.True(((IComparable)me).CompareTo(other) > 0);
                        Assert.False(me.Equals(other));
                        Assert.False(me.Equals((object?)other));
                    }
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
                    SemanticVersion? other = versions[j];
                    if (other is not null)
                    {
                        Assert.True(other.CompareTo(me) > 0);
                        Assert.True(((IComparable)other).CompareTo(me) > 0);
                        Assert.False(other.Equals(me));
                        Assert.False(other.Equals((object?)me));
                    }
                    Assert.True(other > me);
                    Assert.True(other >= me);
                    Assert.True(me < other);
                    Assert.True(me <= other);
                    Assert.True(me != other);
                    Assert.False(me == other);
                }
                // equal to self
                if (me is not null)
                {
                    Assert.True(me.CompareTo(me) == 0);
                    Assert.True(((IComparable)me).CompareTo(me) == 0);
                    Assert.True(me.Equals(me));
                    Assert.True(me.Equals((object)me));
                    Assert.Equal(me.GetHashCode(), me.GetHashCode());

                    SemanticVersion clone = SemanticVersion.Parse(SortFixture[i]!);
                    Assert.True(me.CompareTo(clone) == 0);
                    Assert.True(((IComparable)me).CompareTo(clone) == 0);
                    Assert.True(me.Equals(clone));
                    Assert.True(me.Equals((object)clone));
                    Assert.Equal(me.GetHashCode(), clone.GetHashCode());

                    Assert.Throws<ArgumentException>(() => ((IComparable)me).CompareTo(0.0));
                }
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

        [Fact]
        public void MembersTest()
        {
            SemanticVersion version = SemanticVersion.Parse("12.34.56-alpha.8.beta+test-build.011");

            Assert.Null(version._preReleasesReadonly);
            Assert.Same(version.PreReleases, version.PreReleases);

            Assert.Null(version._buildMetadataReadonly);
            Assert.Same(version.BuildMetadata, version.BuildMetadata);
        }

        private static void AssertVersion(SemanticVersion version, int major, int minor, int patch,
                                          IEnumerable<SemanticPreRelease>? preReleases = null, IEnumerable<string>? buildMetadata = null)
        {
            Assert.Equal(major, version.Major);
            Assert.Equal(minor, version.Minor);
            Assert.Equal(patch, version.Patch);
            if (preReleases is null) Assert.Empty(version.PreReleases);
            else Assert.Equal(preReleases, version.PreReleases);
            if (buildMetadata is null) Assert.Empty(version.BuildMetadata);
            else Assert.Equal(buildMetadata, version.BuildMetadata);
        }

        [Fact]
        public void ConstructorTest()
        {
            string[] stringPreReleases = { "alpha", "8", "beta" };
            SemanticPreRelease[] preReleases = { "alpha", 8, "beta" };
            string[] buildMetadata = { "test-build", "003" };

            AssertVersion(new SemanticVersion(12, 34, 56),
                          12, 34, 56);
            AssertVersion(new SemanticVersion(12, 34, 56, stringPreReleases),
                          12, 34, 56, preReleases);
            AssertVersion(new SemanticVersion(12, 34, 56, preReleases),
                          12, 34, 56, preReleases);
            AssertVersion(new SemanticVersion(12, 34, 56, stringPreReleases.AsEnumerable()),
                          12, 34, 56, preReleases);
            AssertVersion(new SemanticVersion(12, 34, 56, preReleases.AsEnumerable()),
                          12, 34, 56, preReleases);
            AssertVersion(new SemanticVersion(12, 34, 56, stringPreReleases.AsEnumerable(), buildMetadata),
                          12, 34, 56, preReleases, buildMetadata);
            AssertVersion(new SemanticVersion(12, 34, 56, preReleases.AsEnumerable(), buildMetadata),
                          12, 34, 56, preReleases, buildMetadata);

            Assert.Throws<ArgumentOutOfRangeException>(
                static () => new SemanticVersion(-1, 0, 0, (IEnumerable<string>?)null));
            Assert.Throws<ArgumentOutOfRangeException>(
                static () => new SemanticVersion(0, -1, 0, (IEnumerable<string>?)null));
            Assert.Throws<ArgumentOutOfRangeException>(
                static () => new SemanticVersion(0, 0, -1, (IEnumerable<string>?)null));

            Assert.Throws<ArgumentOutOfRangeException>(
                static () => new SemanticVersion(-1, 0, 0, (IEnumerable<SemanticPreRelease>?)null));
            Assert.Throws<ArgumentOutOfRangeException>(
                static () => new SemanticVersion(0, -1, 0, (IEnumerable<SemanticPreRelease>?)null));
            Assert.Throws<ArgumentOutOfRangeException>(
                static () => new SemanticVersion(0, 0, -1, (IEnumerable<SemanticPreRelease>?)null));

            Assert.Throws<ArgumentException>(
                static () => new SemanticVersion(12, 34, 56, (IEnumerable<string>?)null, new[] { "build", string.Empty }));
            Assert.Throws<ArgumentException>(
                static () => new SemanticVersion(12, 34, 56, (IEnumerable<string>?)null, new[] { "build", "$$invalid$$" }));

        }

        [Fact]
        public void ConversionTest()
        {
            Version systemVersion = new Version(12, 34);
            AssertVersion(new SemanticVersion(systemVersion), 12, 34, 0);
            AssertVersion((SemanticVersion)systemVersion, 12, 34, 0);
            systemVersion = new Version(12, 34, 56);
            AssertVersion(new SemanticVersion(systemVersion), 12, 34, 56);
            AssertVersion((SemanticVersion)systemVersion, 12, 34, 56);
            systemVersion = new Version(12, 34, 56, 78);
            AssertVersion(new SemanticVersion(systemVersion), 12, 34, 56);
            AssertVersion((SemanticVersion)systemVersion, 12, 34, 56);

            systemVersion = (Version)new SemanticVersion(12, 34, 56, new []{ "alpha" }, new []{ "build" });
            Assert.Equal(12, systemVersion.Major);
            Assert.Equal(34, systemVersion.Minor);
            Assert.Equal(56, systemVersion.Build);
            Assert.Equal(-1, systemVersion.Revision);

        }

    }
}
