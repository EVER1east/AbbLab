using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public interface IAdvancedComparator
    {
        [Pure] (PrimitiveComparator, PrimitiveComparator?) ToPrimitives();
    }
}
