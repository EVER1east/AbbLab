using System;
using System.Linq;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionBuilderTests
    {
        [Fact]
        public void ConstructorTest()
        {
            AssertEx.Builder(new SemanticVersionBuilder(), 0, 0, 0);
            AssertEx.Builder(new SemanticVersionBuilder(1, 2, 3), 1, 2, 3);

            string[] stringIdentifiers = { "alpha", "7" };
            SemanticPreRelease[] identifiers = { "alpha", 7 };
            string[] buildMetadata = { "test-build", "006" };

            AssertEx.Builder(new SemanticVersionBuilder(1, 2, 3, stringIdentifiers), 1, 2, 3, identifiers);
            AssertEx.Builder(new SemanticVersionBuilder(1, 2, 3, identifiers), 1, 2, 3, identifiers);
            AssertEx.Builder(new SemanticVersionBuilder(1, 2, 3, stringIdentifiers.AsEnumerable()), 1, 2, 3, identifiers);
            AssertEx.Builder(new SemanticVersionBuilder(1, 2, 3, identifiers.AsEnumerable()), 1, 2, 3, identifiers);
            AssertEx.Builder(new SemanticVersionBuilder(1, 2, 3, stringIdentifiers, buildMetadata), 1, 2, 3, identifiers, buildMetadata);
            AssertEx.Builder(new SemanticVersionBuilder(1, 2, 3, identifiers, buildMetadata), 1, 2, 3, identifiers, buildMetadata);

            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticVersionBuilder(-1, 2, 3));
            Assert.StartsWith(Exceptions.MajorNegative, ex.Message);
            ex = Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticVersionBuilder(1, -2, 3));
            Assert.StartsWith(Exceptions.MinorNegative, ex.Message);
            ex = Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticVersionBuilder(1, 2, -3));
            Assert.StartsWith(Exceptions.PatchNegative, ex.Message);

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersionBuilder(-1, 2, 3, stringIdentifiers));
            Assert.StartsWith(Exceptions.MajorNegative, ex.Message);
            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersionBuilder(1, -2, 3, stringIdentifiers));
            Assert.StartsWith(Exceptions.MinorNegative, ex.Message);
            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersionBuilder(1, 2, -3, stringIdentifiers));
            Assert.StartsWith(Exceptions.PatchNegative, ex.Message);

            ex = Assert.Throws<ArgumentException>(
                static () => new SemanticVersionBuilder(1, 2, 3, Array.Empty<SemanticPreRelease>(), new string[] { "meta", "" }));
            Assert.StartsWith(Exceptions.BuildMetadataEmpty, ex.Message);
            ex = Assert.Throws<ArgumentException>(
                static () => new SemanticVersionBuilder(1, 2, 3, Array.Empty<SemanticPreRelease>(), new string[] { "meta", "$$" }));
            Assert.StartsWith(Exceptions.BuildMetadataInvalid, ex.Message);

            SemanticVersionBuilder builder = new SemanticVersionBuilder(1, 2, 3, identifiers, buildMetadata);
            SemanticVersionBuilder copy = new SemanticVersionBuilder(builder);
            copy.AppendPreRelease("appended").AppendBuildMetadata("test");
            Assert.NotEqual(builder.PreReleases, copy.PreReleases);
            Assert.NotEqual(builder.BuildMetadata, copy.BuildMetadata);

        }

    }
}
