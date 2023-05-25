using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal sealed class SubCostRange1ToMinus2 : AbstractSubstitutionCost
{
    private const int CharExactMatchScore = 1;
    private const int CharMismatchMatchScore = -2;

    public override double MaxCost => 1.0;

    public override double MinCost => -2.0;

    public override string ShortDescriptionString => "SubCostRange1ToMinus2";

    public override double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex)
    {
        if (firstWord == null || secondWord == null)
        {
            return -2.0;
        }
        if (firstWord.Length <= firstWordIndex || firstWordIndex < 0)
        {
            return -2.0;
        }
        if (secondWord.Length <= secondWordIndex || secondWordIndex < 0)
        {
            return -2.0;
        }
        return firstWord[firstWordIndex] != secondWord[secondWordIndex] ? -2 : (double)1;
    }
}