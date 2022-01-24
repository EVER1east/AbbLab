using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AbbLab.Extensions;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public sealed partial class PartialVersion : IEquatable<PartialVersion>
    {
        public PartialComponent Major { get; }
        public PartialComponent Minor { get; }
        public PartialComponent Patch { get; }

        internal readonly SemanticPreRelease[] _preReleases;
        internal readonly string[] _buildMetadata;
        internal ReadOnlyCollection<SemanticPreRelease>? _preReleasesReadonly;
        internal ReadOnlyCollection<string>? _buildMetadataReadonly;
        /// <summary>
        ///   <para>Gets a read-only collection of the pre-release identifiers of the partial semantic version.</para>
        /// </summary>
        public ReadOnlyCollection<SemanticPreRelease> PreReleases
        {
            get
            {
                if (_preReleasesReadonly is not null) return _preReleasesReadonly;
                if (_preReleases.Length is 0)
                    return _preReleasesReadonly = ReadOnlyCollection.Empty<SemanticPreRelease>();
                return _preReleasesReadonly = new ReadOnlyCollection<SemanticPreRelease>(_preReleases);
            }
        }
        /// <summary>
        ///   <para>Gets a read-only collection of the build metadata identifiers of the partial semantic version.</para>
        /// </summary>
        public ReadOnlyCollection<string> BuildMetadata
        {
            get
            {
                if (_buildMetadataReadonly is not null) return _buildMetadataReadonly;
                if (_buildMetadata.Length is 0)
                    return _buildMetadataReadonly = ReadOnlyCollection.Empty<string>();
                return _buildMetadataReadonly = new ReadOnlyCollection<string>(_buildMetadata);
            }
        }

        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>?)null, null) { }
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              params string[] preReleases)
            : this(major, minor, patch, preReleases, null) { }
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              params SemanticPreRelease[] preReleases)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>)preReleases, null) { }
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              [InstantHandle] IEnumerable<string>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              [InstantHandle] IEnumerable<SemanticPreRelease>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              [InstantHandle] IEnumerable<string>? preReleases,
                              [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            // if one component is omitted, all subsequent components must be omitted as well
            if (major.IsOmitted && !minor.IsOmitted) throw new ArgumentException(
                "If the major version component is omitted, the minor version component must be omitted as well.", nameof(minor));
            if (minor.IsOmitted && !patch.IsOmitted) throw new ArgumentException(
                "If the minor version component is omitted, the patch version component must be omitted as well.", nameof(patch));
            // if one component is omitted/a wildcard, subsequent components cannot have explicit values
            if (!major.IsNumeric && minor.IsNumeric) throw new ArgumentException(
                "If the major version component is a wildcard, the minor version component must be a wildcard or omitted.", nameof(minor));
            if (!minor.IsNumeric && patch.IsNumeric) throw new ArgumentException(
                "If the minor version component is a wildcard, the patch version component must be a wildcard or omitted.", nameof(patch));
            Major = major;
            Minor = minor;
            Patch = patch;

            // TODO: version cannot have pre-releases or metadata, if any component is omitted
            string[] preReleasesArray;
            if (preReleases is not null && (preReleasesArray = preReleases.ToArray()).Length > 0)
                _preReleases = Array.ConvertAll(preReleasesArray, SemanticPreRelease.Parse);
            else _preReleases = Array.Empty<SemanticPreRelease>();
            SetBuildMetadata(out _buildMetadata, buildMetadata);
        }
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              [InstantHandle] IEnumerable<SemanticPreRelease>? preReleases,
                              [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            // if one component is omitted, all subsequent components must be omitted as well
            if (major.IsOmitted && !minor.IsOmitted) throw new ArgumentException(
                "If the major version component is omitted, the minor version component must be omitted as well.", nameof(minor));
            if (minor.IsOmitted && !patch.IsOmitted) throw new ArgumentException(
                "If the minor version component is omitted, the patch version component must be omitted as well.", nameof(patch));
            // if one component is omitted/a wildcard, subsequent components cannot have explicit values
            if (!major.IsNumeric && minor.IsNumeric) throw new ArgumentException(
                "If the major version component is a wildcard, the minor version component must be a wildcard or omitted.", nameof(minor));
            if (!minor.IsNumeric && patch.IsNumeric) throw new ArgumentException(
                "If the minor version component is a wildcard, the patch version component must be a wildcard or omitted.", nameof(patch));
            Major = major;
            Minor = minor;
            Patch = patch;

            // TODO: version cannot have pre-releases or metadata, if any component is omitted
            SemanticPreRelease[] preReleasesArray;
            if (preReleases is not null && (preReleasesArray = preReleases.ToArray()).Length > 0)
                _preReleases = preReleasesArray;
            else _preReleases = Array.Empty<SemanticPreRelease>();
            SetBuildMetadata(out _buildMetadata, buildMetadata);
        }

        private static void SetBuildMetadata(out string[] backingField, IEnumerable<string>? buildMetadata)
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
            else backingField = Array.Empty<string>();
        }

        // A constructor that is used internally, to minimize memory allocation
        internal PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                                SemanticPreRelease[]? preReleases, string[]? buildMetadata)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            _preReleases = preReleases ?? Array.Empty<SemanticPreRelease>();
            _buildMetadata = buildMetadata ?? Array.Empty<string>();
        }

        public PartialVersion(Version systemVersion)
        {
            Major = new PartialComponent(systemVersion.Major);
            Minor = new PartialComponent(systemVersion.Minor);
            Patch = systemVersion.Build > -1 ? new PartialComponent(systemVersion.Build) : PartialComponent.Omitted;
            _preReleases = Array.Empty<SemanticPreRelease>();
            _buildMetadata = Array.Empty<string>();
        }
        public PartialVersion(SemanticVersion semanticVersion)
        {
            Major = new PartialComponent(semanticVersion.Major);
            Minor = new PartialComponent(semanticVersion.Minor);
            Patch = new PartialComponent(semanticVersion.Patch);
            _preReleases = semanticVersion._preReleases;
            _buildMetadata = semanticVersion._buildMetadata;
            _preReleasesReadonly = semanticVersion._preReleasesReadonly;
            _buildMetadataReadonly = semanticVersion._buildMetadataReadonly;
        }
        [Pure] public static explicit operator PartialVersion(Version systemVersion)
            => new PartialVersion(systemVersion);
        [Pure] public static implicit operator PartialVersion(SemanticVersion semanticVersion)
            => new PartialVersion(semanticVersion);

        [Pure] public bool Equals(PartialVersion? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null || Major != other.Major || Minor != other.Minor || Patch != other.Patch) return false;
            int length = _preReleases.Length;
            if (length != other._preReleases.Length) return false;
            for (int i = 0; i < length; i++)
                if (!_preReleases[i].Equals(other._preReleases[i]))
                    return false;
            return true;
        }
        [Pure] public override bool Equals(object? obj) => obj is PartialVersion other && Equals(other);
        [Pure] public override int GetHashCode()
        {
            if (_preReleases.Length is 0)
                return HashCode.Combine(Major, Minor, Patch);
            HashCode hash = new HashCode();
            hash.Add(Major);
            hash.Add(Minor);
            hash.Add(Patch);
            for (int i = 0, length = _preReleases.Length; i < length; i++)
                hash.Add(_preReleases[i]);
            return hash.ToHashCode();
        }

        /// <summary>
        ///   <para>Determines whether two partial semantic versions are equal.</para>
        /// </summary>
        /// <param name="a">The first semantic version to compare.</param>
        /// <param name="b">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> and <paramref name="b"/> are equal; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(PartialVersion? a, PartialVersion? b) => a is null ? b is null : a.Equals(b);
        /// <summary>
        ///   <para>Determines whether two partial semantic versions are not equal.</para>
        /// </summary>
        /// <param name="a">The first semantic version to compare.</param>
        /// <param name="b">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> and <paramref name="b"/> are not equal; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(PartialVersion? a, PartialVersion? b) => a is null ? b is not null : !a.Equals(b);

    }
}