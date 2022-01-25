using System;
using System.Globalization;
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
    }
}
