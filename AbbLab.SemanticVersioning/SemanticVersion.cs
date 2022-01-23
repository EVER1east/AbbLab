using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AbbLab.Extensions;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    /// <summary>
    ///   <para>Represents a semantic version.</para>
    /// </summary>
    public sealed partial class SemanticVersion
    {
        /// <summary>
        ///   <para>Gets the major version component of the semantic version.</para>
        /// </summary>
        public int Major { get; }
        /// <summary>
        ///   <para>Gets the minor version component of the semantic version.</para>
        /// </summary>
        public int Minor { get; }
        /// <summary>
        ///   <para>Gets the patch version component of the semantic version.</para>
        /// </summary>
        public int Patch { get; }

        internal readonly SemanticPreRelease[]? _preReleases;
        internal readonly string[]? _buildMetadata;
        internal ReadOnlyCollection<SemanticPreRelease>? _preReleasesReadonly;
        internal ReadOnlyCollection<string>? _buildMetadataReadonly;
        /// <summary>
        ///   <para>Gets a read-only collection of the pre-release identifiers of the semantic version.</para>
        /// </summary>
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
        /// <summary>
        ///   <para>Gets a read-only collection of the build metadata identifiers of the semantic version.</para>
        /// </summary>
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

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersion(int major, int minor, int patch)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>?)null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">An array of string representations of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier.</exception>
        public SemanticVersion(int major, int minor, int patch, params string[] preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">An array of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersion(int major, int minor, int patch, params SemanticPreRelease[] preReleases)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>)preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of string representations of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier.</exception>
        public SemanticVersion(int major, int minor, int patch, [InstantHandle] IEnumerable<string>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersion(int major, int minor, int patch, [InstantHandle] IEnumerable<SemanticPreRelease>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of string representations of pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of build metadata identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier, or <paramref name="buildMetadata"/> contains an invalid build metadata identifier.</exception>
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
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of build metadata identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="buildMetadata"/> contains an invalid build metadata identifier.</exception>
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

        // A constructor that is used internally, to minimize memory allocation
        internal SemanticVersion(int major, int minor, int patch, SemanticPreRelease[]? preReleases, string[]? buildMetadata)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            _preReleases = preReleases;
            _buildMetadata = buildMetadata;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class using the specified <paramref name="systemVersion"/>'s major, minor and build components.</para>
        /// </summary>
        /// <param name="systemVersion">The <see cref="Version"/> to use the major, minor and build components of.</param>
        public SemanticVersion(Version systemVersion)
        {
            Major = systemVersion.Major;
            Minor = systemVersion.Minor;
            Patch = Math.Max(systemVersion.Build, 0);
        }
        /// <summary>
        ///   <para>Defines an explicit conversion of a <see cref="Version"/> to a <see cref="SemanticVersion"/>.</para>
        /// </summary>
        /// <param name="systemVersion">The <see cref="Version"/> to convert to a <see cref="SemanticVersion"/>.</param>
        public static explicit operator SemanticVersion(Version systemVersion)
            => new SemanticVersion(systemVersion);
        /// <summary>
        ///   <para>Defines an explicit conversion of a <see cref="SemanticVersion"/> to a <see cref="Version"/>.</para>
        /// </summary>
        /// <param name="semanticVersion">The <see cref="SemanticVersion"/> to convert to a <see cref="Version"/>.</param>
        public static explicit operator Version(SemanticVersion semanticVersion)
            => new Version(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch);



    }
}
