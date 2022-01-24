namespace AbbLab.SemanticVersioning
{
    public abstract class Comparator
    {
        public abstract bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases);

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

        public static HyphenRangeComparator HyphenRange(PartialVersion begin, PartialVersion end)
            => new HyphenRangeComparator(begin, end);
        public static XRangeComparator XRange(PartialVersion version)
            => new XRangeComparator(version);
        public static TildeRangeComparator TildeRange(PartialVersion version)
            => new TildeRangeComparator(version);

    }
    public interface IAdvancedComparator
    {
        (PrimitiveComparator, PrimitiveComparator?) ToPrimitives();
    }
}
