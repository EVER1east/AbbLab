namespace AbbLab.SemanticVersioning
{
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

    }
    public sealed class EqualToComparator : PrimitiveComparator
    {
        public EqualToComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.Equals(Comparand);
    }
    public sealed class GreaterThanComparator : PrimitiveComparator
    {
        public GreaterThanComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.CompareTo(Comparand) > 0;
    }
    public sealed class LessThanComparator : PrimitiveComparator
    {
        public LessThanComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.CompareTo(Comparand) < 0;
    }
    public sealed class GreaterThanOrEqualToComparator : PrimitiveComparator
    {
        public GreaterThanOrEqualToComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.CompareTo(Comparand) >= 0;
    }
    public sealed class LessThanOrEqualToComparator : PrimitiveComparator
    {
        public LessThanOrEqualToComparator(SemanticVersion comparand) : base(comparand) { }
        protected override bool BaseCondition(SemanticVersion version) => version.CompareTo(Comparand) <= 0;
    }
}
