﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using System.Linq;

namespace AbbLab.SemanticVersioning
{
    /// <summary>
    ///   <para>Provides a custom constructor for the <see cref="SemanticVersion"/> class.</para>
    /// </summary>
    public sealed class SemanticVersionBuilder
    {
        private int _major;
        private int _minor;
        private int _patch;

        /// <summary>
        ///   <para>Gets or sets the major version component of the semantic version.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Attempted to set to a value less than 0.</exception>
        public int Major
        {
            get => _major;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.MajorNegative);
                _major = value;
            }
        }
        /// <summary>
        ///   <para>Gets or sets the minor version component of the semantic version.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Attempted to set to a value less than 0.</exception>
        public int Minor
        {
            get => _minor;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.MinorNegative);
                _minor = value;
            }
        }
        /// <summary>
        ///   <para>Gets or sets the patch version component of the semantic version.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Attempted to set to a value less than 0.</exception>
        public int Patch
        {
            get => _patch;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.PatchNegative);
                _patch = value;
            }
        }

        private List<SemanticPreRelease>? _preReleases;
        private List<string>? _buildMetadata;
        private ReadOnlyCollection<SemanticPreRelease>? _preReleasesReadonly;
        private ReadOnlyCollection<string>? _buildMetadataReadonly;
        public ReadOnlyCollection<SemanticPreRelease> PreReleases
        {
            get
            {
                if (_preReleasesReadonly is not null) return _preReleasesReadonly;
                _preReleases ??= new List<SemanticPreRelease>();
                return _preReleasesReadonly = new ReadOnlyCollection<SemanticPreRelease>(_preReleases);
            }
        }
        public ReadOnlyCollection<string> BuildMetadata
        {
            get
            {
                if (_buildMetadataReadonly is not null) return _buildMetadataReadonly;
                _buildMetadata ??= new List<string>();
                return _buildMetadataReadonly = new ReadOnlyCollection<string>(_buildMetadata);
            }
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class.</para>
        /// </summary>
        public SemanticVersionBuilder() { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch)
            : this(major, minor, patch, (IEnumerable<SemanticPreRelease>?)null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">An array of string representations of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch, params string[] preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">An array of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch, params SemanticPreRelease[] preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of string representations of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch, [InstantHandle] IEnumerable<string>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch, [InstantHandle] IEnumerable<SemanticPreRelease>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of string representations of pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of build metadata identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="preReleases"/> contains an invalid string representation of a pre-release identifier, or <paramref name="buildMetadata"/> contains an invalid build metadata identifier.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch,
                                      [InstantHandle] IEnumerable<string>? preReleases,
                                      [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), major, Exceptions.MajorNegative);
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), minor, Exceptions.MinorNegative);
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), patch, Exceptions.PatchNegative);
            Major = major;
            Minor = minor;
            Patch = patch;

            List<string> preReleasesArray;
            if (preReleases is not null && (preReleasesArray = preReleases.ToList()).Count > 0)
                _preReleases = preReleasesArray.ConvertAll(SemanticPreRelease.Parse);
            SetBuildMetadata(ref _buildMetadata, buildMetadata);
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="preReleases">A collection of pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of build metadata identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="buildMetadata"/> contains an invalid build metadata identifier.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch,
                                      [InstantHandle] IEnumerable<SemanticPreRelease>? preReleases,
                                      [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), major, Exceptions.MajorNegative);
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), minor, Exceptions.MinorNegative);
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), patch, Exceptions.PatchNegative);
            Major = major;
            Minor = minor;
            Patch = patch;

            List<SemanticPreRelease> preReleasesArray;
            if (preReleases is not null && (preReleasesArray = preReleases.ToList()).Count > 0)
                _preReleases = preReleasesArray;
            SetBuildMetadata(ref _buildMetadata, buildMetadata);
        }

        private static void SetBuildMetadata(ref List<string>? backingField, IEnumerable<string>? buildMetadata)
        {
            List<string> buildMetadataArray;
            int length;
            if (buildMetadata is not null && (length = (buildMetadataArray = buildMetadata.ToList()).Count) > 0)
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

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified semantic <paramref name="version"/>.</para>
        /// </summary>
        /// <param name="version">The semantic version to initialize the <see cref="SemanticVersionBuilder"/> with.</param>
        public SemanticVersionBuilder(SemanticVersion version)
        {
            _major = version.Major;
            _minor = version.Minor;
            _patch = version.Patch;
            if (version._preReleases.Length > 0)
                _preReleases = new List<SemanticPreRelease>(version._preReleases);
            if (version._buildMetadata.Length > 0)
                _buildMetadata = new List<string>(version._buildMetadata);
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified builder's semantic version.</para>
        /// </summary>
        /// <param name="builder">The semantic version builder to initialize the <see cref="SemanticVersionBuilder"/> with.</param>
        public SemanticVersionBuilder(SemanticVersionBuilder builder)
        {
            _major = builder._major;
            _minor = builder._minor;
            _patch = builder._patch;
            _preReleases = builder._preReleases?.ToList();
            _buildMetadata = builder._buildMetadata?.ToList();
        }

        /// <summary>
        ///   <para>Constructs an instance of the <see cref="SemanticVersion"/> class using this <see cref="SemanticVersionBuilder"/>.</para>
        /// </summary>
        /// <returns>The semantic version constructed by this <see cref="SemanticVersionBuilder"/>.</returns>
        [Pure] public SemanticVersion ToVersion()
            => new SemanticVersion(_major, _minor, _patch, _preReleases?.ToArray(), _buildMetadata?.ToArray());

        /// <summary>
        ///   <para>Sets the major version component of this instance to the specified value.</para>
        /// </summary>
        /// <param name="major">The major version component to set.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/> is less than 0.</exception>
        public SemanticVersionBuilder WithMajor(int major)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), major, Exceptions.MajorNegative);
            _major = major;
            return this;
        }
        /// <summary>
        ///   <para>Sets the minor version component of this instance to the specified value.</para>
        /// </summary>
        /// <param name="minor">The minor version component to set.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minor"/> is less than 0.</exception>
        public SemanticVersionBuilder WithMinor(int minor)
        {
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), minor, Exceptions.MinorNegative);
            _minor = minor;
            return this;
        }
        /// <summary>
        ///   <para>Sets the patch version component of this instance to the specified value.</para>
        /// </summary>
        /// <param name="patch">The patch version component to set.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="patch"/> is less than 0.</exception>
        public SemanticVersionBuilder WithPatch(int patch)
        {
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), patch, Exceptions.PatchNegative);
            _patch = patch;
            return this;
        }

        /// <summary>
        ///   <para>Removes all pre-release identifiers.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        public SemanticVersionBuilder ClearPreReleases()
        {
            _preReleases?.Clear();
            return this;
        }
        /// <summary>
        ///   <para>Removes all build metadata identifiers.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        public SemanticVersionBuilder ClearBuildMetadata()
        {
            _buildMetadata?.Clear();
            return this;
        }

        /// <summary>
        ///   <para>Appends the specified pre-release identifier to this instance.</para>
        /// </summary>
        /// <param name="identifier">The string representation of a pre-release identifier to append.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        public SemanticVersionBuilder AppendPreRelease(string identifier)
        {
            SemanticPreRelease preRelease = SemanticPreRelease.Parse(identifier);
            (_preReleases ??= new List<SemanticPreRelease>()).Add(preRelease);
            return this;
        }
        /// <summary>
        ///   <para>Appends the specified pre-release identifier to this instance.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to append.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        public SemanticVersionBuilder AppendPreRelease(SemanticPreRelease preRelease)
        {
            (_preReleases ??= new List<SemanticPreRelease>()).Add(preRelease);
            return this;
        }
        /// <summary>
        ///   <para>Appends the specified pre-release identifiers to this instance.</para>
        /// </summary>
        /// <param name="identifiers">A collection of string representations of pre-release identifiers to append.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="identifiers"/> contains an invalid string representation of a pre-release identifier.</exception>
        public SemanticVersionBuilder AppendPreReleases([InstantHandle] IEnumerable<string> identifiers)
        {
            (_preReleases ??= new List<SemanticPreRelease>()).AddRange(identifiers.Select(SemanticPreRelease.Parse));
            return this;
        }
        /// <summary>
        ///   <para>Appends the specified pre-release identifiers to this instance.</para>
        /// </summary>
        /// <param name="preReleases">A collection of pre-release identifiers to append.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        public SemanticVersionBuilder AppendPreReleases([InstantHandle] IEnumerable<SemanticPreRelease> preReleases)
        {
            (_preReleases ??= new List<SemanticPreRelease>()).AddRange(preReleases);
            return this;
        }

        /// <summary>
        ///   <para>Appends the specified build metadata identifier to this instance.</para>
        /// </summary>
        /// <param name="identifier">The build metadata identifier to append.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> is not valid build metadata identifier.</exception>
        public SemanticVersionBuilder AppendBuildMetadata(string identifier)
        {
            if (identifier.Length is 0)
                throw new ArgumentException(Exceptions.BuildMetadataEmpty, nameof(identifier));
            if (!Util.ContainsOnlyValidCharacters(identifier))
                throw new ArgumentException(Exceptions.BuildMetadataInvalid, nameof(identifier));
            (_buildMetadata ??= new List<string>()).Add(identifier);
            return this;
        }
        /// <summary>
        ///   <para>Appends the specified build metadata identifiers to this instance.</para>
        /// </summary>
        /// <param name="identifiers">A collection of build metadata identifiers to append.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="identifiers"/> contains an invalid build metadata identifier.</exception>
        public SemanticVersionBuilder AppendBuildMetadata([InstantHandle] IEnumerable<string> identifiers)
        {
            foreach (string identifier in identifiers)
            {
                if (identifier.Length is 0)
                    throw new ArgumentException(Exceptions.BuildMetadataEmpty, nameof(identifiers));
                if (!Util.ContainsOnlyValidCharacters(identifier))
                    throw new ArgumentException(Exceptions.BuildMetadataInvalid, nameof(identifiers));
                (_buildMetadata ??= new List<string>()).Add(identifier);
            }
            return this;
        }

        /// <summary>
        ///   <para>Increments the major version of this instance.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Major"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementMajor()
        {
            if (_minor != 0 || _patch != 0 || _preReleases is null || _preReleases.Count is 0)
            {
                // 1.2.3 → 2.0.0
                int newMajor = _major + 1;
                if (newMajor < 0) throw new InvalidOperationException(Exceptions.MajorTooBig);
                _major = newMajor;
            }
            // BUT: 1.0.0-alpha → 1.0.0
            _minor = 0;
            _patch = 0;
            _preReleases?.Clear();
            return this;
        }
        /// <summary>
        ///   <para>Increments the minor version of this instance.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Minor"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementMinor()
        {
            if (_patch != 0 || _preReleases is null || _preReleases.Count is 0)
            {
                // 1.2.3 → 1.3.0
                int newMinor = _minor + 1;
                if (newMinor < 0) throw new InvalidOperationException(Exceptions.MinorTooBig);
                _minor = newMinor;
            }
            // BUT: 1.2.0-alpha → 1.2.0
            _patch = 0;
            _preReleases?.Clear();
            return this;
        }
        /// <summary>
        ///   <para>Increments the patch version of this instance.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPatch()
        {
            if (_preReleases is null || _preReleases.Count is 0)
            {
                // 1.2.3 → 1.2.4
                int newPatch = _patch + 1;
                if (newPatch < 0) throw new InvalidOperationException(Exceptions.PatchTooBig);
                _patch = newPatch;
            }
            // BUT: 1.2.3-alpha → 1.2.3
            _preReleases?.Clear();
            return this;
        }

        /// <summary>
        ///   <para>Increments the right-most numeric pre-release identifier, or appends <c>0</c> if there isn't one.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException">The numeric value of the right-most numeric pre-release identifier is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreRelease()
            => IncrementPreRelease(SemanticPreRelease.Zero);
        /// <summary>
        ///   <para>If the left-most pre-release identifier is not the same as the one specified (and the specified one is not <c>0</c>), replaces the pre-release identifiers with <c><paramref name="identifier"/>.0</c>; otherwise, increments the right-most numeric pre-release identifier, or appends <c>0</c> if there isn't one.</para>
        /// </summary>
        /// <param name="identifier">The string representation of a pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> or the numeric value of the right-most numeric pre-release identifier is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreRelease(string? identifier)
            => IncrementPreRelease(identifier is null ? SemanticPreRelease.Zero : SemanticPreRelease.Parse(identifier));
        /// <summary>
        ///   <para>If the left-most pre-release identifier is not the same as the one specified (and the specified one is not <c>0</c>), replaces the pre-release identifiers with <c><paramref name="preRelease"/>.0</c>; otherwise, increments the right-most numeric pre-release identifier, or appends <c>0</c> if there isn't one.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> or the numeric value of the right-most numeric pre-release identifier is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreRelease(SemanticPreRelease preRelease)
        {
            if (_preReleases is null || _preReleases.Count is 0)
            {
                // 1.2.3 → 1.2.4-0 or 1.2.4-alpha.0
                int newPatch = _patch + 1;
                if (newPatch < 0) throw new InvalidOperationException(Exceptions.PatchTooBig);
                _patch = newPatch;
                _preReleases ??= new List<SemanticPreRelease>();
                _preReleases.Add(preRelease);
                if (preRelease != SemanticPreRelease.Zero)
                    _preReleases.Add(SemanticPreRelease.Zero);
            }
            else if (preRelease == SemanticPreRelease.Zero || _preReleases[0].Equals(preRelease))
            {
                // increment the right-most numeric pre-release identifier:
                // 1.2.3-4 → 1.2.3-5
                // 1.2.3-4.alpha.6.beta → 1.2.3-4.alpha.7.beta
                for (int i = _preReleases.Count - 1; i >= 0; i--)
                    if (_preReleases[i].IsNumeric)
                    {
                        int newNumber = _preReleases[i].Number + 1;
                        if (newNumber < 0) throw new InvalidOperationException(Exceptions.PreReleaseTooBig);
                        _preReleases[i] = new SemanticPreRelease(newNumber);
                        return this;
                    }
                // if there are no numeric pre-release identifiers:
                // 1.2.3-alpha → 1.2.3-alpha.0
                _preReleases.Add(SemanticPreRelease.Zero);
            }
            else
            {
                // if the specified pre-release identifier is different from the current one:
                // 1.2.3-alpha.5 (beta) → 1.2.3-beta.0
                _preReleases.Clear();
                _preReleases.Add(preRelease);
                _preReleases.Add(SemanticPreRelease.Zero);
            }
            return this;
        }

        /// <summary>
        ///   <para>Increments the major version of this instance, adding a <c>0</c> pre-release identifier.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Major"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreMajor()
            => IncrementPreMajor(SemanticPreRelease.Zero);
        /// <summary>
        ///   <para>Increments the major version of this instance, adding <c><paramref name="identifier"/>.0</c> pre-release identifiers, or just <c>0</c> if <paramref name="identifier"/> is <c>0</c>.</para>
        /// </summary>
        /// <param name="identifier">The string representation of a pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Major"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreMajor(string? identifier)
            => IncrementPreMajor(identifier is null ? SemanticPreRelease.Zero : SemanticPreRelease.Parse(identifier));
        /// <summary>
        ///   <para>Increments the major version of this instance, adding <c><paramref name="preRelease"/>.0</c> pre-release identifiers, or just <c>0</c> if <paramref name="preRelease"/> is <c>0</c>.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Major"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreMajor(SemanticPreRelease preRelease)
        {
            // 1.2.3-alpha → 2.0.0-0 or 2.0.0-beta.0
            int newMajor = _major + 1;
            if (newMajor < 0) throw new InvalidOperationException(Exceptions.MajorTooBig);
            _major = newMajor;
            _minor = 0;
            _patch = 0;
            if (_preReleases is not null) _preReleases.Clear();
            else _preReleases = new List<SemanticPreRelease>();
            _preReleases.Add(preRelease);
            if (preRelease != SemanticPreRelease.Zero)
                _preReleases.Add(SemanticPreRelease.Zero);
            return this;
        }

        /// <summary>
        ///   <para>Increments the minor version of this instance, adding a <c>0</c> pre-release identifier.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Minor"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreMinor()
            => IncrementPreMinor(SemanticPreRelease.Zero);
        /// <summary>
        ///   <para>Increments the minor version of this instance, adding <c><paramref name="identifier"/>.0</c> pre-release identifiers, or just <c>0</c> if <paramref name="identifier"/> is <c>0</c>.</para>
        /// </summary>
        /// <param name="identifier">The string representation of a pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Minor"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreMinor(string? identifier)
            => IncrementPreMinor(identifier is null ? SemanticPreRelease.Zero : SemanticPreRelease.Parse(identifier));
        /// <summary>
        ///   <para>Increments the minor version of this instance, adding <c><paramref name="preRelease"/>.0</c> pre-release identifiers, or just <c>0</c> if <paramref name="preRelease"/> is <c>0</c>.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Minor"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPreMinor(SemanticPreRelease preRelease)
        {
            // 1.2.3-alpha → 1.3.0-0 or 1.3.0-beta.0
            int newMinor = _minor + 1;
            if (newMinor < 0) throw new InvalidOperationException(Exceptions.MinorTooBig);
            _minor = newMinor;
            _patch = 0;
            if (_preReleases is not null) _preReleases.Clear();
            else _preReleases = new List<SemanticPreRelease>();
            _preReleases.Add(preRelease);
            if (preRelease != SemanticPreRelease.Zero)
                _preReleases.Add(SemanticPreRelease.Zero);
            return this;
        }

        /// <summary>
        ///   <para>Increments the patch version of this instance, adding a <c>0</c> pre-release identifier.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPrePatch()
            => IncrementPrePatch(SemanticPreRelease.Zero);
        /// <summary>
        ///   <para>Increments the patch version of this instance, adding <c><paramref name="identifier"/>.0</c> pre-release identifiers, or just <c>0</c> if <paramref name="identifier"/> is <c>0</c>.</para>
        /// </summary>
        /// <param name="identifier">The string representation of a pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> does not represent a valid pre-release identifier.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPrePatch(string? identifier)
            => IncrementPrePatch(identifier is null ? SemanticPreRelease.Zero : SemanticPreRelease.Parse(identifier));
        /// <summary>
        ///   <para>Increments the patch version of this instance, adding <c><paramref name="preRelease"/>.0</c> pre-release identifiers, or just <c>0</c> if <paramref name="preRelease"/> is <c>0</c>.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> is equal to <see cref="int.MaxValue"/> and cannot be incremented.</exception>
        public SemanticVersionBuilder IncrementPrePatch(SemanticPreRelease preRelease)
        {
            // 1.2.3-alpha → 1.2.4-0 or 1.2.4-beta.0
            int newPatch = _patch + 1;
            if (newPatch < 0) throw new InvalidOperationException(Exceptions.PatchTooBig);
            _patch = newPatch;
            if (_preReleases is not null) _preReleases.Clear();
            else _preReleases = new List<SemanticPreRelease>();
            _preReleases.Add(preRelease);
            if (preRelease != SemanticPreRelease.Zero)
                _preReleases.Add(SemanticPreRelease.Zero);
            return this;
        }

        /// <summary>
        ///   <para>Increments the version of this instance with the specified <paramref name="increment"/> type.</para>
        /// </summary>
        /// <param name="increment">The semantic version increment type.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        public SemanticVersionBuilder Increment(IncrementType increment)
            => Increment(increment, SemanticPreRelease.Zero);
        /// <summary>
        ///   <para>Increments the version of this instance with the specified <paramref name="increment"/> type and <paramref name="identifier"/>.</para>
        /// </summary>
        /// <param name="increment">The semantic version increment type.</param>
        /// <param name="identifier">The string representation of a pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        public SemanticVersionBuilder Increment(IncrementType increment, string? identifier)
            => Increment(increment, identifier is null ? SemanticPreRelease.Zero : SemanticPreRelease.Parse(identifier));
        /// <summary>
        ///   <para>Increments the version of this instance with the specified <paramref name="increment"/> type and <paramref name="preRelease"/>.</para>
        /// </summary>
        /// <param name="increment">The semantic version increment type.</param>
        /// <param name="preRelease">The pre-release identifier to use.</param>
        /// <returns>A reference to this instance after the operation.</returns>
        public SemanticVersionBuilder Increment(IncrementType increment, SemanticPreRelease preRelease) => increment switch
        {
            IncrementType.None => this,
            IncrementType.Major => IncrementMajor(),
            IncrementType.Minor => IncrementMinor(),
            IncrementType.Patch => IncrementPatch(),
            IncrementType.PreMajor => IncrementPreMajor(preRelease),
            IncrementType.PreMinor => IncrementPreMinor(preRelease),
            IncrementType.PrePatch => IncrementPrePatch(preRelease),
            IncrementType.PreRelease => IncrementPreRelease(preRelease),
            _ => throw new ArgumentException($"Invalid {nameof(IncrementType)} value.", nameof(increment)),
        };

    }
}