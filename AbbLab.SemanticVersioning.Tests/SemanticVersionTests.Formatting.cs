using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Theory]
        [MemberData(nameof(ParseFixtures))]
        public void ToStringTests(VersionParseTest test)
        {
            if (test.IsValid) // check only semantically valid versions
            {
                Output.WriteLine($"Formatting `{test.Semantic}`.");
                SemanticVersion version = SemanticVersion.Parse(test.Semantic);

                Assert.Equal(test.Semantic, version.ToString());
                Assert.Equal(test.Semantic, version.ToString(null));
                Assert.Equal(test.Semantic, version.ToString("G"));
                Assert.Equal(test.Semantic, version.ToString("g"));
                Assert.Equal(test.Semantic, version.ToString("M.m.p-ppp+mmm"));

                IFormattable format = version;
                Assert.Equal(test.Semantic, format.ToString(null, null));
                Assert.Equal(test.Semantic, format.ToString("G", null));
                Assert.Equal(test.Semantic, format.ToString("g", null));
                Assert.Equal(test.Semantic, format.ToString("M.m.p-ppp+mmm", null));
            }
        }

        [Theory]
        [MemberData(nameof(FormatFixtures))]
        public void FormatTests(VersionFormatTest test)
        {
            Output.WriteLine($"Formatting `{test.Semantic}` using `{test.Format}`.");
            SemanticVersion version = SemanticVersion.Parse(test.Semantic);

            Assert.Equal(test.Expected, version.ToString(test.Format));
            IFormattable format = version;
            Assert.Equal(test.Expected, format.ToString(test.Format, null));
            Assert.Equal(test.Expected, format.ToString(test.Format, CultureInfo.InvariantCulture));
        }

        public static readonly IEnumerable<object[]> FormatFixtures = Util.Arrayify(new VersionFormatTest[]
        {
            new VersionFormatTest("1.2.3", "M.m.p", "1.2.3"),
            new VersionFormatTest("1.2.3", "M.m.pp", "1.2.3"),
            new VersionFormatTest("1.2.0", "M.m.pp", "1.2"),
            new VersionFormatTest("1.2.3", "M.mm.pp", "1.2.3"),
            new VersionFormatTest("1.2.0", "M.mm.pp", "1.2"),
            new VersionFormatTest("1.0.0", "M.mm.pp", "1"),

            new VersionFormatTest("1.0.0", "MM.mm.pp", "1"),
            new VersionFormatTest("0.0.0", "MM.mm.pp", ""),

            new VersionFormatTest("1.2.3-alpha.5+build.02", "M.m.p", "1.2.3"),
            new VersionFormatTest("1.2.3-alpha.5+build.02", "M.m.p-ppp", "1.2.3-alpha.5"),
            new VersionFormatTest("1.2.3-alpha.5+build.02", "M.m.p+mmm", "1.2.3+build.02"),

            new VersionFormatTest("1.2.3", "M-m-p", "1-2-3"),
            new VersionFormatTest("1.2.0", "M-m-pp", "1-2"),
            new VersionFormatTest("1.0.0", "M-mm-pp", "1"),
            new VersionFormatTest("1.2.3", @"M\-m\-p", "1-2-3"),
            new VersionFormatTest("1.2.0", @"M\-m\-pp", "1-2-"),
            new VersionFormatTest("1.0.0", @"M\-mm\-pp", "1--"),
            new VersionFormatTest("1.2.3", @"M.m.p\-ppp", "1.2.3-"),
            new VersionFormatTest("1.2.3", @"M.m.p\+mmm", "1.2.3+"),
            new VersionFormatTest("1.2.3-alpha.5+build.02", @"M.m.p\-ppp", "1.2.3-alpha.5"),
            new VersionFormatTest("1.2.3-alpha.5+build.02", @"M.m.p\+mmm", "1.2.3+build.02"),

            new VersionFormatTest("1.2.3", "M.test.mm.test.pp", "1.test.2.test.3"),
            new VersionFormatTest("1.2.0", "M.test.mm.test.pp", "1.test.2.test"),
            new VersionFormatTest("1.0.0", "M.test.mm.test.pp", "1.test.test"),

            new VersionFormatTest("1.23.456", "M.m.p", "1.23.456"),
            new VersionFormatTest("1000.20000.300000", "M.m.p", "1000.20000.300000"),
            new VersionFormatTest("4000000.50000000.600000000", "M.m.p", "4000000.50000000.600000000"),
            new VersionFormatTest("2147483647.2147483647.2147483647", "M.m.p", "2147483647.2147483647.2147483647"),

        });

        public readonly struct VersionFormatTest
        {
            public string Semantic { get; }
            public string? Format { get; }
            public string Expected { get; }

            public VersionFormatTest(string semantic, string? format, string expected)
            {
                Semantic = semantic;
                Format = format;
                Expected = expected;
            }
        }

    }
}
