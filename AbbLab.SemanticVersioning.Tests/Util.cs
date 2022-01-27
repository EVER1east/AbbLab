using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
            Assert.Equal(major, version.Major);
            Assert.Equal(minor, version.Minor);
            Assert.Equal(patch, version.Patch);
            if (preReleases is null) Assert.Empty(version.PreReleases);
            else Assert.Equal(preReleases, version.PreReleases);
            if (buildMetadata is null) Assert.Empty(version.BuildMetadata);
            else Assert.Equal(buildMetadata, version.BuildMetadata);
        }
        public static void AssertPreRelease(SemanticPreRelease preRelease, int value)
        {
            Assert.True(preRelease.IsNumeric);
            Assert.Equal(value, preRelease.Number);
            Assert.Equal(value, (int)preRelease);
            Assert.Throws<InvalidOperationException>(() => preRelease.Text);
        }
        public static void AssertPreRelease(SemanticPreRelease preRelease, string value, bool stringSource = true)
        {
            Assert.False(preRelease.IsNumeric);
            Assert.Equal(value, preRelease.Text);
            Assert.Equal(value, (string)preRelease);
            if (stringSource)
            {
                Assert.Same(value, preRelease.Text);
                Assert.Same(value, (string)preRelease);
            }
            else
            {
                Assert.NotSame(value, preRelease.Text);
                Assert.NotSame(value, (string)preRelease);
            }
            Assert.Throws<InvalidOperationException>(() => preRelease.Number);
        }
        public static void AssertPreRelease(SemanticPreRelease preRelease, object value, bool stringSource = true)
        {
            if (value is string str) AssertPreRelease(preRelease, str, stringSource);
            else AssertPreRelease(preRelease, (int)value);
        }

    }
}
