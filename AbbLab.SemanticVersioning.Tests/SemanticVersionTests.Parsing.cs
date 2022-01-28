using System;
using System.Collections.Generic;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Theory]
        [MemberData(nameof(ParseFixtures))]
        public void ParseTests(VersionParseFixture test)
        {
            Assert.True(test.IsFullyInitialized);
            Output.WriteLine($"Parsing `{test.Semantic}`.");

            // Strict Parsing
            {
                bool success = SemanticVersion.TryParse(test.Semantic, out SemanticVersion? version);
                Assert.Equal(test.IsValid, success);
                if (success) test.Assert(version!);
            }
            {
                bool success = SemanticVersion.TryParse(test.Semantic, SemanticOptions.Strict, out SemanticVersion? version);
                Assert.Equal(test.IsValid, success);
                if (success) test.Assert(version!);
            }
            if (test.IsValid) test.Assert(SemanticVersion.Parse(test.Semantic));
            else
            {
                Exception ex = Assert.Throws(test.ExceptionType!, () => SemanticVersion.Parse(test.Semantic));
                Assert.StartsWith(test.ExceptionMessage!, ex.Message);
            }

            if (test.IsValid) test.Assert(SemanticVersion.Parse(test.Semantic, SemanticOptions.Strict));
            else
            {
                Exception ex = Assert.Throws(test.ExceptionType!, () => SemanticVersion.Parse(test.Semantic, SemanticOptions.Strict));
                Assert.StartsWith(test.ExceptionMessage!, ex.Message);
            }

            // Pseudo-Strict Parsing
            // TryParse() uses the strict parsing algorithm if options are Strict, so we're using this flag to bypass that
            const SemanticOptions pseudoStrictMode = (SemanticOptions)int.MinValue;
            {
                bool success = SemanticVersion.TryParse(test.Semantic, pseudoStrictMode, out SemanticVersion? version);
                Assert.Equal(test.IsValid, success);
                if (success) test.Assert(version!);
            }
            if (test.IsValid) test.Assert(SemanticVersion.Parse(test.Semantic, pseudoStrictMode));
            else
            {
                Exception ex = Assert.Throws<ArgumentException>(() => SemanticVersion.Parse(test.Semantic, pseudoStrictMode));
                Assert.StartsWith(test.ExceptionMessage!, ex.Message);
            }

            // Loose Parsing
            {
                bool success = SemanticVersion.TryParse(test.Semantic, SemanticOptions.Loose, out SemanticVersion? version);
                Assert.Equal(test.IsValidLoose, success);
                if (success) test.Assert(version!);
            }
            if (test.IsValidLoose) test.Assert(SemanticVersion.Parse(test.Semantic, SemanticOptions.Loose));
            else
            {
                Exception ex = Assert.Throws(test.ExceptionTypeLoose!, () => SemanticVersion.Parse(test.Semantic, SemanticOptions.Loose));
                Assert.StartsWith(test.ExceptionMessageLoose!, ex.Message);
            }

        }

        private static VersionParseFixture New(string semantic) => new VersionParseFixture(semantic);

        public static readonly IEnumerable<object[]> ParseFixtures = Util.Arrayify(new VersionParseFixture[]
        {
            // Helper methods:
            // New("string to parse")   - initializes a new fixture;
            // .Returns(...)            - both strict and loose modes should return the specified results;
            // .Throws(...)             - both strict and loose modes should throw the specified exception;
            // .ReturnsLoose(...)       - only loose mode should return the specified result;
            // .ThrowsStrict(...)       - only strict mode should throw the specified exception;

            // Behaviour for both strict and loose modes must be set only once.
            // You can't use .Throws(...) and then ReturnsLoose(...), for example.
            // Use the combination of .ThrowsStrict() and .ReturnsLoose(...).

            // all zeroes
            New("0.0.0").Returns(0, 0, 0),
            // all non-zeroes
            New("1.2.3").Returns(1, 2, 3),
            // double-digits
            New("12.34.56").Returns(12, 34, 56),
            // varying lengths
            New("1.23.456").Returns(1, 23, 456),
            New("123.45.6").Returns(123, 45, 6),
            // leading zeroes
            New("01.2.3").ThrowsStrict(Exceptions.MajorLeadingZeroes).ReturnsLoose(1, 2, 3),
            New("1.02.3").ThrowsStrict(Exceptions.MinorLeadingZeroes).ReturnsLoose(1, 2, 3),
            New("1.2.03").ThrowsStrict(Exceptions.PatchLeadingZeroes).ReturnsLoose(1, 2, 3),

            // missing components
            New("").Throws(Exceptions.MajorNotFound), // invalid even in loose mode
            New("1").ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 0, 0),
            New("1.").ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 0, 0),
            New("1.2").ThrowsStrict(Exceptions.PatchNotFound).ReturnsLoose(1, 2, 0),
            New("1.2.").ThrowsStrict(Exceptions.PatchNotFound).ReturnsLoose(1, 2, 0),
            New("1-alpha.0+build.007")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 0, 0, "alpha", 0, "+build", "007"),
            New("1.-alpha.0+build.007")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 0, 0, "alpha", 0, "+build", "007"),
            New("1.2-alpha.0+build.007")
                .ThrowsStrict(Exceptions.PatchNotFound).ReturnsLoose(1, 2, 0, "alpha", 0, "+build", "007"),
            New("1.2.-alpha.0+build.007")
                .ThrowsStrict(Exceptions.PatchNotFound).ReturnsLoose(1, 2, 0, "alpha", 0, "+build", "007"),

            // alphabetic pre-releases
            New("1.2.3-alpha").Returns(1, 2, 3, "alpha"),
            New("1.2.3-beta").Returns(1, 2, 3, "beta"),
            New("1.2.3-beta.alpha.dev").Returns(1, 2, 3, "beta", "alpha", "dev"),

            // numeric pre-releases
            New("1.2.3-0").Returns(1, 2, 3, 0),
            New("1.2.3-72").Returns(1, 2, 3, 72),
            New("1.2.3-72.0.9").Returns(1, 2, 3, 72, 0, 9),

            // numeric pre-releases with leading zeroes
            New("1.2.3-00").ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 0),
            New("1.2.3-000").ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 0),
            New("1.2.3-072").ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 72),
            New("1.2.3-0072").ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 72),
            New("1.2.3-72.0.09").ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 72, 0, 9),

            // alphabetic and numeric pre-releases
            New("0.1.23-alpha.5").Returns(0, 1, 23, "alpha", 5),
            New("0.1.23-alpha.5.beta").Returns(0, 1, 23, "alpha", 5, "beta"),
            New("0.1.23-alpha.5.beta.9").Returns(0, 1, 23, "alpha", 5, "beta", 9),
            New("0.1.23-alpha.05.beta")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(0, 1, 23, "alpha", 5, "beta"),
            New("0.1.23-alpha.5.beta.09")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(0, 1, 23, "alpha", 5, "beta", 9),

            // empty pre-releases
            New("1.2.3-").ThrowsStrict(Exceptions.PreReleaseNotFound).ReturnsLoose(1, 2, 3),
            New("1.2.3-0.").ThrowsStrict(Exceptions.PreReleaseNotFound).ReturnsLoose(1, 2, 3, 0),
            New("1.2.3-0.pre..1").ThrowsStrict(Exceptions.PreReleaseNotFound).ReturnsLoose(1, 2, 3, 0, "pre", 1),

            // alphanumeric pre-releases
            New("7.12.80-rc-1").Returns(7, 12, 80, "rc-1"),
            New("7.12.80-alpha5.beta").Returns(7, 12, 80, "alpha5", "beta"),
            New("7.12.80-alpha.01-beta").Returns(7, 12, 80, "alpha", "01-beta"),
            // alphanumeric pre-releases with hyphens
            New("7.12.80--rc-1").Returns(7, 12, 80, "-rc-1"),
            New("7.12.80-rc-1-").Returns(7, 12, 80, "rc-1-"),
            New("7.12.80--rc-1-").Returns(7, 12, 80, "-rc-1-"),
            New("7.12.80---rc-1").Returns(7, 12, 80, "--rc-1"),
            New("7.12.80-rc-1--").Returns(7, 12, 80, "rc-1--"),
            New("7.12.80---rc-1--").Returns(7, 12, 80, "--rc-1--"),
            // *numeric* pre-releases with hyphens and leading zeroes
            New("5.6.0--1").Returns(5, 6, 0, "-1"),
            New("5.6.0---1").Returns(5, 6, 0, "--1"),
            New("5.6.0--1.65").Returns(5, 6, 0, "-1", 65),
            New("5.6.0-1.-65").Returns(5, 6, 0, 1, "-65"),
            New("5.6.0--01").Returns(5, 6, 0, "-01"),
            New("5.6.0--01.65").Returns(5, 6, 0, "-01", 65),
            New("5.6.0--01.065").ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(5, 6, 0, "-01", 65),
            New("5.6.0-01.-65").ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(5, 6, 0, 1, "-65"),

            // build metadata
            New("4.0.2+build").Returns(4, 0, 2, "+build"),
            New("4.0.2+build.2").Returns(4, 0, 2, "+build", "2"),
            New("4.0.2+build.002").Returns(4, 0, 2, "+build", "002"),
            New("4.0.2+test-build.1").Returns(4, 0, 2, "+test-build", "1"),
            New("4.0.2+-test-build--.1").Returns(4, 0, 2, "+-test-build--", "1"),
            New("4.0.2+-test-build--.001").Returns(4, 0, 2, "+-test-build--", "001"),
            New("4.0.2+-test-build--.-01").Returns(4, 0, 2, "+-test-build--", "-01"),
            // empty build metadata
            New("4.0.2+").ThrowsStrict(Exceptions.BuildMetadataNotFound).ReturnsLoose(4, 0, 2),
            New("4.0.2+0.").ThrowsStrict(Exceptions.BuildMetadataNotFound).ReturnsLoose(4, 0, 2, "+0"),
            New("4.0.2+0.build..1").ThrowsStrict(Exceptions.BuildMetadataNotFound).ReturnsLoose(4, 0, 2, "+0", "build", "1"),

            // pre-releases and build metadata
            New("0.0.7-pre.3+build.02").Returns(0, 0, 7, "pre", 3, "+build", "02"),
            New("0.0.7-pre.3+build-meta--.02").Returns(0, 0, 7, "pre", 3, "+build-meta--", "02"),
            New("0.0.7-pre-alpha.3+build.-02--").Returns(0, 0, 7, "pre-alpha", 3, "+build", "-02--"),
            New("0.0.7-pre-alpha.-03+build.02").Returns(0, 0, 7, "pre-alpha", "-03", "+build", "02"),
            New("0.0.7-pre-alpha.03+build.02")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(0, 0, 7, "pre-alpha", 3, "+build", "02"),

            // prefixes
            New("v1.2.3").ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            New("V1.2.3").ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            New("=v1.2.3").ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            New("=V1.2.3").ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            New("v  1.2.3").ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            New("V  1.2.3").ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            New("=  v  1.2.3").ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            New("=  V  1.2.3").ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            New("v=1.2.3").Throws(Exceptions.MajorNotFound), // '=' must precede 'v'
            New("V=1.2.3").Throws(Exceptions.MajorNotFound),
            New("v  =  1.2.3").Throws(Exceptions.MajorNotFound),
            New("V  =  1.2.3").Throws(Exceptions.MajorNotFound),

            // leading and trailing whitespace
            New("  1.7.10-alpha.5  ")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 7, 10, "alpha", 5),
            New("\r\n \t1.7.10-alpha.5\t \n\r")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 7, 10, "alpha", 5),
            // leftovers
            New("1.7.5-pre.2+build$$")
                .ThrowsStrict(LeftoversException).ReturnsLoose(1, 7, 5, "pre", 2, "+build"),
            New("\r\n \t1.7.5-pre.2+build\t \n\r$$")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 7, 5, "pre", 2, "+build"),
            // inner whitespace
            New("1 . 2 . 5 - alpha . 6 . dev")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 2, 5, "alpha", 6, "dev"),
            New("1\r . \t2 .\n 5 \n-\t alpha \n. 6\r \n. \tdev")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 2, 5, "alpha", 6, "dev"),

            // optional pre-release separator
            New("0.6.7alpha")
                .ThrowsStrict(LeftoversException).ReturnsLoose(0, 6, 7, "alpha"),
            New("0.6.7beta5alpha")
                .ThrowsStrict(LeftoversException).ReturnsLoose(0, 6, 7, "beta", 5, "alpha"),
            New("0.6.7beta5alpha+build.007")
                .ThrowsStrict(LeftoversException).ReturnsLoose(0, 6, 7, "beta", 5, "alpha", "+build", "007"),
            New("0.6.7 0alpha7")
                .ThrowsStrict(LeftoversException).ReturnsLoose(0, 6, 7, 0, "alpha", 7),

            // number limits
            New("2147483647.2147483647.2147483647").Returns(2147483647, 2147483647, 2147483647),
            New("2147483648.2147483647.2147483647").Throws(Exceptions.MajorTooBig),
            New("2147483647.2147483648.2147483647").Throws(Exceptions.MinorTooBig),
            New("2147483647.2147483647.2147483648").Throws(Exceptions.PatchTooBig),
            New("1.2.3-alpha.2147483647").Returns(1, 2, 3, "alpha", 2147483647),
            New("1.2.3alpha2147483647").ThrowsStrict(LeftoversException).ReturnsLoose(1, 2, 3, "alpha", 2147483647),
            New("1.2.3-alpha.2147483648").Throws(Exceptions.PreReleaseTooBig),
            New("1.2.3alpha2147483648").Throws(LeftoversException, Exceptions.PreReleaseTooBig),
            New("1.2.3+build.2147483647").Returns(1, 2, 3, "+build", "2147483647"),
            New("1.2.3+build.2147483648").Returns(1, 2, 3, "+build", "2147483648"),

        });

        private const string LeftoversException = "Encountered an unexpected character at ";

        public struct VersionParseFixture
        {
            public string Semantic { get; }

            public Type? ExceptionType { get; private set; }
            public string? ExceptionMessage { get; private set; }
            public Type? ExceptionTypeLoose { get; private set; }
            public string? ExceptionMessageLoose { get; private set; }

            public int Major { get; private set; }
            public int Minor { get; private set; }
            public int Patch { get; private set; }
            public SemanticPreRelease[] PreReleases { get; private set; }
            public string[] BuildMetadata { get; private set; }

            private bool StrictSet;
            private bool LooseSet;
            public readonly bool IsFullyInitialized => StrictSet && LooseSet;

            public readonly bool IsValid => ExceptionType is null;
            public readonly bool IsValidLoose => ExceptionTypeLoose is null;

            public VersionParseFixture(string semantic) : this()
            {
                Major = -1;
                Minor = -1;
                Patch = -1;
                PreReleases = Array.Empty<SemanticPreRelease>();
                BuildMetadata = Array.Empty<string>();
                Semantic = semantic;
            }

            public VersionParseFixture Returns(int major, int minor, int patch, params object[] identifiers)
            {
                Xunit.Assert.False(StrictSet || LooseSet);
                StrictSet = true;
                LooseSet = true;
                ExceptionType = null;
                ExceptionMessage = null;
                ExceptionTypeLoose = null;
                ExceptionMessageLoose = null;
                Major = major;
                Minor = minor;
                Patch = patch;
                PreReleases = Util.SeparateIdentifiers(identifiers, out string[] buildMetadata);
                BuildMetadata = buildMetadata;
                return this;
            }
            public VersionParseFixture ReturnsLoose(int major, int minor, int patch, params object[] identifiers)
            {
                Xunit.Assert.False(LooseSet);
                LooseSet = true;
                ExceptionTypeLoose = null;
                ExceptionMessageLoose = null;
                Major = major;
                Minor = minor;
                Patch = patch;
                PreReleases = Util.SeparateIdentifiers(identifiers, out string[] buildMetadata);
                BuildMetadata = buildMetadata;
                return this;
            }

            public VersionParseFixture Throws(string message)
                => Throws<ArgumentException, ArgumentException>(message, message);
            public VersionParseFixture Throws(string messageStrict, string messageLoose)
                => Throws<ArgumentException, ArgumentException>(messageStrict, messageLoose);
            public VersionParseFixture Throws<TExceptionStrict, TExceptionLoose>(string messageStrict, string messageLoose)
            {
                Xunit.Assert.False(StrictSet || LooseSet);
                StrictSet = true;
                LooseSet = true;
                ExceptionType = typeof(TExceptionStrict);
                ExceptionMessage = messageStrict;
                ExceptionTypeLoose = typeof(TExceptionLoose);
                ExceptionMessageLoose = messageLoose;
                return this;
            }

            public VersionParseFixture ThrowsStrict(string message)
                => ThrowsStrict<ArgumentException>(message);
            public VersionParseFixture ThrowsStrict<TException>(string message)
            {
                Xunit.Assert.False(StrictSet);
                StrictSet = true;
                ExceptionType = typeof(TException);
                ExceptionMessage = message;
                return this;
            }

            public readonly string[] GetPreReleaseStrings()
                => Array.ConvertAll(PreReleases, static p => p.ToString());
            public readonly void Assert(SemanticVersion version)
                => AssertEx.Version(version, Major, Minor, Patch, PreReleases, BuildMetadata);

        }

    }
}
