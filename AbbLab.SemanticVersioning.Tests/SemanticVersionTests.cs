using System;
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
        public void ParseTests(VersionInfo info)
        {
            Output.WriteLine($"Parsing `{info.Semantic}`.");
            bool success = SemanticVersion.TryParse(info.Semantic, out SemanticVersion? tryParseResult);
            try
            {
                SemanticVersion parseResult = SemanticVersion.Parse(info.Semantic);
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
        public void LooseParseTests(VersionInfo info)
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

    }
}
