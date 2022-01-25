namespace AbbLab.SemanticVersioning.Tests
{
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
