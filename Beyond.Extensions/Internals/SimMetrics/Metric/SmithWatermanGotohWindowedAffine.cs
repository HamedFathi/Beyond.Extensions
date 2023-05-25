using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal class SmithWatermanGotohWindowedAffine : AbstractStringMetric
{
    private const double defaultMismatchScore = 0.0;
    private const double defaultPerfectScore = 1.0;
    private const int defaultWindowSize = 100;
    private AbstractSubstitutionCost dCostFunction;
    private double estimatedTimingConstant;
    private AbstractAffineGapCost gGapFunction;
    private int windowSize;

    public SmithWatermanGotohWindowedAffine() : this(new AffineGapRange5To0Multiplier1(), new SubCostRange5ToMinus3(), 100)
    {
    }

    public SmithWatermanGotohWindowedAffine(AbstractAffineGapCost gapCostFunction) : this(gapCostFunction, new SubCostRange5ToMinus3(), 100)
    {
    }

    public SmithWatermanGotohWindowedAffine(AbstractSubstitutionCost costFunction) : this(new AffineGapRange5To0Multiplier1(), costFunction, 100)
    {
    }

    public SmithWatermanGotohWindowedAffine(int affineGapWindowSize) : this(new AffineGapRange5To0Multiplier1(), new SubCostRange5ToMinus3(), affineGapWindowSize)
    {
    }

    public SmithWatermanGotohWindowedAffine(AbstractAffineGapCost gapCostFunction, AbstractSubstitutionCost costFunction) : this(gapCostFunction, costFunction, 100)
    {
    }

    public SmithWatermanGotohWindowedAffine(AbstractAffineGapCost gapCostFunction, int affineGapWindowSize) : this(gapCostFunction, new SubCostRange5ToMinus3(), affineGapWindowSize)
    {
    }

    public SmithWatermanGotohWindowedAffine(AbstractSubstitutionCost costFunction, int affineGapWindowSize) : this(new AffineGapRange5To0Multiplier1(), costFunction, affineGapWindowSize)
    {
    }

    public SmithWatermanGotohWindowedAffine(AbstractAffineGapCost gapCostFunction, AbstractSubstitutionCost costFunction, int affineGapWindowSize)
    {
        estimatedTimingConstant = 4.5000000682193786E-05;
        gGapFunction = gapCostFunction;
        dCostFunction = costFunction;
        windowSize = affineGapWindowSize;
    }

    public AbstractSubstitutionCost DCostFunction
    {
        get => dCostFunction;
        set => dCostFunction = value;
    }

    public AbstractAffineGapCost GGapFunction
    {
        get => gGapFunction;
        set => gGapFunction = value;
    }

    public override string LongDescriptionString => "Implements the Smith-Waterman-Gotoh algorithm with a windowed affine gap providing a similarity measure between two string";

    public override string ShortDescriptionString => "SmithWatermanGotohWindowedAffine";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if ((firstWord == null) || (secondWord == null))
        {
            return 0.0;
        }
        var unnormalisedSimilarity = GetUnnormalisedSimilarity(firstWord, secondWord);
        double num2 = Math.Min(firstWord.Length, secondWord.Length);
        if (dCostFunction.MaxCost > -gGapFunction.MaxCost)
        {
            num2 *= dCostFunction.MaxCost;
        }
        else
        {
            num2 *= -gGapFunction.MaxCost;
        }
        if (num2 == 0.0)
        {
            return 1.0;
        }
        return (unnormalisedSimilarity / num2);
    }

    public override string GetSimilarityExplained(string firstWord, string secondWord)
    {
        throw new NotImplementedException();
    }

    public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            double length = firstWord.Length;
            double num2 = secondWord.Length;
            return ((((length * num2) * windowSize) + ((length * num2) * windowSize)) * estimatedTimingConstant);
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        if ((firstWord == null) || (secondWord == null))
        {
            return 0.0;
        }
        var length = firstWord.Length;
        var num2 = secondWord.Length;
        if (length == 0)
        {
            return num2;
        }
        if (num2 == 0)
        {
            return length;
        }
        double[][] numArray = new double[length][];
        for (var i = 0; i < length; i++)
        {
            numArray[i] = new double[num2];
        }
        var num4 = 0.0;
        for (var j = 0; j < length; j++)
        {
            var num6 = dCostFunction.GetCost(firstWord, j, secondWord, 0);
            if (j == 0)
            {
                numArray[0][0] = Math.Max(0.0, num6);
            }
            else
            {
                var num7 = 0.0;
                var num8 = j - windowSize;
                if (num8 < 1)
                {
                    num8 = 1;
                }
                for (var n = num8; n < j; n++)
                {
                    num7 = Math.Max(num7, numArray[j - n][0] - gGapFunction.GetCost(firstWord, j - n, j));
                }
                numArray[j][0] = MathFunctions.MaxOf3(0.0, num7, num6);
            }
            if (numArray[j][0] > num4)
            {
                num4 = numArray[j][0];
            }
        }
        for (var k = 0; k < num2; k++)
        {
            var num11 = dCostFunction.GetCost(firstWord, 0, secondWord, k);
            if (k == 0)
            {
                numArray[0][0] = Math.Max(0.0, num11);
            }
            else
            {
                var num12 = 0.0;
                var num13 = k - windowSize;
                if (num13 < 1)
                {
                    num13 = 1;
                }
                for (var num14 = num13; num14 < k; num14++)
                {
                    num12 = Math.Max(num12, numArray[0][k - num14] - gGapFunction.GetCost(secondWord, k - num14, k));
                }
                numArray[0][k] = MathFunctions.MaxOf3(0.0, num12, num11);
            }
            if (numArray[0][k] > num4)
            {
                num4 = numArray[0][k];
            }
        }
        for (var m = 1; m < length; m++)
        {
            for (var num16 = 1; num16 < num2; num16++)
            {
                var num17 = dCostFunction.GetCost(firstWord, m, secondWord, num16);
                var num18 = 0.0;
                var num19 = 0.0;
                var num20 = m - windowSize;
                if (num20 < 1)
                {
                    num20 = 1;
                }
                for (var num21 = num20; num21 < m; num21++)
                {
                    num18 = Math.Max(num18, numArray[m - num21][num16] - gGapFunction.GetCost(firstWord, m - num21, m));
                }
                num20 = num16 - windowSize;
                if (num20 < 1)
                {
                    num20 = 1;
                }
                for (var num22 = num20; num22 < num16; num22++)
                {
                    num19 = Math.Max(num19, numArray[m][num16 - num22] - gGapFunction.GetCost(secondWord, num16 - num22, num16));
                }
                numArray[m][num16] = MathFunctions.MaxOf4(0.0, num18, num19, numArray[m - 1][num16 - 1] + num17);
                if (numArray[m][num16] > num4)
                {
                    num4 = numArray[m][num16];
                }
            }
        }
        return num4;
    }
}