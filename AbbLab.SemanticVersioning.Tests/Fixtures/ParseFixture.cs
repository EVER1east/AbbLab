using System.Collections.Generic;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        public static readonly IEnumerable<object[]> ParseFixture = Util.Arrayify(new VersionTest[]
        {
            // all zeroes
            new VersionTest("0.0.0", 0, 0, 0),
            // all non-zeroes
            new VersionTest("1.2.3", 1, 2, 3),
            // double-digits
            new VersionTest("12.34.56", 12, 34, 56),
            // varying length
            new VersionTest("1.23.456", 1, 23, 456),
            new VersionTest("123.45.6", 123, 45, 6),
            // leading zeroes
            new VersionTest("01.2.3", true, 1, 2, 3),
            new VersionTest("1.02.3", true, 1, 2, 3),
            new VersionTest("1.2.03", true, 1, 2, 3),
            // missing components
            new VersionTest(""), // invalid even for loose mode
            new VersionTest("1", true, 1, 0, 0),
            new VersionTest("1.", true, 1, 0, 0),
            new VersionTest("1.2", true, 1, 2, 0),
            new VersionTest("1.2.", true, 1, 2, 0),
            new VersionTest("1-alpha.0+build.007", true, 1, 0, 0, "alpha", 0, "+build", "007"),
            new VersionTest("1.-alpha.0+build.007", true, 1, 0, 0, "alpha", 0, "+build", "007"),
            new VersionTest("1.2-alpha.0+build.007", true, 1, 2, 0, "alpha", 0, "+build", "007"),
            new VersionTest("1.2.-alpha.0+build.007", true, 1, 2, 0, "alpha", 0, "+build", "007"),

            // alphabetic pre-releases
            new VersionTest("1.2.3-alpha", 1, 2, 3, "alpha"),
            new VersionTest("1.2.3-beta", 1, 2, 3, "beta"),
            new VersionTest("1.2.3-beta.alpha.dev", 1, 2, 3, "beta", "alpha", "dev"),
            // numeric pre-releases
            new VersionTest("1.2.3-0", 1, 2, 3, 0),
            new VersionTest("1.2.3-72", 1, 2, 3, 72),
            new VersionTest("1.2.3-72.0.9", 1, 2, 3, 72, 0, 9),
            // numeric pre-releases with leading zeroes
            new VersionTest("1.2.3-00", true, 1, 2, 3, 0),
            new VersionTest("1.2.3-000", true, 1, 2, 3, 0),
            new VersionTest("1.2.3-072", true, 1, 2, 3, 72),
            new VersionTest("1.2.3-0072", true, 1, 2, 3, 72),
            new VersionTest("1.2.3-72.0.09", true, 1, 2, 3, 72, 0, 9),
            // alphabetic and numeric pre-releases
            new VersionTest("0.1.23-alpha.5", 0, 1, 23, "alpha", 5),
            new VersionTest("0.1.23-alpha.5.beta", 0, 1, 23, "alpha", 5, "beta"),
            new VersionTest("0.1.23-alpha.5.beta.9", 0, 1, 23, "alpha", 5, "beta", 9),
            new VersionTest("0.1.23-alpha.05.beta", true, 0, 1, 23, "alpha", 5, "beta"),
            new VersionTest("0.1.23-alpha.5.beta.09", true, 0, 1, 23, "alpha", 5, "beta", 9),
            // empty pre-releases
            new VersionTest("1.2.3-", true, 1, 2, 3),
            new VersionTest("1.2.3-0.", true, 1, 2, 3, 0),
            new VersionTest("1.2.3-0.pre..1", true, 1, 2, 3, 0, "pre", 1),

            // alphanumeric pre-releases
            new VersionTest("7.12.80-rc-1", 7, 12, 80, "rc-1"),
            new VersionTest("7.12.80-alpha5.beta", 7, 12, 80, "alpha5", "beta"),
            new VersionTest("7.12.80-alpha.01-beta", 7, 12, 80, "alpha", "01-beta"),
            // alphanumeric pre-releases with hyphens
            new VersionTest("7.12.80--rc-1", 7, 12, 80, "-rc-1"),
            new VersionTest("7.12.80-rc-1-", 7, 12, 80, "rc-1-"),
            new VersionTest("7.12.80--rc-1-", 7, 12, 80, "-rc-1-"),
            new VersionTest("7.12.80---rc-1", 7, 12, 80, "--rc-1"),
            new VersionTest("7.12.80-rc-1--", 7, 12, 80, "rc-1--"),
            new VersionTest("7.12.80---rc-1--", 7, 12, 80, "--rc-1--"),
            // *numeric* pre-releases with hyphens and leading zeroes
            new VersionTest("5.6.0--1", 5, 6, 0, "-1"),
            new VersionTest("5.6.0---1", 5, 6, 0, "--1"),
            new VersionTest("5.6.0--1.65", 5, 6, 0, "-1", 65),
            new VersionTest("5.6.0-1.-65", 5, 6, 0, 1, "-65"),
            new VersionTest("5.6.0--01", 5, 6, 0, "-01"),
            new VersionTest("5.6.0--01.65", 5, 6, 0, "-01", 65),
            new VersionTest("5.6.0--01.065", true, 5, 6, 0, "-01", 65),
            new VersionTest("5.6.0-01.-65", true, 5, 6, 0, 1, "-65"),

            // build metadata
            new VersionTest("4.0.2+build", 4, 0, 2, "+build"),
            new VersionTest("4.0.2+build.2", 4, 0, 2, "+build", "2"),
            new VersionTest("4.0.2+build.002", 4, 0, 2, "+build", "002"),
            new VersionTest("4.0.2+test-build.1", 4, 0, 2, "+test-build", "1"),
            new VersionTest("4.0.2+-test-build--.1", 4, 0, 2, "+-test-build--", "1"),
            new VersionTest("4.0.2+-test-build--.001", 4, 0, 2, "+-test-build--", "001"),
            new VersionTest("4.0.2+-test-build--.-01", 4, 0, 2, "+-test-build--", "-01"),
            // empty build metadata
            new VersionTest("4.0.2+", true, 4, 0, 2),
            new VersionTest("4.0.2+0.", true, 4, 0, 2, "+0"),
            new VersionTest("4.0.2+0.build..1", true, 4, 0, 2, "+0", "build", "1"),

            // pre-releases and build metadata
            new VersionTest("0.0.7-pre.3+build.02", 0, 0, 7, "pre", 3, "+build", "02"),
            new VersionTest("0.0.7-pre.3+build-meta--.02", 0, 0, 7, "pre", 3, "+build-meta--", "02"),
            new VersionTest("0.0.7-pre-alpha.3+build.-02--", 0, 0, 7, "pre-alpha", 3, "+build", "-02--"),
            new VersionTest("0.0.7-pre-alpha.-03+build.02", 0, 0, 7, "pre-alpha", "-03", "+build", "02"),
            new VersionTest("0.0.7-pre-alpha.03+build.02", true, 0, 0, 7, "pre-alpha", 3, "+build", "02"),

            // prefixes
            new VersionTest("v1.2.3", true, 1, 2, 3),
            new VersionTest("V1.2.3", true, 1, 2, 3),
            new VersionTest("=v1.2.3", true, 1, 2, 3),
            new VersionTest("=V1.2.3", true, 1, 2, 3),
            new VersionTest("v  1.2.3", true, 1, 2, 3),
            new VersionTest("V  1.2.3", true, 1, 2, 3),
            new VersionTest("=  v  1.2.3", true, 1, 2, 3),
            new VersionTest("=  V  1.2.3", true, 1, 2, 3),
            new VersionTest("v=1.2.3"), // invalid
            new VersionTest("V=1.2.3"), // '=' must precede 'v'
            new VersionTest("v  =  1.2.3"),
            new VersionTest("V  =  1.2.3"),

            // leading and trailing whitespace
            new VersionTest("  1.7.10-alpha.5  ", true, 1, 7, 10, "alpha", 5),
            new VersionTest("\r\n \t1.7.10-alpha.5\t \n\r", true, 1, 7, 10, "alpha", 5),
            // leftovers
            new VersionTest("1.7.5-pre.2+build$$", true, 1, 7, 5, "pre", 2, "+build"),
            new VersionTest("\r\n \t1.7.5-pre.2+build\t \n\r$$", true, 1, 7, 5, "pre", 2, "+build"),
            // inner whitespace
            new VersionTest("1 . 2 . 5 - alpha . 6 . dev", true, 1, 2, 5, "alpha", 6, "dev"),
            new VersionTest("1\r . \t2 .\n 5 \n-\t alpha \n. 6\r \n. \tdev", true, 1, 2, 5, "alpha", 6, "dev"),

            // optional pre-release separator
            new VersionTest("0.6.7alpha", true, 0, 6, 7, "alpha"),
            new VersionTest("0.6.7beta5alpha", true, 0, 6, 7, "beta", 5, "alpha"),
            new VersionTest("0.6.7beta5alpha+build.007", true, 0, 6, 7, "beta", 5, "alpha", "+build", "007"),
            new VersionTest("0.6.7 0alpha7", true, 0, 6, 7, 0, "alpha", 7),

            // number limits
            new VersionTest("2147483647.2147483647.2147483647", 2147483647, 2147483647, 2147483647),
            new VersionTest("2147483648.2147483647.2147483647"),
            new VersionTest("2147483647.2147483648.2147483647"),
            new VersionTest("2147483647.2147483647.2147483648"),
            new VersionTest("1.2.3-alpha.2147483647", 1, 2, 3, "alpha", 2147483647),
            new VersionTest("1.2.3-alpha.2147483648"),
            new VersionTest("1.2.3alpha2147483647", true, 1, 2, 3, "alpha", 2147483647),
            new VersionTest("1.2.3alpha2147483648"),
            new VersionTest("1.2.3+build.2147483647", 1, 2, 3, "+build", "2147483647"),
            new VersionTest("1.2.3+build.2147483648", 1, 2, 3, "+build", "2147483648"),

        });
    }
}
