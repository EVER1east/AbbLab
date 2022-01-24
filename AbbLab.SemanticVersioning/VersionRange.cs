using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AbbLab.Extensions;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public class VersionRange
    {

    }
    public class ComparatorSet
    {

    }
    public abstract class Comparator
    {
        public abstract bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases);
        public abstract (PrimitiveComparator, PrimitiveComparator?) ToPrimitives();

        public static EqualToComparator EqualTo(SemanticVersion version)
            => new EqualToComparator(version);
        public static GreaterThanComparator GreaterThan(SemanticVersion version)
            => new GreaterThanComparator(version);
        public static LessThanComparator LessThan(SemanticVersion version)
            => new LessThanComparator(version);
        public static GreaterThanOrEqualToComparator GreaterThanOrEqualTo(SemanticVersion version)
            => new GreaterThanOrEqualToComparator(version);
        public static LessThanOrEqualToComparator LessThanOrEqualTo(SemanticVersion version)
            => new LessThanOrEqualToComparator(version);



    }
    public abstract class PrimitiveComparator : Comparator
    {
        public SemanticVersion Comparand { get; }

        protected PrimitiveComparator(SemanticVersion comparand) => Comparand = comparand;

        protected abstract bool BaseCondition(SemanticVersion version);
        public override bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases)
        {
            // if the version doesn't have pre-releases, compare normally:
            // 1.2.3: >1.2.2, >1.2.3-alpha, >1.0.0-alpha, >1.0.0
            if (includePreReleases || version._preReleases.Length is 0)
                return BaseCondition(version);

            // comparable only to versions with the same component tuple and at least one pre-release:
            // 1.2.3-beta: >1.2.3-alpha,
            // But NOT: >1.2.2, >1.0.0, >1.2.2-alpha
            return version.Major == Comparand.Major && version.Minor == Comparand.Minor && version.Patch == Comparand.Patch
                   && Comparand._preReleases.Length > 0 && BaseCondition(version);
        }
        public override (PrimitiveComparator, PrimitiveComparator?) ToPrimitives() => (this, null);

    }
    public class EqualToComparator : PrimitiveComparator
    {
        public EqualToComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.Equals(Comparand);
    }
    public class GreaterThanComparator : PrimitiveComparator
    {
        public GreaterThanComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.CompareTo(Comparand) > 0;
    }
    public class LessThanComparator : PrimitiveComparator
    {
        public LessThanComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.CompareTo(Comparand) < 0;
    }
    public class GreaterThanOrEqualToComparator : PrimitiveComparator
    {
        public GreaterThanOrEqualToComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.CompareTo(Comparand) >= 0;
    }
    public class LessThanOrEqualToComparator : PrimitiveComparator
    {
        public LessThanOrEqualToComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.CompareTo(Comparand) <= 0;
    }
    public class HyphenComparator : Comparator
    {
        public PartialVersion Begin { get; }
        public PartialVersion End { get; }
        private PrimitiveComparator? primitiveBegin;
        private PrimitiveComparator? primitiveEnd;

        public HyphenComparator(PartialVersion begin, PartialVersion end)
        {
            Begin = begin;
            End = end;
        }

        private static PrimitiveComparator ConvertBeginVersion(PartialVersion partial)
        {
            // 1.2.3 - … → >=1.2.3
            // 1.2.x - … → >=1.2.0
            // 1.x.x - … → >=1.0.0
            // x.x.x - … → >=0.0.0
            // wildcards are equivalent to omitted components

            // SemanticVersion(PartialVersion) replaces wildcards with zeroes
            return new GreaterThanOrEqualToComparator(new SemanticVersion(partial));
        }
        private static PrimitiveComparator ConvertEndVersion(PartialVersion partial)
        {
            if (!partial.Major.IsNumeric)
            {
                // … - x.x.x → <2147483647.2147483647.2147483647-0
                return new LessThanComparator(new SemanticVersion(int.MaxValue, int.MaxValue, int.MaxValue,
                                                                  SemanticPreRelease.ZeroArray, null));
            }
            if (!partial.Minor.IsNumeric)
            {
                // … - 1.x.x → <2.0.0-0
                return new LessThanComparator(new SemanticVersion(partial.Major.Value + 1, 0, 0,
                                                                  SemanticPreRelease.ZeroArray, null));
            }
            if (!partial.Patch.IsNumeric)
            {
                // … - 1.2.x → <1.3.0-0
                return new LessThanComparator(new SemanticVersion(partial.Major.Value, partial.Minor.Value + 1, 0,
                                                                  SemanticPreRelease.ZeroArray, null));
            }
            // … - 1.2.3 → <=1.2.3
            // … - 1.2.3-beta → <=1.2.3-beta
            return new LessThanOrEqualToComparator((SemanticVersion)partial);
        }

        public override (PrimitiveComparator, PrimitiveComparator?) ToPrimitives()
        {
            primitiveBegin ??= ConvertBeginVersion(Begin);
            primitiveEnd ??= ConvertEndVersion(End);
            return (primitiveBegin, primitiveEnd);
        }

        public override bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases)
        {
            (PrimitiveComparator begin, PrimitiveComparator? end) = ToPrimitives();
            return begin.IsSatisfiedBy(version, includePreReleases) && end!.IsSatisfiedBy(version, includePreReleases);
        }

    }
}