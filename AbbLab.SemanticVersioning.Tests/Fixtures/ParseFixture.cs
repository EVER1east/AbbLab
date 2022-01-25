using System.Collections.Generic;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        public static readonly IEnumerable<object[]> ParseFixture = Util.Arrayify(new VersionInfo[]
        {
            // all zeroes
            new VersionInfo("0.0.0", 0, 0, 0),
            // all non-zeroes
            new VersionInfo("1.2.3", 1, 2, 3),
            // double-digits
            new VersionInfo("12.34.56", 12, 34, 56),
            // varying length
            new VersionInfo("1.23.456", 1, 23, 456),
            new VersionInfo("123.45.6", 123, 45, 6),
            // leading zeroes
            new VersionInfo("01.2.3", true, 1, 2, 3),
            new VersionInfo("1.02.3", true, 1, 2, 3),
            new VersionInfo("1.2.03", true, 1, 2, 3),
            // missing components
            new VersionInfo(""), // invalid even for loose mode
            new VersionInfo("1", true, 1, 0, 0),
            new VersionInfo("1.", true, 1, 0, 0),
            new VersionInfo("1.2", true, 1, 2, 0),
            new VersionInfo("1.2.", true, 1, 2, 0),

            // alphabetic pre-releases
            new VersionInfo("1.2.3-alpha", 1, 2, 3, "alpha"),
            new VersionInfo("1.2.3-beta", 1, 2, 3, "beta"),
            new VersionInfo("1.2.3-beta.alpha.dev", 1, 2, 3, "beta", "alpha", "dev"),
            // numeric pre-releases
            new VersionInfo("1.2.3-0", 1, 2, 3, 0),
            new VersionInfo("1.2.3-72", 1, 2, 3, 72),
            new VersionInfo("1.2.3-72.0.9", 1, 2, 3, 72, 0, 9),
            // numeric pre-releases with leading zeroes
            new VersionInfo("1.2.3-00", true, 1, 2, 3, 0),
            new VersionInfo("1.2.3-000", true, 1, 2, 3, 0),
            new VersionInfo("1.2.3-072", true, 1, 2, 3, 72),
            new VersionInfo("1.2.3-0072", true, 1, 2, 3, 72),
            new VersionInfo("1.2.3-72.0.09", true, 1, 2, 3, 72, 0, 9),
            // alphabetic and numeric pre-releases
            new VersionInfo("0.1.23-alpha.5", 0, 1, 23, "alpha", 5),
            new VersionInfo("0.1.23-alpha.5.beta", 0, 1, 23, "alpha", 5, "beta"),
            new VersionInfo("0.1.23-alpha.5.beta.9", 0, 1, 23, "alpha", 5, "beta", 9),
            new VersionInfo("0.1.23-alpha.05.beta", true, 0, 1, 23, "alpha", 5, "beta"),
            new VersionInfo("0.1.23-alpha.5.beta.09", true, 0, 1, 23, "alpha", 5, "beta", 9),
            // empty pre-releases
            new VersionInfo("1.2.3-", true, 1, 2, 3),
            new VersionInfo("1.2.3-0.", true, 1, 2, 3, 0),
            new VersionInfo("1.2.3-0.pre..1", true, 1, 2, 3, 0, "pre", 1),

            // alphanumeric pre-releases
            new VersionInfo("7.12.80-rc-1", 7, 12, 80, "rc-1"),
            new VersionInfo("7.12.80-alpha5.beta", 7, 12, 80, "alpha5", "beta"),
            new VersionInfo("7.12.80-alpha.01-beta", 7, 12, 80, "alpha", "01-beta"),
            // alphanumeric pre-releases with hyphens
            new VersionInfo("7.12.80--rc-1", 7, 12, 80, "-rc-1"),
            new VersionInfo("7.12.80-rc-1-", 7, 12, 80, "rc-1-"),
            new VersionInfo("7.12.80--rc-1-", 7, 12, 80, "-rc-1-"),
            new VersionInfo("7.12.80---rc-1", 7, 12, 80, "--rc-1"),
            new VersionInfo("7.12.80-rc-1--", 7, 12, 80, "rc-1--"),
            new VersionInfo("7.12.80---rc-1--", 7, 12, 80, "--rc-1--"),
            // *numeric* pre-releases with hyphens and leading zeroes
            new VersionInfo("5.6.0--1", 5, 6, 0, "-1"),
            new VersionInfo("5.6.0---1", 5, 6, 0, "--1"),
            new VersionInfo("5.6.0--1.65", 5, 6, 0, "-1", 65),
            new VersionInfo("5.6.0-1.-65", 5, 6, 0, 1, "-65"),
            new VersionInfo("5.6.0--01", 5, 6, 0, "-01"),
            new VersionInfo("5.6.0--01.65", 5, 6, 0, "-01", 65),
            new VersionInfo("5.6.0--01.065", true, 5, 6, 0, "-01", 65),
            new VersionInfo("5.6.0-01.-65", true, 5, 6, 0, 1, "-65"),

            // build metadata
            new VersionInfo("4.0.2+build", 4, 0, 2, "+build"),
            new VersionInfo("4.0.2+build.2", 4, 0, 2, "+build", "2"),
            new VersionInfo("4.0.2+build.002", 4, 0, 2, "+build", "002"),
            new VersionInfo("4.0.2+test-build.1", 4, 0, 2, "+test-build", "1"),
            new VersionInfo("4.0.2+-test-build--.1", 4, 0, 2, "+-test-build--", "1"),
            new VersionInfo("4.0.2+-test-build--.001", 4, 0, 2, "+-test-build--", "001"),
            new VersionInfo("4.0.2+-test-build--.-01", 4, 0, 2, "+-test-build--", "-01"),
            // empty build metadata
            new VersionInfo("4.0.2+", true, 4, 0, 2),
            new VersionInfo("4.0.2+0.", true, 4, 0, 2, "+0"),
            new VersionInfo("4.0.2+0.build..1", true, 4, 0, 2, "+0", "build", "1"),

            // pre-releases and build metadata
            new VersionInfo("0.0.7-pre.3+build.02", 0, 0, 7, "pre", 3, "+build", "02"),
            new VersionInfo("0.0.7-pre.3+build-meta--.02", 0, 0, 7, "pre", 3, "+build-meta--", "02"),
            new VersionInfo("0.0.7-pre-alpha.3+build.-02--", 0, 0, 7, "pre-alpha", 3, "+build", "-02--"),
            new VersionInfo("0.0.7-pre-alpha.-03+build.02", 0, 0, 7, "pre-alpha", "-03", "+build", "02"),
            new VersionInfo("0.0.7-pre-alpha.03+build.02", true, 0, 0, 7, "pre-alpha", 3, "+build", "02"),

            // leading and trailing whitespace
            new VersionInfo("  1.7.10-alpha.5  ", true, 1, 7, 10, "alpha", 5),
            new VersionInfo("\r\n \t1.7.10-alpha.5\t \n\r", true, 1, 7, 10, "alpha", 5),
            // leftovers
            new VersionInfo("1.7.5-pre.2+build$$", true, 1, 7, 5, "pre", 2, "+build"),
            new VersionInfo("\r\n \t1.7.5-pre.2+build\t \n\r$$", true, 1, 7, 5, "pre", 2, "+build"),

            // inner whitespace
            new VersionInfo("1 . 2 . 5 - alpha . 6 . dev", true, 1, 2, 5, "alpha", 6, "dev"),
            new VersionInfo("1\r . \t2 .\n 5 \n-\t alpha \n. 6\r \n. \tdev", true, 1, 2, 5, "alpha", 6, "dev"),

            // optional pre-release separator
            new VersionInfo("0.6.7alpha", true, 0, 6, 7, "alpha"),
            new VersionInfo("0.6.7beta5alpha", true, 0, 6, 7, "beta", 5, "alpha"),
            new VersionInfo("0.6.7beta5alpha+build.007", true, 0, 6, 7, "beta", 5, "alpha", "+build", "007"),
            new VersionInfo("0.6.7 0alpha7", true, 0, 6, 7, 0, "alpha", 7),
        });
    }
}
