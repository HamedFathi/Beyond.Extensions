namespace Beyond.Extensions.Internals.SimMetrics.API;

internal interface ISubstitutionCost
{
    double MaxCost { get; }

    double MinCost { get; }

    string ShortDescriptionString { get; }

    double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex);
}