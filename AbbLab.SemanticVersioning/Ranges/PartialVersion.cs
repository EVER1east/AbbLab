using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AbbLab.Extensions;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    /// <summary>
    ///   <para>Represents a partial version – a semantic version with version components, that can be omitted or represented by wildcards.</para>
    /// </summary>
    public sealed partial class PartialVersion : IEquatable<PartialVersion>
    {
        /// <summary>
        ///   <para>Gets the major version component of the partial version.</para>
        /// </summary>
        public PartialComponent Major { get; }
        /// <summary>
        ///   <para>Gets the minor version component of the partial version.</para>
        /// </summary>
        public PartialComponent Minor { get; }
        /// <summary>
        ///   <para>Gets the patch version component of the partial version.</para>
        /// </summary>
        public PartialComponent Patch { get; }

        internal readonly SemanticPreRelease[] _preReleases;
        internal readonly string[] _buildMetadata;
        internal ReadOnlyCollection<SemanticPreRelease>? _preReleasesReadonly;
        internal ReadOnlyCollection<string>? _buildMetadataReadonly;
        /// <summary>
        ///   <para>Gets a read-only collection of the pre-release identifiers of the partial version.</para>
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
        ///   <para>Gets a read-only collection of the build metadata identifiers of the partial version.</para>
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

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> structure with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>?)null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> structure with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">An array of string representation of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              params string[] preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> structure with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">An array of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              params SemanticPreRelease[] preReleases)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>)preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> structure with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of string representation of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              [InstantHandle] IEnumerable<string>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> structure with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              [InstantHandle] IEnumerable<SemanticPreRelease>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> structure with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of string representation of pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of build metadata identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier, or <paramref name="buildMetadata"/> contains an invalid build metadata identifier.</exception>
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
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> structure with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of build metadata identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="buildMetadata"/> contains an invalid build metadata identifier.</exception>
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

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class with the specified <paramref name="systemVersion"/>'s major, minor and build components.</para>
        /// </summary>
        /// <param name="systemVersion">The <see cref="Version"/> to use the major, minor and build components of.</param>
        public PartialVersion(Version systemVersion)
        {
            Major = new PartialComponent(systemVersion.Major);
            Minor = new PartialComponent(systemVersion.Minor);
            Patch = systemVersion.Build > -1 ? new PartialComponent(systemVersion.Build) : PartialComponent.Omitted;
            _preReleases = Array.Empty<SemanticPreRelease>();
            _buildMetadata = Array.Empty<string>();
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class with the specified <paramref name="semanticVersion"/>'s version components and pre-release and build metadata identifiers.</para>
        /// </summary>
        /// <param name="semanticVersion">The <see cref="SemanticVersion"/> to use the version components and pre-release and build metadata identifiers of.</param>
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
        /// <summary>
        ///   <para>Defines an explicit conversion of a <see cref="Version"/> to a <see cref="PartialVersion"/>.</para>
        /// </summary>
        /// <param name="systemVersion">The <see cref="Version"/> to convert to a <see cref="PartialVersion"/>.</param>
        [Pure] public static explicit operator PartialVersion(Version systemVersion)
            => new PartialVersion(systemVersion);
        /// <summary>
        ///   <para>Defines an implicit conversion of a <see cref="SemanticVersion"/> to a <see cref="PartialVersion"/>.</para>
        /// </summary>
        /// <param name="semanticVersion">The <see cref="SemanticVersion"/> to convert to a <see cref="PartialVersion"/>.</param>
        [Pure] public static implicit operator PartialVersion(SemanticVersion semanticVersion)
            => new PartialVersion(semanticVersion);

        /// <inheritdoc/>
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
        /// <inheritdoc/>
        [Pure] public override bool Equals(object? obj) => obj is PartialVersion other && Equals(other);
        /// <inheritdoc/>
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
        /// <param name="a">The first partial version to compare.</param>
        /// <param name="b">The second partial version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> and <paramref name="b"/> are equal; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(PartialVersion? a, PartialVersion? b) => a is null ? b is null : a.Equals(b);
        /// <summary>
        ///   <para>Determines whether two partial semantic versions are not equal.</para>
        /// </summary>
        /// <param name="a">The first partial version to compare.</param>
        /// <param name="b">The second partial version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> and <paramref name="b"/> are not equal; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(PartialVersion? a, PartialVersion? b) => a is null ? b is not null : !a.Equals(b);

    }
}