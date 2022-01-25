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
            bool tryParseSuccess = SemanticVersion.TryParse(info.Semantic, out SemanticVersion? tryParseResult);
            try
            {
                SemanticVersion parseResult = SemanticVersion.Parse(info.Semantic);
                if (!info.IsValid) throw new InvalidOperationException("An invalid semantic string was successfully parsed.");
                Assert.True(tryParseSuccess, "TryParse() could not parse the version, but Parse() did.");
                Assert.Equal(parseResult, tryParseResult, BuildMetadataComparer.Instance!);
                info.Assert(parseResult);
            }
            catch (ArgumentException)
            {
                if (info.IsValid) throw new InvalidOperationException("Could not parse a valid semantic string.");
                Assert.False(tryParseSuccess, "Parse() could not parse the version, but TryParse() did.");
            }
        }

    }
}
