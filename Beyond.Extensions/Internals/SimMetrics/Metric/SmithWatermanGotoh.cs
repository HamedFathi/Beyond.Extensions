﻿using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class SmithWatermanGotoh : SmithWatermanGotohWindowedAffine
{
    private const int AffineGapWindowSize = 0x7fffffff;
    private const double EstimatedTimingConstant = 2.2000000171829015E-05;

    public SmithWatermanGotoh() : base(new AffineGapRange5To0Multiplier1(), new SubCostRange5ToMinus3(), 0x7fffffff)
    {
    }

    public SmithWatermanGotoh(AbstractAffineGapCost gapCostFunction) : base(gapCostFunction, new SubCostRange5ToMinus3(), 0x7fffffff)
    {
    }

    public SmithWatermanGotoh(AbstractSubstitutionCost costFunction) : base(new AffineGapRange5To0Multiplier1(), costFunction, 0x7fffffff)
    {
    }

    public SmithWatermanGotoh(AbstractAffineGapCost gapCostFunction, AbstractSubstitutionCost costFunction) : base(gapCostFunction, costFunction, 0x7fffffff)
    {
    }

    public override string LongDescriptionString => "Implements the Smith-Waterman-Gotoh algorithm providing a similarity measure between two string";

    public override string ShortDescriptionString => "SmithWatermanGotoh";

    public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
    {
        if (firstWord != null && secondWord != null)
        {
            double length = firstWord.Length;
            double num2 = secondWord.Length;
            return (length * num2 * length + length * num2 * num2) * 2.2000000171829015E-05;
        }
        return 0.0;
    }
}