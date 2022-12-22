using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal sealed class AffineGapRange5To0Multiplier1 : AbstractAffineGapCost
{
    private const int charExactMatchScore = 5;
    private const int charMismatchMatchScore = 0;

    public override double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap)
    {
        if (stringIndexStartGap >= stringIndexEndGap)
        {
            return 0.0;
        }
        return (double)(5 + ((stringIndexEndGap - 1) - stringIndexStartGap));
    }

    public override double MaxCost => 5.0;

    public override double MinCost => 0.0;

    public override string ShortDescriptionString => "AffineGapRange5To0Multiplier1";
}