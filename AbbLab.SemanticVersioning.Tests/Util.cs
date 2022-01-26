using System;
using System.Collections.Generic;
using System.Linq;

namespace AbbLab.SemanticVersioning.Tests
{
    internal static class Util
    {
        // ReSharper disable once IdentifierTypo
        public static IEnumerable<object[]> Arrayify<T>(IEnumerable<T> versions)
            => versions.Select(static v => new object[1] { v! }).ToArray();

        public static SemanticPreRelease[] SeparateIdentifiers(object[] identifiers, out string[] buildMetadata)
        {
            static SemanticPreRelease Convert(object obj) => obj is string str ? new SemanticPreRelease(str) : (int)obj;

            // build metadata begins with the first identifier with leading '+'
            int metadataStart = Array.FindIndex(identifiers, static i => i is string { Length: > 0 } str && str[0] == '+');
            if (metadataStart is -1)
            {
                buildMetadata = Array.Empty<string>();
                return Array.ConvertAll(identifiers, Convert);
            }
            buildMetadata = identifiers[metadataStart..].Cast<string>().ToArray();
            buildMetadata[0] = buildMetadata[0][1..]; // remove the leading '+'
            return Array.ConvertAll(identifiers[..metadataStart], Convert);
        }

        public static void AssertVersion(SemanticVersion version, int major, int minor, int patch,
                                         IEnumerable<SemanticPreRelease>? preReleases = null,
                                         IEnumerable<string>? buildMetadata = null)
        {
            Xunit.Assert.Equal(major, version.Major);
            Xunit.Assert.Equal(minor, version.Minor);
            Xunit.Assert.Equal(patch, version.Patch);
            if (preReleases is null) Xunit.Assert.Empty(version.PreReleases);
            else Xunit.Assert.Equal(preReleases, version.PreReleases);
            if (buildMetadata is null) Xunit.Assert.Empty(version.BuildMetadata);
            else Xunit.Assert.Equal(buildMetadata, version.BuildMetadata);
        }

    }
}
