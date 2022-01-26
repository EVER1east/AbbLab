using System;
using System.Collections.Generic;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Theory]
        [MemberData(nameof(ParseFixtures))]
        public void ParseTests(VersionParseTest test)
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
            else Assert.Throws<ArgumentException>(() => SemanticVersion.Parse(test.Semantic));
            if (test.IsValid) test.Assert(SemanticVersion.Parse(test.Semantic, SemanticOptions.Strict));
            else Assert.Throws<ArgumentException>(() => SemanticVersion.Parse(test.Semantic, SemanticOptions.Strict));

            // Pseudo-Strict Parsing
            // TryParse() uses the strict parsing algorithm if options are Strict, so we're using this flag to bypass that
            const SemanticOptions pseudoStrictMode = (SemanticOptions)int.MinValue;
            {
                bool success = SemanticVersion.TryParse(test.Semantic, pseudoStrictMode, out SemanticVersion? version);
                Assert.Equal(test.IsValid, success);
                if (success) test.Assert(version!);
            }
            if (test.IsValid) test.Assert(SemanticVersion.Parse(test.Semantic, pseudoStrictMode));
            else Assert.Throws<ArgumentException>(() => SemanticVersion.Parse(test.Semantic, pseudoStrictMode));

            // Loose Parsing
            {
                bool success = SemanticVersion.TryParse(test.Semantic, SemanticOptions.Loose, out SemanticVersion? version);
                Assert.Equal(test.IsValidLoose, success);
                if (success) test.Assert(version!);
            }
            if (test.IsValidLoose) test.Assert(SemanticVersion.Parse(test.Semantic, SemanticOptions.Loose));
            else Assert.Throws<ArgumentException>(() => SemanticVersion.Parse(test.Semantic, SemanticOptions.Loose));

        }

        public static VersionParseTest Strict(string semantic, int major, int minor, int patch, params object[] identifiers)
            => new VersionParseTest(semantic, true, true, major, minor, patch, identifiers);
        public static VersionParseTest Loose(string semantic, int major, int minor, int patch, params object[] identifiers)
            => new VersionParseTest(semantic, false, true, major, minor, patch, identifiers);
        public static VersionParseTest Invalid(string semantic)
            => new VersionParseTest(semantic, false, false, -1, -1, -1);

        public static readonly IEnumerable<object[]> ParseFixtures = Util.Arrayify(new VersionParseTest[]
        {
            // all zeroes
            Strict("0.0.0", 0, 0, 0),
            // all non-zeroes
            Strict("1.2.3", 1, 2, 3),
            // double-digits
            Strict("12.34.56", 12, 34, 56),
            // varying lengths
            Strict("1.23.456", 1, 23, 456),
            Strict("123.45.6", 123, 45, 6),
            // leading zeroes
            Loose("01.2.3", 1, 2, 3),
            Loose("1.02.3", 1, 2, 3),
            Loose("1.2.03", 1, 2, 3),
            // missing components
            Invalid(""), // invalid even in loose mode
            Loose("1", 1, 0, 0),
            Loose("1.", 1, 0, 0),
            Loose("1.2", 1, 2, 0),
            Loose("1.2.", 1, 2, 0),
            Loose("1-alpha.0+build.007", 1, 0, 0, "alpha", 0, "+build", "007"),
            Loose("1.-alpha.0+build.007", 1, 0, 0, "alpha", 0, "+build", "007"),
            Loose("1.2-alpha.0+build.007", 1, 2, 0, "alpha", 0, "+build", "007"),
            Loose("1.2.-alpha.0+build.007", 1, 2, 0, "alpha", 0, "+build", "007"),

            // alphabetic pre-releases
            Strict("1.2.3-alpha", 1, 2, 3, "alpha"),
            Strict("1.2.3-beta", 1, 2, 3, "beta"),
            Strict("1.2.3-beta.alpha.dev", 1, 2, 3, "beta", "alpha", "dev"),
            // numeric pre-releases
            Strict("1.2.3-0", 1, 2, 3, 0),
            Strict("1.2.3-72", 1, 2, 3, 72),
            Strict("1.2.3-72.0.9", 1, 2, 3, 72, 0, 9),
            // numeric pre-releases with leading zeroes
            Loose("1.2.3-00", 1, 2, 3, 0),
            Loose("1.2.3-000", 1, 2, 3, 0),
            Loose("1.2.3-072", 1, 2, 3, 72),
            Loose("1.2.3-0072", 1, 2, 3, 72),
            Loose("1.2.3-72.0.09", 1, 2, 3, 72, 0, 9),
            // alphabetic and numeric pre-releases
            Strict("0.1.23-alpha.5", 0, 1, 23, "alpha", 5),
            Strict("0.1.23-alpha.5.beta", 0, 1, 23, "alpha", 5, "beta"),
            Strict("0.1.23-alpha.5.beta.9", 0, 1, 23, "alpha", 5, "beta", 9),
            Loose("0.1.23-alpha.05.beta", 0, 1, 23, "alpha", 5, "beta"),
            Loose("0.1.23-alpha.5.beta.09", 0, 1, 23, "alpha", 5, "beta", 9),
            // empty pre-releases
            Loose("1.2.3-", 1, 2, 3),
            Loose("1.2.3-0.", 1, 2, 3, 0),
            Loose("1.2.3-0.pre..1", 1, 2, 3, 0, "pre", 1),

            // alphanumeric pre-releases
            Strict("7.12.80-rc-1", 7, 12, 80, "rc-1"),
            Strict("7.12.80-alpha5.beta", 7, 12, 80, "alpha5", "beta"),
            Strict("7.12.80-alpha.01-beta", 7, 12, 80, "alpha", "01-beta"),
            // alphanumeric pre-releases with hyphens
            Strict("7.12.80--rc-1", 7, 12, 80, "-rc-1"),
            Strict("7.12.80-rc-1-", 7, 12, 80, "rc-1-"),
            Strict("7.12.80--rc-1-", 7, 12, 80, "-rc-1-"),
            Strict("7.12.80---rc-1", 7, 12, 80, "--rc-1"),
            Strict("7.12.80-rc-1--", 7, 12, 80, "rc-1--"),
            Strict("7.12.80---rc-1--", 7, 12, 80, "--rc-1--"),
            // *numeric* pre-releases with hyphens and leading zeroes
            Strict("5.6.0--1", 5, 6, 0, "-1"),
            Strict("5.6.0---1", 5, 6, 0, "--1"),
            Strict("5.6.0--1.65", 5, 6, 0, "-1", 65),
            Strict("5.6.0-1.-65", 5, 6, 0, 1, "-65"),
            Strict("5.6.0--01", 5, 6, 0, "-01"),
            Strict("5.6.0--01.65", 5, 6, 0, "-01", 65),
            Loose("5.6.0--01.065", 5, 6, 0, "-01", 65),
            Loose("5.6.0-01.-65", 5, 6, 0, 1, "-65"),

            // build metadata
            Strict("4.0.2+build", 4, 0, 2, "+build"),
            Strict("4.0.2+build.2", 4, 0, 2, "+build", "2"),
            Strict("4.0.2+build.002", 4, 0, 2, "+build", "002"),
            Strict("4.0.2+test-build.1", 4, 0, 2, "+test-build", "1"),
            Strict("4.0.2+-test-build--.1", 4, 0, 2, "+-test-build--", "1"),
            Strict("4.0.2+-test-build--.001", 4, 0, 2, "+-test-build--", "001"),
            Strict("4.0.2+-test-build--.-01", 4, 0, 2, "+-test-build--", "-01"),
            // empty build metadata
            Loose("4.0.2+", 4, 0, 2),
            Loose("4.0.2+0.", 4, 0, 2, "+0"),
            Loose("4.0.2+0.build..1", 4, 0, 2, "+0", "build", "1"),

            // pre-releases and build metadata
            Strict("0.0.7-pre.3+build.02", 0, 0, 7, "pre", 3, "+build", "02"),
            Strict("0.0.7-pre.3+build-meta--.02", 0, 0, 7, "pre", 3, "+build-meta--", "02"),
            Strict("0.0.7-pre-alpha.3+build.-02--", 0, 0, 7, "pre-alpha", 3, "+build", "-02--"),
            Strict("0.0.7-pre-alpha.-03+build.02", 0, 0, 7, "pre-alpha", "-03", "+build", "02"),
            Loose("0.0.7-pre-alpha.03+build.02", 0, 0, 7, "pre-alpha", 3, "+build", "02"),

            // prefixes
            Loose("v1.2.3", 1, 2, 3),
            Loose("V1.2.3", 1, 2, 3),
            Loose("=v1.2.3", 1, 2, 3),
            Loose("=V1.2.3", 1, 2, 3),
            Loose("v  1.2.3", 1, 2, 3),
            Loose("V  1.2.3", 1, 2, 3),
            Loose("=  v  1.2.3", 1, 2, 3),
            Loose("=  V  1.2.3", 1, 2, 3),
            Invalid("v=1.2.3"), // '=' must precede 'v'
            Invalid("V=1.2.3"),
            Invalid("v  =  1.2.3"),
            Invalid("V  =  1.2.3"),

            // leading and trailing whitespace
            Loose("  1.7.10-alpha.5  ", 1, 7, 10, "alpha", 5),
            Loose("\r\n \t1.7.10-alpha.5\t \n\r", 1, 7, 10, "alpha", 5),
            // leftovers
            Loose("1.7.5-pre.2+build$$", 1, 7, 5, "pre", 2, "+build"),
            Loose("\r\n \t1.7.5-pre.2+build\t \n\r$$", 1, 7, 5, "pre", 2, "+build"),
            // inner whitespace
            Loose("1 . 2 . 5 - alpha . 6 . dev", 1, 2, 5, "alpha", 6, "dev"),
            Loose("1\r . \t2 .\n 5 \n-\t alpha \n. 6\r \n. \tdev", 1, 2, 5, "alpha", 6, "dev"),

            // optional pre-release separator
            Loose("0.6.7alpha", 0, 6, 7, "alpha"),
            Loose("0.6.7beta5alpha", 0, 6, 7, "beta", 5, "alpha"),
            Loose("0.6.7beta5alpha+build.007", 0, 6, 7, "beta", 5, "alpha", "+build", "007"),
            Loose("0.6.7 0alpha7", 0, 6, 7, 0, "alpha", 7),

            // number limits
            Strict("2147483647.2147483647.2147483647", 2147483647, 2147483647, 2147483647),
            Invalid("2147483648.2147483647.2147483647"),
            Invalid("2147483647.2147483648.2147483647"),
            Invalid("2147483647.2147483647.2147483648"),
            Strict("1.2.3-alpha.2147483647", 1, 2, 3, "alpha", 2147483647),
            Loose("1.2.3alpha2147483647", 1, 2, 3, "alpha", 2147483647),
            Invalid("1.2.3-alpha.2147483648"),
            Invalid("1.2.3alpha2147483648"),
            Strict("1.2.3+build.2147483647", 1, 2, 3, "+build", "2147483647"),
            Strict("1.2.3+build.2147483648", 1, 2, 3, "+build", "2147483648"),

        });

        public readonly struct VersionParseTest
        {
            public string Semantic { get; }
            public bool IsValid { get; }
            public bool IsValidLoose { get; }
            public int Major { get; }
            public int Minor { get; }
            public int Patch { get; }
            public SemanticPreRelease[] PreReleases { get; }
            public string[] BuildMetadata { get; }

            public VersionParseTest(string semantic, bool isValid, bool isValidLoose, int major, int minor, int patch, params object[] identifiers)
            {
                Semantic = semantic;
                IsValid = isValid;
                IsValidLoose = isValidLoose;
                Major = major;
                Minor = minor;
                Patch = patch;
                PreReleases = Util.SeparateIdentifiers(identifiers, out string[] buildMetadata);
                BuildMetadata = buildMetadata;
            }

            public string[] GetPreReleaseStrings()
                => Array.ConvertAll(PreReleases, static p => p.ToString());
            public void Assert(SemanticVersion version) => Util.AssertVersion(version, Major, Minor, Patch, PreReleases, BuildMetadata);

        }

    }
}
