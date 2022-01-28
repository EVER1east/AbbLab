using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Theory]
        [MemberData(nameof(ParseFixtures))]
        public void ConstructorTests(VersionParseFixture test)
        {
            if (test.IsValid)
            {
                int m = test.Major;
                int n = test.Minor;
                int p = test.Patch;

                if (test.BuildMetadata.Length is 0)
                {
                    if (test.PreReleases.Length is 0)
                        test.Assert(new SemanticVersion(m, n, p));
                    test.Assert(new SemanticVersion(m, n, p, test.PreReleases));
                    test.Assert(new SemanticVersion(m, n, p, test.GetPreReleaseStrings()));
                    test.Assert(new SemanticVersion(m, n, p, test.PreReleases.AsEnumerable()));
                    test.Assert(new SemanticVersion(m, n, p, test.GetPreReleaseStrings().AsEnumerable()));
                }
                test.Assert(new SemanticVersion(m, n, p, test.PreReleases, test.BuildMetadata));
                test.Assert(new SemanticVersion(m, n, p, test.GetPreReleaseStrings(), test.BuildMetadata));
            }
        }

        [Fact]
        public void ConstructorTest()
        {
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
            AssertEx.Version((SemanticVersion)new Version(), 0, 0, 0);
            AssertEx.Version((SemanticVersion)new Version(12, 34), 12, 34, 0);
            AssertEx.Version((SemanticVersion)new Version(12, 34, 56), 12, 34, 56);
            AssertEx.Version((SemanticVersion)new Version(12, 34, 56, 78), 12, 34, 56);

            Version systemVersion = (Version)new SemanticVersion(4, 0, 0);
            Assert.Equal(4, systemVersion.Major);
            Assert.Equal(0, systemVersion.Minor);
            Assert.Equal(0, systemVersion.Build);
            Assert.Equal(-1, systemVersion.Revision);

        }

    }
}
