using System;
using System.Linq;

namespace AbbLab.SemanticVersioning.Tests
{
    public readonly struct VersionTest
    {
        public string Semantic { get; }
        public bool IsValid { get; }
        public bool IsValidLoose { get; }
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public object[] PreReleases { get; }
        public string[] BuildMetadata { get; }

        public VersionTest(string semantic)
        {
            Semantic = semantic;
            IsValid = false;
            IsValidLoose = false;
            Major = -1;
            Minor = -1;
            Patch = -1;
            PreReleases = Array.Empty<object>();
            BuildMetadata = Array.Empty<string>();
        }
        public VersionTest(string semantic, bool looseOnly, int major, int minor, int patch, params object[] identifiers)
            : this(semantic, major, minor, patch, identifiers)
        {
            if (looseOnly)
            {
                IsValid = false;
                IsValidLoose = true;
            }
        }
        public VersionTest(string semantic, int major, int minor, int patch, params object[] identifiers)
        {
            Semantic = semantic;
            IsValid = true;
            IsValidLoose = true;
            Major = major;
            Minor = minor;
            Patch = patch;

            // build metadata begins with the first identifier with leading '+'
            int metadataStart = Array.FindIndex(identifiers, static i => i is string str && str[0] == '+');
            if (metadataStart is -1)
            {
                PreReleases = identifiers;
                BuildMetadata = Array.Empty<string>();
            }
            else
            {
                PreReleases = identifiers[..metadataStart];
                BuildMetadata = identifiers[metadataStart..].Cast<string>().ToArray();
                BuildMetadata[0] = BuildMetadata[0][1..]; // remove the leading '+'
            }
        }

        public void Assert(SemanticVersion version)
        {
            Xunit.Assert.Equal(Major, version.Major);
            Xunit.Assert.Equal(Minor, version.Minor);
            Xunit.Assert.Equal(Patch, version.Patch);
            Xunit.Assert.Equal(PreReleases.Length, version.PreReleases.Count);
            for (int i = 0, length = PreReleases.Length; i < length; i++)
            {
                SemanticPreRelease preRelease = version.PreReleases[i];
                Xunit.Assert.Equal(PreReleases[i], preRelease.IsNumeric ? preRelease.Number : preRelease.Text);
            }
            Xunit.Assert.Equal(BuildMetadata, version.BuildMetadata);
        }

    }
}
