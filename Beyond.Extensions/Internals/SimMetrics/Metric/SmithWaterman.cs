using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class SmithWaterman : AbstractStringMetric
{
    private const double defaultGapCost = 0.5;
    private const double defaultMismatchScore = 0.0;
    private const double defaultPerfectMatchScore = 1.0;
    private const double estimatedTimingConstant = 0.0001610000035725534;
    private AbstractSubstitutionCost dCostFunction;
    private double gapCost;

    public SmithWaterman() : this(0.5, new SubCostRange1ToMinus2())
    {
    }

    public SmithWaterman(AbstractSubstitutionCost costFunction) : this(0.5, costFunction)
    {
    }

    public SmithWaterman(double costG) : this(costG, new SubCostRange1ToMinus2())
    {
    }

    public SmithWaterman(double costG, AbstractSubstitutionCost costFunction)
    {
        gapCost = costG;
        dCostFunction = costFunction;
    }

    public AbstractSubstitutionCost DCostFunction
    {
        get => dCostFunction;
        set => DCostFunction = value;
    }

    public double GapCost
    {
        get => gapCost;
        set => gapCost = value;
    }

    public override string LongDescriptionString => "Implements the Smith-Waterman algorithm providing a similarity measure between two string";

    public override string ShortDescriptionString => "SmithWaterman";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if ((firstWord == null) || (secondWord == null))
        {
            return 0.0;
        }
        var unnormalisedSimilarity = GetUnnormalisedSimilarity(firstWord, secondWord);
        double num2 = Math.Min(firstWord.Length, secondWord.Length);
        if (dCostFunction.MaxCost > -gapCost)
        {
            num2 *= dCostFunction.MaxCost;
        }
        else
        {
            num2 *= -gapCost;
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
            return ((((length * num2) + length) + num2) * 0.0001610000035725534);
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
            var thirdNumber = dCostFunction.GetCost(firstWord, j, secondWord, 0);
            if (j == 0)
            {
                numArray[0][0] = MathFunctions.MaxOf3(0.0, -gapCost, thirdNumber);
            }
            else
            {
                numArray[j][0] = MathFunctions.MaxOf3(0.0, numArray[j - 1][0] - gapCost, thirdNumber);
            }
            if (numArray[j][0] > num4)
            {
                num4 = numArray[j][0];
            }
        }
        for (var k = 0; k < num2; k++)
        {
            var num8 = dCostFunction.GetCost(firstWord, 0, secondWord, k);
            if (k == 0)
            {
                numArray[0][0] = MathFunctions.MaxOf3(0.0, -gapCost, num8);
            }
            else
            {
                numArray[0][k] = MathFunctions.MaxOf3(0.0, numArray[0][k - 1] - gapCost, num8);
            }
            if (numArray[0][k] > num4)
            {
                num4 = numArray[0][k];
            }
        }
        for (var m = 1; m < length; m++)
        {
            for (var n = 1; n < num2; n++)
            {
                var num11 = dCostFunction.GetCost(firstWord, m, secondWord, n);
                numArray[m][n] = MathFunctions.MaxOf4(0.0, numArray[m - 1][n] - gapCost, numArray[m][n - 1] - gapCost, numArray[m - 1][n - 1] + num11);
                if (numArray[m][n] > num4)
                {
                    num4 = numArray[m][n];
                }
            }
        }
        return num4;
    }
}