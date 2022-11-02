namespace Beyond.Extensions.Types;

public class PartitionedSequence<TSource>
{
    public PartitionedSequence(IList<TSource> matches, IList<TSource> mismatches)
    {
        Matches = matches;
        Mismatches = mismatches;
    }

    public IList<TSource> Matches { get; }
    public IList<TSource> Mismatches { get; }
}