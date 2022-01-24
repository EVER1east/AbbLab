using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public abstract class Comparator
    {
        [Pure] public abstract bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases);

        [Pure] public static EqualToComparator EqualTo(SemanticVersion version)
            => new EqualToComparator(version);
        [Pure] public static GreaterThanComparator GreaterThan(SemanticVersion version)
            => new GreaterThanComparator(version);
        [Pure] public static LessThanComparator LessThan(SemanticVersion version)
            => new LessThanComparator(version);
        [Pure] public static GreaterThanOrEqualToComparator GreaterThanOrEqualTo(SemanticVersion version)
            => new GreaterThanOrEqualToComparator(version);
        [Pure] public static LessThanOrEqualToComparator LessThanOrEqualTo(SemanticVersion version)
            => new LessThanOrEqualToComparator(version);

        [Pure] public static HyphenRangeComparator HyphenRange(PartialVersion begin, PartialVersion end)
            => new HyphenRangeComparator(begin, end);
        [Pure] public static XRangeComparator XRange(PartialVersion version)
            => new XRangeComparator(version);
        [Pure] public static TildeRangeComparator TildeRange(PartialVersion version)
            => new TildeRangeComparator(version);

    }
}
