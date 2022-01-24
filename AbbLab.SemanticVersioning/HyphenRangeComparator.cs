using System;

namespace AbbLab.SemanticVersioning
{
    public sealed class HyphenRangeComparator : Comparator, IAdvancedComparator
    {
        public PartialVersion Begin { get; }
        public PartialVersion End { get; }
        private PrimitiveComparator? primitiveBegin;
        private PrimitiveComparator? primitiveEnd;

        public HyphenRangeComparator(PartialVersion begin, PartialVersion end)
        {
            Begin = begin;
            End = end;
        }

        private static PrimitiveComparator ConvertBeginVersion(PartialVersion partial)
        {
            // wildcards are equivalent to omitted components:
            // x.x.x - … → >=0.0.0
            // 1.x.x - … → >=1.0.0
            // 1.2.x - … → >=1.2.0
            // 1.2.3 - … → >=1.2.3
            // 1.2.3-alpha - … → >=1.2.3-alpha

            // SemanticVersion(PartialVersion) replaces wildcards with zeroes
            return new GreaterThanOrEqualToComparator(new SemanticVersion(partial));
        }
        private static PrimitiveComparator ConvertEndVersion(PartialVersion partial)
        {
            if (!partial.Major.IsNumeric)
            {
                // … - x.x.x → <2147483648.0.0-0 (basically, any)
                // return new AnyComparator()?
                throw new NotImplementedException();
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

        public (PrimitiveComparator, PrimitiveComparator) ToPrimitives()
        {
            primitiveBegin ??= ConvertBeginVersion(Begin);
            primitiveEnd ??= ConvertEndVersion(End);
            return (primitiveBegin, primitiveEnd);
        }
        public override bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases)
        {
            (PrimitiveComparator begin, PrimitiveComparator end) = ToPrimitives();
            return begin.IsSatisfiedBy(version, includePreReleases) && end.IsSatisfiedBy(version, includePreReleases);
        }

    }
}
