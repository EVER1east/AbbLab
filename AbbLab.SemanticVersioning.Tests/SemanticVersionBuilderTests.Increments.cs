using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
                static void TestBuilder(IncrementFixture fixture, Func<SemanticVersionBuilder, SemanticVersionBuilder> func)
                {
                    SemanticVersionBuilder builder = new SemanticVersionBuilder(fixture.Version);
                    builder = func(builder);
                    Assert.Equal(fixture.Expected, builder.ToVersion(), BuildMetadataComparer.Instance!);
                }

                if (test.Identifier is null or "0")
                    TestBuilder(test, b => b.Increment(test.Type));
                TestBuilder(test, b => b.Increment(test.Type, test.Identifier));

                // Make sure that all other overloads give identical results

                if (test.Type == IncrementType.Major)
                    TestBuilder(test, static b => b.IncrementMajor());
                else if (test.Type == IncrementType.Minor)
                    TestBuilder(test, static b => b.IncrementMinor());
                else if (test.Type == IncrementType.Patch)
                    TestBuilder(test, static b => b.IncrementPatch());
                else if (test.Type == IncrementType.PreMajor)
                {
                    if (test.Identifier is null or "0")
                        TestBuilder(test, static b => b.IncrementPreMajor());
                    TestBuilder(test, b => b.IncrementPreMajor(test.Identifier));
                }
                else if (test.Type == IncrementType.PreMinor)
                {
                    if (test.Identifier is null or "0")
                        TestBuilder(test, static b => b.IncrementPreMinor());
                    TestBuilder(test, b => b.IncrementPreMinor(test.Identifier));
                }
                else if (test.Type == IncrementType.PrePatch)
                {
                    if (test.Identifier is null or "0")
                        TestBuilder(test, static b => b.IncrementPrePatch());
                    TestBuilder(test, b => b.IncrementPrePatch(test.Identifier));
                }
                else if (test.Type == IncrementType.PreRelease)
                {
                    if (test.Identifier is null or "0")
                        TestBuilder(test, static b => b.IncrementPreRelease());
                    TestBuilder(test, b => b.IncrementPreRelease(test.Identifier));
                }
            }
            else
            {
                Exception ex = Assert.Throws(test.ExceptionType!, () => builder.Increment(test.Type, test.Identifier));
                Assert.StartsWith(test.ExceptionMessage!, ex.Message);
            }
        }

        private static IncrementFixture New(string version, IncrementType type, string? identifier = null)
            => new IncrementFixture(version, type, identifier);

        public static readonly IEnumerable<object[]> IncrementFixtures = Util.Arrayify(new IncrementFixture[]
        {
            // None
            New("1.2.3-alpha.7.beta", IncrementType.None).Returns("1.2.3-alpha.7.beta"),
            // Invalid
            New("1.2.3", (IncrementType)407080).Throws<ArgumentException>($"Invalid {nameof(IncrementType)} value."),

            // Major increment
            New("0.1.2", IncrementType.Major).Returns("1.0.0"),
            New("1.0.0", IncrementType.Major).Returns("2.0.0"),
            New("1.2.3", IncrementType.Major).Returns("2.0.0"),
            New("1.2.3-alpha", IncrementType.Major).Returns("2.0.0"),
            New("2147483647.0.0-alpha", IncrementType.Major).Returns("2147483647.0.0"),
            New("2147483647.2.3", IncrementType.Major).Throws(Exceptions.MajorTooBig),
            // Minor increment
            New("0.0.1", IncrementType.Minor).Returns("0.1.0"),
            New("0.1.0", IncrementType.Minor).Returns("0.2.0"),
            New("1.2.3", IncrementType.Minor).Returns("1.3.0"),
            New("1.2147483647.0-alpha", IncrementType.Minor).Returns("1.2147483647.0"),
            New("1.2147483647.3", IncrementType.Minor).Throws(Exceptions.MinorTooBig),
            // Patch increment
            New("0.0.1", IncrementType.Patch).Returns("0.0.2"),
            New("0.1.0", IncrementType.Patch).Returns("0.1.1"),
            New("1.2.3", IncrementType.Patch).Returns("1.2.4"),
            New("1.2.2147483647-alpha", IncrementType.Patch).Returns("1.2.2147483647"),
            New("1.2.2147483647", IncrementType.Patch).Throws(Exceptions.PatchTooBig),

            // Pre-Major increment
            New("0.0.0", IncrementType.PreMajor).Returns("1.0.0-0"),
            New("0.0.0-0", IncrementType.PreMajor).Returns("1.0.0-0"),
            New("1.2.3", IncrementType.PreMajor).Returns("2.0.0-0"),
            New("1.2.3", IncrementType.PreMajor, "0").Returns("2.0.0-0"),
            New("1.2.3", IncrementType.PreMajor, "17").Returns("2.0.0-17.0"),
            New("1.2.3", IncrementType.PreMajor, "rc").Returns("2.0.0-rc.0"),
            New("2.0.0-alpha", IncrementType.PreMajor).Returns("3.0.0-0"),
            New("2.0.0-alpha", IncrementType.PreMajor, "0").Returns("3.0.0-0"),
            New("2.0.0-alpha", IncrementType.PreMajor, "17").Returns("3.0.0-17.0"),
            New("2.0.0-alpha", IncrementType.PreMajor, "rc").Returns("3.0.0-rc.0"),
            New("2147483647.2.3", IncrementType.PreMajor).Throws(Exceptions.MajorTooBig),
            New("2147483647.2.3-0", IncrementType.PreMajor).Throws(Exceptions.MajorTooBig),
            // Pre-Minor increment
            New("0.0.0", IncrementType.PreMinor).Returns("0.1.0-0"),
            New("0.0.0-0", IncrementType.PreMinor).Returns("0.1.0-0"),
            New("1.2.3", IncrementType.PreMinor).Returns("1.3.0-0"),
            New("1.2.3", IncrementType.PreMinor, "0").Returns("1.3.0-0"),
            New("1.2.3", IncrementType.PreMinor, "17").Returns("1.3.0-17.0"),
            New("1.2.3", IncrementType.PreMinor, "rc").Returns("1.3.0-rc.0"),
            New("2.3.0-alpha", IncrementType.PreMinor).Returns("2.4.0-0"),
            New("2.3.0-alpha", IncrementType.PreMinor, "0").Returns("2.4.0-0"),
            New("2.3.0-alpha", IncrementType.PreMinor, "17").Returns("2.4.0-17.0"),
            New("2.3.0-alpha", IncrementType.PreMinor, "rc").Returns("2.4.0-rc.0"),
            New("1.2147483647.3", IncrementType.PreMinor).Throws(Exceptions.MinorTooBig),
            New("1.2147483647.3-0", IncrementType.PreMinor).Throws(Exceptions.MinorTooBig),
            // Pre-Patch increment
            New("0.0.0", IncrementType.PrePatch).Returns("0.0.1-0"),
            New("0.0.0-0", IncrementType.PrePatch).Returns("0.0.1-0"),
            New("1.2.3", IncrementType.PrePatch).Returns("1.2.4-0"),
            New("1.2.3", IncrementType.PrePatch, "0").Returns("1.2.4-0"),
            New("1.2.3", IncrementType.PrePatch, "17").Returns("1.2.4-17.0"),
            New("1.2.3", IncrementType.PrePatch, "rc").Returns("1.2.4-rc.0"),
            New("2.3.4-alpha", IncrementType.PrePatch).Returns("2.3.5-0"),
            New("2.3.4-alpha", IncrementType.PrePatch, "0").Returns("2.3.5-0"),
            New("2.3.4-alpha", IncrementType.PrePatch, "17").Returns("2.3.5-17.0"),
            New("2.3.4-alpha", IncrementType.PrePatch, "rc").Returns("2.3.5-rc.0"),
            New("1.2.2147483647", IncrementType.PrePatch).Throws(Exceptions.PatchTooBig),
            New("1.2.2147483647-0", IncrementType.PrePatch).Throws(Exceptions.PatchTooBig),

            // Pre-Release increment (numeric increment)
            New("0.0.0", IncrementType.PreRelease).Returns("0.0.1-0"),
            New("0.0.0-0", IncrementType.PreRelease).Returns("0.0.0-1"),
            New("1.2.3", IncrementType.PreRelease).Returns("1.2.4-0"),
            New("1.2.3-0", IncrementType.PreRelease).Returns("1.2.3-1"),
            New("1.2.3-4", IncrementType.PreRelease).Returns("1.2.3-5"),
            New("1.2.3-alpha", IncrementType.PreRelease).Returns("1.2.3-alpha.0"),
            New("1.2.3-alpha.0", IncrementType.PreRelease).Returns("1.2.3-alpha.1"),
            New("1.2.3-alpha.4", IncrementType.PreRelease).Returns("1.2.3-alpha.5"),
            New("1.2.3-4.alpha", IncrementType.PreRelease).Returns("1.2.3-5.alpha"),
            // Pre-Release increment (explicit identifier)
            New("1.2.3", IncrementType.PreRelease, "beta").Returns("1.2.4-beta.0"),
            New("1.2.3-alpha", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),
            New("1.2.3-alpha.0", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),
            New("1.2.3-alpha.4", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),
            New("1.2.3-0.alpha", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),
            New("1.2.3-4.alpha", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),
            New("1.2.3-beta", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),
            New("1.2.3-beta.gamma", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),
            New("1.2.3-beta.0", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.1"),
            New("1.2.3-beta.4", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.5"),
            New("1.2.3-0.beta", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),

            // Pre-Release increment (numeric limits for components)
            New("1.2.2147483647", IncrementType.PreRelease).Throws(Exceptions.PatchTooBig),
            New("1.2.2147483647", IncrementType.PreRelease, "beta").Throws(Exceptions.PatchTooBig),
            New("1.2.2147483647-0", IncrementType.PreRelease).Returns("1.2.2147483647-1"),
            New("1.2.2147483647-alpha", IncrementType.PreRelease).Returns("1.2.2147483647-alpha.0"),
            New("1.2.2147483647-alpha.0", IncrementType.PreRelease).Returns("1.2.2147483647-alpha.1"),
            New("1.2.2147483647-alpha.4", IncrementType.PreRelease).Returns("1.2.2147483647-alpha.5"),
            // Pre-Release increment (numeric limits for pre-release identifiers)
            New("1.2.3-2147483647", IncrementType.PreRelease).Throws(Exceptions.PreReleaseTooBig),
            New("1.2.3-2147483647", IncrementType.PreRelease, "0").Throws(Exceptions.PreReleaseTooBig),
            New("1.2.3-2147483647", IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0"),
            New("1.2.3-2147483647", IncrementType.PreRelease, "2147483647").Returns("1.2.3-2147483647.0"),
            New("1.2.3-2147483647.0", IncrementType.PreRelease, "2147483647").Returns("1.2.3-2147483647.1"),
            New("1.2.3-2147483647.2147483647", IncrementType.PreRelease, "2147483647")
                .Throws(Exceptions.PreReleaseTooBig),

        });

        public struct IncrementFixture
        {
            public SemanticVersion Version { get; }
            public IncrementType Type { get; }
            public string? Identifier { get; }
            public SemanticVersion? Expected { get; private set; }
            public Type? ExceptionType { get; private set; }
            public string? ExceptionMessage { get; private set; }

            [MemberNotNullWhen(true, nameof(Expected))]
            public readonly bool IsValid => ExceptionType is null;

            public IncrementFixture(string version, IncrementType type, string? identifier) : this()
            {
                Version = SemanticVersion.Parse(version);
                Type = type;
                Identifier = identifier;
            }

            public IncrementFixture Returns(string expected)
            {
                Assert.Null(ExceptionType);
                Expected = SemanticVersion.Parse(expected);
                return this;
            }
            public IncrementFixture Throws(string message)
                => Throws<InvalidOperationException>(message);
            public IncrementFixture Throws<TException>(string message)
            {
                Assert.Null(Expected);
                ExceptionType = typeof(TException);
                ExceptionMessage = message;
                return this;
            }

        }

    }
}
