using System;
using System.Collections.Generic;

namespace AbbLab.SemanticVersioning
{
    public partial class SemanticVersionBuilder
    {
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
            // NOTE:
            // The implementation of this method is kind of vague and doesn't cover all of the cases in node-semver,
            // and I just copied the implementation from there. so there may be some strange cases.

            if (_preReleases is null || _preReleases.Count is 0)
            {
                // if this is not a pre-release version, just increment patch and add a '0' pre-release:
                // 1.2.3 → 1.2.4-0 or 1.2.4-alpha.0
                int newPatch = _patch + 1;
                if (newPatch < 0) throw new InvalidOperationException(Exceptions.PatchTooBig);
                _patch = newPatch;
                _preReleases ??= new List<SemanticPreRelease>();
                _preReleases.Add(preRelease);
                if (preRelease != SemanticPreRelease.Zero)
                    _preReleases.Add(SemanticPreRelease.Zero);
            }
            else if (preRelease == SemanticPreRelease.Zero)
            {
                // (default behaviour) simply increment the right-most numeric pre-release identifier:
                // 1.2.3-0 (0) → 1.2.3-1
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
            else if (_preReleases[0].Equals(preRelease))
            {
                // if the specified pre-release identifier is the same as the current one, then…

                // if there's a right-most numeric pre-release identifier (excluding the specified one), increment it:
                // 1.2.3-beta.3 (beta) → 1.2.3-beta.4
                // 1.2.3-beta.3.gamma (beta) → 1.2.3-beta.4.gamma
                // 1.2.3-2022.3 (2022) → 1.2.3-2022.4
                for (int i = _preReleases.Count - 1; i >= 1; i--)
                    if (_preReleases[i].IsNumeric)
                    {
                        int newNumber = _preReleases[i].Number + 1;
                        if (newNumber < 0) throw new InvalidOperationException(Exceptions.PreReleaseTooBig);
                        _preReleases[i] = new SemanticPreRelease(newNumber);
                        return this;
                    }
                // otherwise, ensure that there are only two identifiers - the specified one and a numeric one:
                // 1.2.3-beta (beta) → 1.2.3-beta.0
                // 1.2.3-beta.alpha (beta) → 1.2.3-beta.0
                // 1.2.3-2022 (2022) → 1.2.3-2022.0
                // 1.2.3-2022.alpha (2022) → 1.2.3-2022.0
                if (_preReleases.Count > 1)
                    _preReleases.RemoveRange(1, _preReleases.Count - 1);
                _preReleases.Add(SemanticPreRelease.Zero);
            }
            else
            {
                // if the specified pre-release identifier is different from the current one, change it:
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
