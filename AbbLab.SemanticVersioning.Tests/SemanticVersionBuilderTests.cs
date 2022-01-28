using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionBuilderTests
    {
        private readonly ITestOutputHelper Output;
        public SemanticVersionBuilderTests(ITestOutputHelper output) => Output = output;

        [Fact]
        public void WithTest()
        {
            SemanticVersion sample = SemanticVersion.Parse("1.2.3-alpha.3+test-build.07");
            SemanticVersionBuilder builder = new SemanticVersionBuilder(sample);

            List<SemanticPreRelease> preReleases = sample.PreReleases.ToList();
            List<string> buildMetadata = sample.BuildMetadata.ToList();

            builder.Major = 4;
            AssertEx.Builder(builder, 4, 2, 3, preReleases, buildMetadata);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.Major = -1);
            builder.Minor = 5;
            AssertEx.Builder(builder, 4, 5, 3, preReleases, buildMetadata);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.Minor = -7007);
            builder.Patch = 6;
            AssertEx.Builder(builder, 4, 5, 6, preReleases, buildMetadata);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.Patch = -2147483648);

            AssertEx.Builder(builder.WithMajor(7), 7, 5, 6, preReleases, buildMetadata);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithMajor(-1));
            AssertEx.Builder(builder.WithMinor(8), 7, 8, 6, preReleases, buildMetadata);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithMinor(-4004));
            AssertEx.Builder(builder.WithPatch(9), 7, 8, 9, preReleases, buildMetadata);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithPatch(-2147483648));

            preReleases.Add("beta");
            AssertEx.Builder(builder.AppendPreRelease("beta"), 7, 8, 9, preReleases, buildMetadata);
            preReleases.Add(13);
            AssertEx.Builder(builder.AppendPreRelease(13), 7, 8, 9, preReleases, buildMetadata);

            Assert.Throws<ArgumentException>(() => builder.AppendPreRelease(""));
            Assert.Throws<ArgumentException>(() => builder.AppendPreRelease("$$"));

            preReleases.Clear();
            AssertEx.Builder(builder.ClearPreReleases(), 7, 8, 9, preReleases, buildMetadata);

            string[] identifiers = { "beta", "7" };
            preReleases.AddRange(identifiers.Select(static p => (SemanticPreRelease)p));
            AssertEx.Builder(builder.AppendPreReleases(identifiers), 7, 8, 9, preReleases, buildMetadata);

            Assert.Throws<ArgumentException>(() => builder.AppendPreReleases(new string[] { "beta", "" }));
            Assert.Throws<ArgumentException>(() => builder.AppendPreReleases(new string[] { "beta", "$$" }));

            SemanticPreRelease[] identifiers2 = { "pre-gamma", 14 };
            preReleases.AddRange(identifiers2);
            AssertEx.Builder(builder.AppendPreReleases(identifiers2), 7, 8, 9, preReleases, buildMetadata);

            buildMetadata.Add("meta--");
            AssertEx.Builder(builder.AppendBuildMetadata("meta--"), 7, 8, 9, preReleases, buildMetadata);

            Assert.Throws<ArgumentException>(() => builder.AppendBuildMetadata(""));
            Assert.Throws<ArgumentException>(() => builder.AppendBuildMetadata("$$"));

            buildMetadata.Clear();
            AssertEx.Builder(builder.ClearBuildMetadata(), 7, 8, 9, preReleases, buildMetadata);

            identifiers = new string[] { "-metadata-", "0AF6B24" };
            buildMetadata.AddRange(identifiers);
            AssertEx.Builder(builder.AppendBuildMetadata(identifiers), 7, 8, 9, preReleases, buildMetadata);

            Assert.Throws<ArgumentException>(() => builder.AppendBuildMetadata(new string[] { "beta", "" }));
            Assert.Throws<ArgumentException>(() => builder.AppendBuildMetadata(new string[] { "beta", "$$" }));

        }

        [Theory]
        [MemberData(nameof(IncrementFixtures))]
        public void IncrementTests(IncrementFixture test)
        {
            Output.WriteLine($"Incrementing `{test.Version}` with {test.Type}{(test.Identifier is null ? "" : $" ({test.Identifier})")}.");
            SemanticVersionBuilder builder = new SemanticVersionBuilder(test.Version);

            if (test.IsValid)
            {
                builder.Increment(test.Type, test.Identifier);
                Assert.Equal(test.Expected, builder.ToVersion(), BuildMetadataComparer.Instance!);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => builder.Increment(test.Type, test.Identifier));
            }
        }





        private static IncrementFixture Valid(string version, IncrementType type, string expected)
            => new IncrementFixture(version, type, null, expected);
        private static IncrementFixture Valid(string version, IncrementType type, string? identifier, string expected)
            => new IncrementFixture(version, type, identifier, expected);
        private static IncrementFixture Invalid(string version, IncrementType type)
            => new IncrementFixture(version, type, null, null);
        private static IncrementFixture Invalid(string version, IncrementType type, string? identifier)
            => new IncrementFixture(version, type, identifier, null);


        public static readonly IEnumerable<object[]> IncrementFixtures = Util.Arrayify(new IncrementFixture[]
        {
            // None
            Valid("1.2.3-alpha.7.beta", IncrementType.None, "1.2.3-alpha.7.beta"),

            // Major increment
            Valid("0.1.2", IncrementType.Major, "1.0.0"),
            Valid("1.0.0", IncrementType.Major, "2.0.0"),
            Valid("1.2.3", IncrementType.Major, "2.0.0"),
            Valid("1.2.3-alpha", IncrementType.Major, "2.0.0"),
            Valid("2147483647.0.0-alpha", IncrementType.Major, "2147483647.0.0"),
            Invalid("2147483647.2.3", IncrementType.Major),
            // Minor increment
            Valid("0.0.1", IncrementType.Minor, "0.1.0"),
            Valid("0.1.0", IncrementType.Minor, "0.2.0"),
            Valid("1.2.3", IncrementType.Minor, "1.3.0"),
            Valid("1.2147483647.0-alpha", IncrementType.Minor, "1.2147483647.0"),
            Invalid("1.2147483647.3", IncrementType.Minor),
            // Patch increment
            Valid("0.0.1", IncrementType.Patch, "0.0.2"),
            Valid("0.1.0", IncrementType.Patch, "0.1.1"),
            Valid("1.2.3", IncrementType.Patch, "1.2.4"),
            Valid("1.2.2147483647-alpha", IncrementType.Patch, "1.2.2147483647"),
            Invalid("1.2.2147483647", IncrementType.Patch),

            // Pre-Major increment
            Valid("0.0.0", IncrementType.PreMajor, "1.0.0-0"),
            Valid("0.0.0-0", IncrementType.PreMajor, "1.0.0-0"),
            Valid("1.2.3", IncrementType.PreMajor, "2.0.0-0"),
            Valid("1.2.3", IncrementType.PreMajor, "0", "2.0.0-0"),
            Valid("1.2.3", IncrementType.PreMajor, "17", "2.0.0-17.0"),
            Valid("1.2.3", IncrementType.PreMajor, "rc", "2.0.0-rc.0"),
            Valid("2.0.0-alpha", IncrementType.PreMajor, "3.0.0-0"),
            Valid("2.0.0-alpha", IncrementType.PreMajor, "0", "3.0.0-0"),
            Valid("2.0.0-alpha", IncrementType.PreMajor, "17", "3.0.0-17.0"),
            Valid("2.0.0-alpha", IncrementType.PreMajor, "rc", "3.0.0-rc.0"),
            Invalid("2147483647.2.3", IncrementType.PreMajor),
            Invalid("2147483647.2.3-0", IncrementType.PreMajor),
            // Pre-Minor increment
            Valid("0.0.0", IncrementType.PreMinor, "0.1.0-0"),
            Valid("0.0.0-0", IncrementType.PreMinor, "0.1.0-0"),
            Valid("1.2.3", IncrementType.PreMinor, "1.3.0-0"),
            Valid("1.2.3", IncrementType.PreMinor, "0", "1.3.0-0"),
            Valid("1.2.3", IncrementType.PreMinor, "17", "1.3.0-17.0"),
            Valid("1.2.3", IncrementType.PreMinor, "rc", "1.3.0-rc.0"),
            Valid("2.3.0-alpha", IncrementType.PreMinor, "2.4.0-0"),
            Valid("2.3.0-alpha", IncrementType.PreMinor, "0", "2.4.0-0"),
            Valid("2.3.0-alpha", IncrementType.PreMinor, "17", "2.4.0-17.0"),
            Valid("2.3.0-alpha", IncrementType.PreMinor, "rc", "2.4.0-rc.0"),
            Invalid("1.2147483647.3", IncrementType.PreMinor),
            Invalid("1.2147483647.3-0", IncrementType.PreMinor),
            // Pre-Patch increment
            Valid("0.0.0", IncrementType.PrePatch, "0.0.1-0"),
            Valid("0.0.0-0", IncrementType.PrePatch, "0.0.1-0"),
            Valid("1.2.3", IncrementType.PrePatch, "1.2.4-0"),
            Valid("1.2.3", IncrementType.PrePatch, "0", "1.2.4-0"),
            Valid("1.2.3", IncrementType.PrePatch, "17", "1.2.4-17.0"),
            Valid("1.2.3", IncrementType.PrePatch, "rc", "1.2.4-rc.0"),
            Valid("2.3.4-alpha", IncrementType.PrePatch, "2.3.5-0"),
            Valid("2.3.4-alpha", IncrementType.PrePatch, "0", "2.3.5-0"),
            Valid("2.3.4-alpha", IncrementType.PrePatch, "17", "2.3.5-17.0"),
            Valid("2.3.4-alpha", IncrementType.PrePatch, "rc", "2.3.5-rc.0"),
            Invalid("1.2.2147483647", IncrementType.PrePatch),
            Invalid("1.2.2147483647-0", IncrementType.PrePatch),

            // Pre-Release increment (numeric increment)
            Valid("0.0.0", IncrementType.PreRelease, "0.0.1-0"),
            Valid("0.0.0-0", IncrementType.PreRelease, "0.0.0-1"),
            Valid("1.2.3", IncrementType.PreRelease, "1.2.4-0"),
            Valid("1.2.3-0", IncrementType.PreRelease, "1.2.3-1"),
            Valid("1.2.3-4", IncrementType.PreRelease, "1.2.3-5"),
            Valid("1.2.3-alpha", IncrementType.PreRelease, "1.2.3-alpha.0"),
            Valid("1.2.3-alpha.0", IncrementType.PreRelease, "1.2.3-alpha.1"),
            Valid("1.2.3-alpha.4", IncrementType.PreRelease, "1.2.3-alpha.5"),
            Valid("1.2.3-4.alpha", IncrementType.PreRelease, "1.2.3-5.alpha"),
            // Pre-Release increment (explicit identifier)
            Valid("1.2.3", IncrementType.PreRelease, "beta", "1.2.4-beta.0"),
            Valid("1.2.3-alpha", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),
            Valid("1.2.3-alpha.0", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),
            Valid("1.2.3-alpha.4", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),
            Valid("1.2.3-0.alpha", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),
            Valid("1.2.3-4.alpha", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),
            Valid("1.2.3-beta", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),
            Valid("1.2.3-beta.gamma", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),
            Valid("1.2.3-beta.0", IncrementType.PreRelease, "beta", "1.2.3-beta.1"),
            Valid("1.2.3-beta.4", IncrementType.PreRelease, "beta", "1.2.3-beta.5"),
            Valid("1.2.3-0.beta", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),

            // Pre-Release increment (numeric limits for components)
            Invalid("1.2.2147483647", IncrementType.PreRelease),
            Invalid("1.2.2147483647", IncrementType.PreRelease, "beta"),
            Valid("1.2.2147483647-0", IncrementType.PreRelease, "1.2.2147483647-1"),
            Valid("1.2.2147483647-alpha", IncrementType.PreRelease, "1.2.2147483647-alpha.0"),
            Valid("1.2.2147483647-alpha.0", IncrementType.PreRelease, "1.2.2147483647-alpha.1"),
            Valid("1.2.2147483647-alpha.4", IncrementType.PreRelease, "1.2.2147483647-alpha.5"),
            // Pre-Release increment (numeric limits for pre-release identifiers)
            Invalid("1.2.3-2147483647", IncrementType.PreRelease),
            Invalid("1.2.3-2147483647", IncrementType.PreRelease, "0"),
            Valid("1.2.3-2147483647", IncrementType.PreRelease, "beta", "1.2.3-beta.0"),
            Valid("1.2.3-2147483647", IncrementType.PreRelease, "2147483647", "1.2.3-2147483647.0"),
            Valid("1.2.3-2147483647.0", IncrementType.PreRelease, "2147483647", "1.2.3-2147483647.1"),
            Invalid("1.2.3-2147483647.2147483647", IncrementType.PreRelease, "2147483647"),

        });

        public readonly struct IncrementFixture
        {
            public SemanticVersion Version { get; }
            public IncrementType Type { get; }
            public string? Identifier { get; }
            public SemanticVersion? Expected { get; }
            public bool IsValid { get; }

            public IncrementFixture(string version, IncrementType type, string? identifier, string? expected)
            {
                Version = SemanticVersion.Parse(version);
                Type = type;
                Identifier = identifier;
                Expected = expected is not null ? SemanticVersion.Parse(expected) : null;
                IsValid = expected is not null;
            }

        }

    }
}
