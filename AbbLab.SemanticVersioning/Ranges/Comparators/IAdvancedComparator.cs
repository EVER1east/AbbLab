namespace AbbLab.SemanticVersioning
{
    public interface IAdvancedComparator
    {
        (PrimitiveComparator, PrimitiveComparator?) ToPrimitives();
    }
}
