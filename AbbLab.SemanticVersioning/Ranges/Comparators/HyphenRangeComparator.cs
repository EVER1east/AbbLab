using JetBrains.Annotations;

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

        [Pure] public (PrimitiveComparator, PrimitiveComparator?) ToPrimitives()
        {
            if (primitiveBegin is null)
            {
                // wildcards are equivalent to omitted components:
                // x.x.x - … → >=0.0.0
                // 1.x.x - … → >=1.0.0
                // 1.2.x - … → >=1.2.0
                // 1.2.3 - … → >=1.2.3
                // 1.2.3-alpha - … → >=1.2.3-alpha

                // SemanticVersion(PartialVersion) replaces wildcards with zeroes
                primitiveBegin = new GreaterThanOrEqualToComparator(new SemanticVersion(Begin));

                if (!End.Major.IsNumeric)
                {
                    // … - x.x.x → <2147483648.0.0-0 (basically, any)
                    primitiveEnd = null;
                }
                else if (!End.Minor.IsNumeric)
                {
                    // … - 1.x.x → <2.0.0-0
                    primitiveEnd = new LessThanComparator(
                        new SemanticVersion(End.Major.Value + 1, 0, 0, SemanticPreRelease.ZeroArray, null));
                }
                else if (!End.Patch.IsNumeric)
                {
                    // … - 1.2.x → <1.3.0-0
                    primitiveEnd = new LessThanComparator(
                        new SemanticVersion(End.Major.Value, End.Minor.Value + 1, 0, SemanticPreRelease.ZeroArray, null));
                }
                else
                {
                    // … - 1.2.3 → <=1.2.3
                    // … - 1.2.3-beta → <=1.2.3-beta
                    primitiveEnd = new LessThanOrEqualToComparator((SemanticVersion)End);
                }
            }
            return (primitiveBegin, primitiveEnd);
        }
        [Pure] public override bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases)
        {
            (PrimitiveComparator begin, PrimitiveComparator? end) = ToPrimitives();
            return begin.IsSatisfiedBy(version, includePreReleases) && (end is null || end.IsSatisfiedBy(version, includePreReleases));
        }

    }
}
