using System.Collections.Generic;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        public static readonly IEnumerable<object[]> FormatFixture = Util.Arrayify(new VersionFormatTest[]
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

            new VersionFormatTest("1.2.3", @"M.test.mm.test.pp", "1.test.2.test.3"),
            new VersionFormatTest("1.2.0", @"M.test.mm.test.pp", "1.test.2.test"),
            new VersionFormatTest("1.0.0", @"M.test.mm.test.pp", "1.test.test"),

        });
    }
}
