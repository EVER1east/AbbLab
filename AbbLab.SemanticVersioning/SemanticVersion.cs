using System.Collections.ObjectModel;

namespace AbbLab.SemanticVersioning
{
    public class SemanticVersion
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        private readonly SemanticPreRelease[] preReleases;
        private readonly string[] buildMetadata;
        public ReadOnlyCollection<SemanticPreRelease> PreReleases { get; }
        public ReadOnlyCollection<string> BuildMetadata { get; }





    }
}
