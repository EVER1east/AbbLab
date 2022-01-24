using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public sealed class TildeRangeComparator : Comparator, IAdvancedComparator
    {
        public PartialVersion Version { get; }
        private PrimitiveComparator? primitiveBegin;
        private PrimitiveComparator? primitiveEnd;

        public TildeRangeComparator(PartialVersion version) => Version = version;

        [Pure] public (PrimitiveComparator, PrimitiveComparator?) ToPrimitives()
        {
            if (primitiveBegin is null)
            {
                if (!Version.Major.IsNumeric)
                {
                    // ~x.x.x → >=0.0.0 <2147483648.0.0-0, or just >=0.0.0 (basically, any)
                    primitiveBegin = new GreaterThanOrEqualToComparator(
                        new SemanticVersion(0, 0, 0, (SemanticPreRelease[]?)null, null));
                    primitiveEnd = null;
                }
                else if (!Version.Minor.IsNumeric)
                {
                    // allows minor-level changes
                    // ~1.x.x → >=1.0.0 <2.0.0-0
                    int major = Version.Major.Value;
                    primitiveBegin = new GreaterThanOrEqualToComparator(
                        new SemanticVersion(major, 0, 0, (SemanticPreRelease[]?)null, null));
                    primitiveEnd = new LessThanComparator(
                        new SemanticVersion(major + 1, 0, 0, SemanticPreRelease.ZeroArray, null));
                }
                else
                {
                    // allows patch-level changes
                    // ~1.2.x → >=1.2.0 <1.3.0-0
                    // ~1.2.3 → >=1.2.3 <1.3.0-0
                    // ~1.2.3-alpha → >=1.2.3-alpha <1.3.0-0
                    int major = Version.Major.Value;
                    int minor = Version.Minor.Value;
                    primitiveBegin = new GreaterThanOrEqualToComparator(
                        new SemanticVersion(major, minor, Version.Patch.GetValueOrZero(), Version._preReleases, null));
                    primitiveEnd = new LessThanComparator(
                        new SemanticVersion(major, minor + 1, 0, SemanticPreRelease.ZeroArray, null));
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
