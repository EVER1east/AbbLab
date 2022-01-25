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

            // alphabetic pre-releases
            new VersionInfo("1.2.3-alpha", 1, 2, 3, "alpha"),
            new VersionInfo("1.2.3-beta", 1, 2, 3, "beta"),
            new VersionInfo("1.2.3-beta.alpha.dev", 1, 2, 3, "beta", "alpha", "dev"),
            // numeric pre-releases
            new VersionInfo("1.2.3-0", 1, 2, 3, 0),
            new VersionInfo("1.2.3-72", 1, 2, 3, 72),
            new VersionInfo("1.2.3-72.0.9", 1, 2, 3, 72, 0, 9),
            // numeric pre-releases with leading zeroes
            new VersionInfo("1.2.3-00", false),
            new VersionInfo("1.2.3-000", false),
            new VersionInfo("1.2.3-072", false),
            new VersionInfo("1.2.3-0072", false),
            new VersionInfo("1.2.3-72.0.09", false),
            // alphabetic and numeric pre-releases
            new VersionInfo("0.1.23-alpha.5", 0, 1, 23, "alpha", 5),
            new VersionInfo("0.1.23-alpha.5.beta", 0, 1, 23, "alpha", 5, "beta"),
            new VersionInfo("0.1.23-alpha.5.beta.9", 0, 1, 23, "alpha", 5, "beta", 9),
            new VersionInfo("0.1.23-alpha.05.beta", false),
            new VersionInfo("0.1.23-alpha.5.beta.09", false),

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
            new VersionInfo("5.6.0--01.065", false),
            new VersionInfo("5.6.0-01.-65", false),

            // build metadata
            new VersionInfo("4.0.2+build", 4, 0, 2, "+build"),
            new VersionInfo("4.0.2+build.2", 4, 0, 2, "+build", "2"),
            new VersionInfo("4.0.2+build.002", 4, 0, 2, "+build", "002"),
            new VersionInfo("4.0.2+test-build.1", 4, 0, 2, "+test-build", "1"),
            new VersionInfo("4.0.2+-test-build--.1", 4, 0, 2, "+-test-build--", "1"),
            new VersionInfo("4.0.2+-test-build--.001", 4, 0, 2, "+-test-build--", "001"),
            new VersionInfo("4.0.2+-test-build--.-01", 4, 0, 2, "+-test-build--", "-01"),

            // pre-releases and build metadata
            new VersionInfo("0.0.7-pre.3+build.02", 0, 0, 7, "pre", 3, "+build", "02"),
            new VersionInfo("0.0.7-pre.3+build-meta--.02", 0, 0, 7, "pre", 3, "+build-meta--", "02"),
            new VersionInfo("0.0.7-pre-alpha.3+build.-02--", 0, 0, 7, "pre-alpha", 3, "+build", "-02--"),
            new VersionInfo("0.0.7-pre-alpha.-03+build.02", 0, 0, 7, "pre-alpha", "-03", "+build", "02"),
            new VersionInfo("0.0.7-pre-alpha.03+build.02", false),

        });
    }
}
