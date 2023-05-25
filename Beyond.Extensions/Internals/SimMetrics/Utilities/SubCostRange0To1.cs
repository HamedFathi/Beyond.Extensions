using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal sealed class SubCostRange0To1 : AbstractSubstitutionCost
{
    private const int charExactMatchScore = 1;
    private const int charMismatchMatchScore = 0;

    public override double MaxCost => 1.0;

    public override double MinCost => 0.0;

    public override string ShortDescriptionString => "SubCostRange0To1";

    public override double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            return ((firstWord[firstWordIndex] != secondWord[secondWordIndex]) ? 1 : ((double)0));
        }
        return 0.0;
    }
}