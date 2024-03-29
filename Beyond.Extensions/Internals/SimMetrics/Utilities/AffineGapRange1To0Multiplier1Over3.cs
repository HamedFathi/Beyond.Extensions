﻿using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal sealed class AffineGapRange1To0Multiplier1Over3 : AbstractAffineGapCost
{
    private const int CharExactMatchScore = 1;
    private const int CharMismatchMatchScore = 0;

    public override double MaxCost => 1.0;

    public override double MinCost => 0.0;

    public override string ShortDescriptionString => "AffineGapRange1To0Multiplier1Over3";

    public override double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap)
    {
        if (stringIndexStartGap >= stringIndexEndGap)
        {
            return 0.0;
        }
        return 1f + (stringIndexEndGap - 1 - stringIndexStartGap) * 0.3333333f;
    }
}