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

        public static readonly IEnumerable<object[]> ParseFixtures = Util.Arrayify(new VersionParseFixture[]
        {
            // all zeroes
            new VersionParseFixture("0.0.0").Returns(0, 0, 0),
            // all non-zeroes
            new VersionParseFixture("1.2.3").Returns(1, 2, 3),
            // double-digits
            new VersionParseFixture("12.34.56").Returns(12, 34, 56),
            // varying lengths
            new VersionParseFixture("1.23.456").Returns(1, 23, 456),
            new VersionParseFixture("123.45.6").Returns(123, 45, 6),
            // leading zeroes
            new VersionParseFixture("01.2.3")
                .ThrowsStrict(Exceptions.MajorLeadingZeroes).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("1.02.3")
                .ThrowsStrict(Exceptions.MinorLeadingZeroes).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("1.2.03")
                .ThrowsStrict(Exceptions.PatchLeadingZeroes).ReturnsLoose(1, 2, 3),

            // missing components
            new VersionParseFixture("").Throws(Exceptions.MajorNotFound), // invalid even in loose mode
            new VersionParseFixture("1")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 0, 0),
            new VersionParseFixture("1.")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 0, 0),
            new VersionParseFixture("1.2")
                .ThrowsStrict(Exceptions.PatchNotFound).ReturnsLoose(1, 2, 0),
            new VersionParseFixture("1.2.")
                .ThrowsStrict(Exceptions.PatchNotFound).ReturnsLoose(1, 2, 0),
            new VersionParseFixture("1-alpha.0+build.007")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 0, 0, "alpha", 0, "+build", "007"),
            new VersionParseFixture("1.-alpha.0+build.007")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 0, 0, "alpha", 0, "+build", "007"),
            new VersionParseFixture("1.2-alpha.0+build.007")
                .ThrowsStrict(Exceptions.PatchNotFound).ReturnsLoose(1, 2, 0, "alpha", 0, "+build", "007"),
            new VersionParseFixture("1.2.-alpha.0+build.007")
                .ThrowsStrict(Exceptions.PatchNotFound).ReturnsLoose(1, 2, 0, "alpha", 0, "+build", "007"),

            // alphabetic pre-releases
            new VersionParseFixture("1.2.3-alpha").Returns(1, 2, 3, "alpha"),
            new VersionParseFixture("1.2.3-beta").Returns(1, 2, 3, "beta"),
            new VersionParseFixture("1.2.3-beta.alpha.dev").Returns(1, 2, 3, "beta", "alpha", "dev"),

            // numeric pre-releases
            new VersionParseFixture("1.2.3-0").Returns(1, 2, 3, 0),
            new VersionParseFixture("1.2.3-72").Returns(1, 2, 3, 72),
            new VersionParseFixture("1.2.3-72.0.9").Returns(1, 2, 3, 72, 0, 9),

            // numeric pre-releases with leading zeroes
            new VersionParseFixture("1.2.3-00")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 0),
            new VersionParseFixture("1.2.3-000")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 0),
            new VersionParseFixture("1.2.3-072")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 72),
            new VersionParseFixture("1.2.3-0072")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 72),
            new VersionParseFixture("1.2.3-72.0.09")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(1, 2, 3, 72, 0, 9),

            // alphabetic and numeric pre-releases
            new VersionParseFixture("0.1.23-alpha.5").Returns(0, 1, 23, "alpha", 5),
            new VersionParseFixture("0.1.23-alpha.5.beta").Returns(0, 1, 23, "alpha", 5, "beta"),
            new VersionParseFixture("0.1.23-alpha.5.beta.9").Returns(0, 1, 23, "alpha", 5, "beta", 9),
            new VersionParseFixture("0.1.23-alpha.05.beta")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(0, 1, 23, "alpha", 5, "beta"),
            new VersionParseFixture("0.1.23-alpha.5.beta.09")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(0, 1, 23, "alpha", 5, "beta", 9),

            // empty pre-releases
            new VersionParseFixture("1.2.3-")
                .ThrowsStrict(Exceptions.PreReleaseNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("1.2.3-0.")
                .ThrowsStrict(Exceptions.PreReleaseNotFound).ReturnsLoose(1, 2, 3, 0),
            new VersionParseFixture("1.2.3-0.pre..1")
                .ThrowsStrict(Exceptions.PreReleaseNotFound).ReturnsLoose(1, 2, 3, 0, "pre", 1),

            // alphanumeric pre-releases
            new VersionParseFixture("7.12.80-rc-1").Returns(7, 12, 80, "rc-1"),
            new VersionParseFixture("7.12.80-alpha5.beta").Returns(7, 12, 80, "alpha5", "beta"),
            new VersionParseFixture("7.12.80-alpha.01-beta").Returns(7, 12, 80, "alpha", "01-beta"),
            // alphanumeric pre-releases with hyphens
            new VersionParseFixture("7.12.80--rc-1").Returns(7, 12, 80, "-rc-1"),
            new VersionParseFixture("7.12.80-rc-1-").Returns(7, 12, 80, "rc-1-"),
            new VersionParseFixture("7.12.80--rc-1-").Returns(7, 12, 80, "-rc-1-"),
            new VersionParseFixture("7.12.80---rc-1").Returns(7, 12, 80, "--rc-1"),
            new VersionParseFixture("7.12.80-rc-1--").Returns(7, 12, 80, "rc-1--"),
            new VersionParseFixture("7.12.80---rc-1--").Returns(7, 12, 80, "--rc-1--"),
            // *numeric* pre-releases with hyphens and leading zeroes
            new VersionParseFixture("5.6.0--1").Returns(5, 6, 0, "-1"),
            new VersionParseFixture("5.6.0---1").Returns(5, 6, 0, "--1"),
            new VersionParseFixture("5.6.0--1.65").Returns(5, 6, 0, "-1", 65),
            new VersionParseFixture("5.6.0-1.-65").Returns(5, 6, 0, 1, "-65"),
            new VersionParseFixture("5.6.0--01").Returns(5, 6, 0, "-01"),
            new VersionParseFixture("5.6.0--01.65").Returns(5, 6, 0, "-01", 65),
            new VersionParseFixture("5.6.0--01.065")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(5, 6, 0, "-01", 65),
            new VersionParseFixture("5.6.0-01.-65")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(5, 6, 0, 1, "-65"),

            // build metadata
            new VersionParseFixture("4.0.2+build").Returns(4, 0, 2, "+build"),
            new VersionParseFixture("4.0.2+build.2").Returns(4, 0, 2, "+build", "2"),
            new VersionParseFixture("4.0.2+build.002").Returns(4, 0, 2, "+build", "002"),
            new VersionParseFixture("4.0.2+test-build.1").Returns(4, 0, 2, "+test-build", "1"),
            new VersionParseFixture("4.0.2+-test-build--.1").Returns(4, 0, 2, "+-test-build--", "1"),
            new VersionParseFixture("4.0.2+-test-build--.001").Returns(4, 0, 2, "+-test-build--", "001"),
            new VersionParseFixture("4.0.2+-test-build--.-01").Returns(4, 0, 2, "+-test-build--", "-01"),
            // empty build metadata
            new VersionParseFixture("4.0.2+")
                .ThrowsStrict(Exceptions.BuildMetadataNotFound).ReturnsLoose(4, 0, 2),
            new VersionParseFixture("4.0.2+0.")
                .ThrowsStrict(Exceptions.BuildMetadataNotFound).ReturnsLoose(4, 0, 2, "+0"),
            new VersionParseFixture("4.0.2+0.build..1")
                .ThrowsStrict(Exceptions.BuildMetadataNotFound).ReturnsLoose(4, 0, 2, "+0", "build", "1"),

            // pre-releases and build metadata
            new VersionParseFixture("0.0.7-pre.3+build.02").Returns(0, 0, 7, "pre", 3, "+build", "02"),
            new VersionParseFixture("0.0.7-pre.3+build-meta--.02").Returns(0, 0, 7, "pre", 3, "+build-meta--", "02"),
            new VersionParseFixture("0.0.7-pre-alpha.3+build.-02--").Returns(0, 0, 7, "pre-alpha", 3, "+build", "-02--"),
            new VersionParseFixture("0.0.7-pre-alpha.-03+build.02").Returns(0, 0, 7, "pre-alpha", "-03", "+build", "02"),
            new VersionParseFixture("0.0.7-pre-alpha.03+build.02")
                .ThrowsStrict(Exceptions.PreReleaseLeadingZeroes).ReturnsLoose(0, 0, 7, "pre-alpha", 3, "+build", "02"),

            // prefixes
            new VersionParseFixture("v1.2.3")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("V1.2.3")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("=v1.2.3")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("=V1.2.3")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("v  1.2.3")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("V  1.2.3")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("=  v  1.2.3")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("=  V  1.2.3")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 2, 3),
            new VersionParseFixture("v=1.2.3").Throws(Exceptions.MajorNotFound), // '=' must precede 'v'
            new VersionParseFixture("V=1.2.3").Throws(Exceptions.MajorNotFound),
            new VersionParseFixture("v  =  1.2.3").Throws(Exceptions.MajorNotFound),
            new VersionParseFixture("V  =  1.2.3").Throws(Exceptions.MajorNotFound),

            // leading and trailing whitespace
            new VersionParseFixture("  1.7.10-alpha.5  ")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 7, 10, "alpha", 5),
            new VersionParseFixture("\r\n \t1.7.10-alpha.5\t \n\r")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 7, 10, "alpha", 5),
            // leftovers
            new VersionParseFixture("1.7.5-pre.2+build$$")
                .ThrowsStrict(LeftoversException).ReturnsLoose(1, 7, 5, "pre", 2, "+build"),
            new VersionParseFixture("\r\n \t1.7.5-pre.2+build\t \n\r$$")
                .ThrowsStrict(Exceptions.MajorNotFound).ReturnsLoose(1, 7, 5, "pre", 2, "+build"),
            // inner whitespace
            new VersionParseFixture("1 . 2 . 5 - alpha . 6 . dev")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 2, 5, "alpha", 6, "dev"),
            new VersionParseFixture("1\r . \t2 .\n 5 \n-\t alpha \n. 6\r \n. \tdev")
                .ThrowsStrict(Exceptions.MinorNotFound).ReturnsLoose(1, 2, 5, "alpha", 6, "dev"),

            // optional pre-release separator
            new VersionParseFixture("0.6.7alpha")
                .ThrowsStrict(LeftoversException).ReturnsLoose(0, 6, 7, "alpha"),
            new VersionParseFixture("0.6.7beta5alpha")
                .ThrowsStrict(LeftoversException).ReturnsLoose(0, 6, 7, "beta", 5, "alpha"),
            new VersionParseFixture("0.6.7beta5alpha+build.007")
                .ThrowsStrict(LeftoversException).ReturnsLoose(0, 6, 7, "beta", 5, "alpha", "+build", "007"),
            new VersionParseFixture("0.6.7 0alpha7")
                .ThrowsStrict(LeftoversException).ReturnsLoose(0, 6, 7, 0, "alpha", 7),

            // number limits
            new VersionParseFixture("2147483647.2147483647.2147483647").Returns(2147483647, 2147483647, 2147483647),
            new VersionParseFixture("2147483648.2147483647.2147483647").Throws(Exceptions.MajorTooBig),
            new VersionParseFixture("2147483647.2147483648.2147483647").Throws(Exceptions.MinorTooBig),
            new VersionParseFixture("2147483647.2147483647.2147483648").Throws(Exceptions.PatchTooBig),
            new VersionParseFixture("1.2.3-alpha.2147483647").Returns(1, 2, 3, "alpha", 2147483647),
            new VersionParseFixture("1.2.3alpha2147483647")
                .ThrowsStrict(LeftoversException).ReturnsLoose(1, 2, 3, "alpha", 2147483647),
            new VersionParseFixture("1.2.3-alpha.2147483648").Throws(Exceptions.PreReleaseTooBig),
            new VersionParseFixture("1.2.3alpha2147483648").Throws(LeftoversException, Exceptions.PreReleaseTooBig),
            new VersionParseFixture("1.2.3+build.2147483647").Returns(1, 2, 3, "+build", "2147483647"),
            new VersionParseFixture("1.2.3+build.2147483648").Returns(1, 2, 3, "+build", "2147483648"),

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
