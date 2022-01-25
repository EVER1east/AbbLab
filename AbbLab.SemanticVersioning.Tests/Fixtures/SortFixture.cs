namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        public static readonly string?[] SortFixture =
        {
            null,

            "0.0.0-0",
            "0.0.0-1",
            "0.0.0-45",
            "0.0.0-alpha",
            "0.0.0-alpha.0",
            "0.0.0-alpha.1",
            "0.0.0-alpha.beta",
            "0.0.0-rc.0",
            "0.0.0-rc.0.0",
            "0.0.0-rc.0.1",
            "0.0.0-rc.1.0",
            "0.0.0-rc.1.1",
            "0.0.0",
            "0.0.1-alpha",
            "0.0.1",
            "0.0.2-5",
            "0.0.2",
            "0.1.0-dev",
            "0.1.0",

            "1.0.0-beta.6",
            "1.0.0",
            "1.0.1-beta",
            "1.0.1-rc.1",
            "1.0.1",
            "1.1.0-alpha",
            "1.1.0-alpha.beta",
            "1.1.0-alpha.beta.7",

            "2.0.0-0",
            "2.0.0-0.dev",
            "2.0.0-0.dev.0",
            "2.0.0-0.dev.5",
            "2.0.0-1",
            "2.0.0-1.rc",
            "2.0.0-rc",
            "2.0.0",

        };
    }
}
