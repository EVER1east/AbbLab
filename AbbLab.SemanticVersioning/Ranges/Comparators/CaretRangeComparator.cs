namespace AbbLab.SemanticVersioning
{
    public sealed class CaretRangeComparator : Comparator, IAdvancedComparator
    {
        public PartialVersion Version { get; }
        private PrimitiveComparator? primitiveBegin;
        private PrimitiveComparator? primitiveEnd;

        public CaretRangeComparator(PartialVersion version) => Version = version;

        public (PrimitiveComparator, PrimitiveComparator?) ToPrimitives()
        {
            if (primitiveBegin is null)
            {
                if (!Version.Major.IsNumeric)
                {
                    // ^x.x.x → >=0.0.0 <2147483648.0.0-0, or just >=0.0.0 (basically, any)
                    primitiveBegin = new GreaterThanOrEqualToComparator(
                        new SemanticVersion(0, 0, 0, (SemanticPreRelease[]?)null, null));
                    primitiveEnd = null;
                }
                else if (!Version.Minor.IsNumeric || Version.Major.Value > 0)
                {
                    // ^1 → >=1.0.0 <2.0.0-0
                    // ^1.x.x → >=1.0.0 <2.0.0-0
                    // ^0 → >=0.0.0 <1.0.0-0
                    // ^0.x.x → >=0.0.0 <1.0.0-0

                    // ^1.2 → >=1.2.0 <2.0.0-0
                    // ^1.2.x → >=1.2.0 <2.0.0-0
                    int major = Version.Major.Value;
                    primitiveBegin = new GreaterThanOrEqualToComparator(
                        new SemanticVersion(major, 0, 0, (SemanticPreRelease[]?)null, null));
                    primitiveEnd = new LessThanComparator(
                        new SemanticVersion(major + 1, 0, 0, SemanticPreRelease.ZeroArray, null));
                }
                else if (!Version.Patch.IsNumeric || Version.Minor.Value > 0)
                {
                    // ^0.0 → >=0.0.0 <0.1.0-0
                    // ^0.0.x → >=0.0.0 <0.1.0-0

                    // ^0.1 → >=0.1.0 <0.2.0-0
                    // ^0.1.x → >=0.1.0 <0.2.0-0
                    int minor = Version.Minor.Value;
                    primitiveBegin = new GreaterThanOrEqualToComparator(
                        new SemanticVersion(0, minor, 0, (SemanticPreRelease[]?)null, null));
                    primitiveEnd = new LessThanComparator(
                        new SemanticVersion(0, minor + 1, 0, SemanticPreRelease.ZeroArray, null));
                }
                else
                {
                    // ^0.0.0 → >=0.0.0 <0.0.1-0 (only 0.0.0)
                    // ^0.0.1 → >=0.0.1 <0.0.2-0 (only 0.0.1)
                    // ^0.0.1-alpha → >=0.0.1-alpha <0.0.2-0
                    int patch = Version.Patch.Value;
                    primitiveBegin = new GreaterThanOrEqualToComparator(
                        new SemanticVersion(0, 0, patch, Version._preReleases, null));
                    primitiveEnd = new LessThanComparator(
                        new SemanticVersion(0, 0, patch + 1, SemanticPreRelease.ZeroArray, null));
                }
            }
            return (primitiveBegin, primitiveEnd);
        }
        public override bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases)
        {
            (PrimitiveComparator begin, PrimitiveComparator? end) = ToPrimitives();
            return begin.IsSatisfiedBy(version, includePreReleases) && (end is null || end.IsSatisfiedBy(version, includePreReleases));
        }

    }
}
