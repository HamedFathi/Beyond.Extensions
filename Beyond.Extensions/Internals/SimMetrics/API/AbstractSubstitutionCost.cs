namespace Beyond.Extensions.Internals.SimMetrics.API;

internal abstract class AbstractSubstitutionCost : ISubstitutionCost
{
    protected AbstractSubstitutionCost()
    {
    }

    public abstract double MaxCost { get; }

    public abstract double MinCost { get; }

    public abstract string ShortDescriptionString { get; }

    public abstract double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex);
}