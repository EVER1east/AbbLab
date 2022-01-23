using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AbbLab.Extensions;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public partial class SemanticVersion
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        internal readonly SemanticPreRelease[]? _preReleases;
        internal readonly string[]? _buildMetadata;
        internal ReadOnlyCollection<SemanticPreRelease>? _preReleasesReadonly;
        internal ReadOnlyCollection<string>? _buildMetadataReadonly;
        public ReadOnlyCollection<SemanticPreRelease> PreReleases
        {
            get
            {
                if (_preReleasesReadonly is not null) return _preReleasesReadonly;
                if (_preReleases is null || _preReleases.Length is 0)
                    return _preReleasesReadonly = ReadOnlyCollection.Empty<SemanticPreRelease>();
                return _preReleasesReadonly = new ReadOnlyCollection<SemanticPreRelease>(_preReleases);
            }
        }
        public ReadOnlyCollection<string> BuildMetadata
        {
            get
            {
                if (_buildMetadataReadonly is not null) return _buildMetadataReadonly;
                if (_buildMetadata is null || _buildMetadata.Length is 0)
                    return _buildMetadataReadonly = ReadOnlyCollection.Empty<string>();
                return _buildMetadataReadonly = new ReadOnlyCollection<string>(_buildMetadata);
            }
        }

        public SemanticVersion(int major, int minor, int patch)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>?)null, null) { }
        public SemanticVersion(int major, int minor, int patch, params string[] preReleases)
            : this(major, minor, patch, preReleases, null) { }
        public SemanticVersion(int major, int minor, int patch, params SemanticPreRelease[] preReleases)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>)preReleases, null) { }
        public SemanticVersion(int major, int minor, int patch, [InstantHandle] IEnumerable<string>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        public SemanticVersion(int major, int minor, int patch, [InstantHandle] IEnumerable<SemanticPreRelease>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        public SemanticVersion(int major, int minor, int patch,
                               [InstantHandle] IEnumerable<string>? preReleases,
                               [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), major, Exceptions.MajorNegative);
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), minor, Exceptions.MinorNegative);
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), patch, Exceptions.PatchNegative);
            Major = major;
            Minor = minor;
            Patch = patch;

            string[] preReleasesArray;
            if (preReleases is not null && (preReleasesArray = preReleases.ToArray()).Length > 0)
                _preReleases = Array.ConvertAll(preReleasesArray, SemanticPreRelease.Parse);
            SetBuildMetadata(ref _buildMetadata, buildMetadata);
        }
        public SemanticVersion(int major, int minor, int patch,
                               [InstantHandle] IEnumerable<SemanticPreRelease>? preReleases,
                               [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), major, Exceptions.MajorNegative);
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), minor, Exceptions.MinorNegative);
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), patch, Exceptions.PatchNegative);
            Major = major;
            Minor = minor;
            Patch = patch;

            SemanticPreRelease[] preReleasesArray;
            if (preReleases is not null && (preReleasesArray = preReleases.ToArray()).Length > 0)
                _preReleases = preReleasesArray;
            SetBuildMetadata(ref _buildMetadata, buildMetadata);
        }
        private static void SetBuildMetadata(ref string[]? backingField, IEnumerable<string>? buildMetadata)
        {
            string[] buildMetadataArray;
            int length;
            if (buildMetadata is not null && (length = (buildMetadataArray = buildMetadata.ToArray()).Length) > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    string identifier = buildMetadataArray[i];
                    if (identifier.Length is 0)
                        throw new ArgumentException(Exceptions.BuildMetadataEmpty, nameof(buildMetadata));
                    if (!Util.ContainsOnlyValidCharacters(identifier))
                        throw new ArgumentException(Exceptions.BuildMetadataInvalid, nameof(buildMetadata));
                }
                backingField = buildMetadataArray;
            }
        }

        private SemanticVersion(int major, int minor, int patch, SemanticPreRelease[]? preReleases, string[]? buildMetadata)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            _preReleases = preReleases;
            _buildMetadata = buildMetadata;
        }

        public SemanticVersion(Version systemVersion)
        {
            Major = systemVersion.Major;
            Minor = systemVersion.Minor;
            Patch = Math.Max(systemVersion.Build, 0);
        }
        public static explicit operator SemanticVersion(Version systemVersion) => new SemanticVersion(systemVersion);
        public static explicit operator Version(SemanticVersion version) => new Version(version.Major, version.Minor, version.Patch);



    }
}
