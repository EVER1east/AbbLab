namespace AbbLab.SemanticVersioning
{
    public readonly struct SemanticPreRelease
    {
        internal readonly string? text;
        internal readonly int number;

        public bool IsNumeric => text is null;



    }
}