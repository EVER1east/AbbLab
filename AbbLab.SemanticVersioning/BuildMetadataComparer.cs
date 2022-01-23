using System;
using System.Collections;
using System.Collections.Generic;

namespace AbbLab.SemanticVersioning
{
    public sealed class BuildMetadataComparer
        : IEqualityComparer<SemanticVersion>, IComparer<SemanticVersion>,
          IEqualityComparer, IComparer
    {
        private BuildMetadataComparer() { }

        public static readonly BuildMetadataComparer Instance = new BuildMetadataComparer();

        public bool Equals(SemanticVersion? a, SemanticVersion? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null || a.Major != b.Major || a.Minor != b.Minor || a.Patch != b.Patch) return false;

            int preReleasesLength = a._preReleases.Length;
            if (preReleasesLength != b._preReleases.Length) return false;
            int buildMetadataLength = a._buildMetadata.Length;
            if (buildMetadataLength != b._buildMetadata.Length) return false;

            for (int i = 0; i < preReleasesLength; i++)
                if (!a._preReleases[i].Equals(b._preReleases[i]))
                    return false;
            for (int i = 0; i < buildMetadataLength; i++)
                if (!a._buildMetadata[i].Equals(b._buildMetadata[i]))
                    return false;
            return true;
        }
        public int GetHashCode(SemanticVersion version)
        {
            int preReleasesLength = version._preReleases.Length;
            int buildMetadataLength = version._buildMetadata.Length;
            if (preReleasesLength is 0 && buildMetadataLength is 0)
                return HashCode.Combine(version.Major, version.Minor, version.Patch);

            HashCode hash = new HashCode();
            hash.Add(version.Major);
            hash.Add(version.Minor);
            hash.Add(version.Patch);

            for (int i = 0; i < preReleasesLength; i++)
                hash.Add(version._preReleases[i]);
            for (int i = 0; i < buildMetadataLength; i++)
                hash.Add(version._buildMetadata[i]);

            return hash.ToHashCode();
        }
        public int Compare(SemanticVersion? a, SemanticVersion? b)
        {
            if (ReferenceEquals(a, b)) return 0;
            if (a is null) return -1;
            if (b is null) return 1;

            int res = a.Major.CompareTo(b.Major);
            if (res is not 0) return res;
            res = a.Minor.CompareTo(b.Minor);
            if (res is not 0) return res;
            res = a.Patch.CompareTo(b.Patch);
            if (res is not 0) return res;

            int aLength = a._preReleases.Length;
            int bLength = b._preReleases.Length;
            if (aLength is 0 && bLength > 0) return 1;
            if (aLength > 0 && bLength is 0) return -1;
            int length = Math.Max(aLength, bLength);
            for (int i = 0; i < length; i++)
            {
                if (i == aLength) return i == bLength ? 0 : -1;
                if (i == bLength) return 1;
                res = a._preReleases[i].CompareTo(b._preReleases[i]);
                if (res is not 0) return res;
            }

            aLength = a._buildMetadata.Length;
            bLength = b._buildMetadata.Length;
            if (aLength is 0 && bLength > 0) return 1;
            if (aLength > 0 && bLength is 0) return -1;
            length = Math.Max(aLength, bLength);
            for (int i = 0; i < length; i++)
            {
                if (i == aLength) return i == bLength ? 0 : -1;
                if (i == bLength) return 1;
                res = string.CompareOrdinal(a._buildMetadata[i], b._buildMetadata[i]);
                if (res is not 0) return res;
            }
            return 0;
        }

        bool IEqualityComparer.Equals(object? a, object? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;

            if (a is SemanticVersion aSem && b is SemanticVersion bSem)
                return Equals(aSem, bSem);

            return a.Equals(b);
        }
        int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is null) return 0;

            if (obj is SemanticVersion version) return GetHashCode(version);

            return obj.GetHashCode();
        }
        int IComparer.Compare(object? a, object? b)
        {
            if (ReferenceEquals(a, b)) return 0;

            if (a is SemanticVersion aSem && b is SemanticVersion bSem)
                return Compare(aSem, bSem);

            if (a is IComparable comparable)
                return comparable.CompareTo(b);
            throw new ArgumentException($"The objects must be of type {nameof(SemanticVersion)} or implement {nameof(IComparable)}.");
        }

    }
}
