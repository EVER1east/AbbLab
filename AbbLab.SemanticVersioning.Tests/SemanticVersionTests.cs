using Xunit;
using Xunit.Abstractions;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        private readonly ITestOutputHelper Output;
        public SemanticVersionTests(ITestOutputHelper output) => Output = output;

        [Fact]
        public void MembersTest()
        {
            SemanticVersion version = SemanticVersion.Parse("1.2.3-alpha.5+test-build.007");

            // Check lazy initialization of the read-only collections
            Assert.Null(version._preReleasesReadonly);
            Assert.Same(version.PreReleases, version.PreReleases);
            Assert.Null(version._buildMetadataReadonly);
            Assert.Same(version.BuildMetadata, version.BuildMetadata);
        }

    }
}
