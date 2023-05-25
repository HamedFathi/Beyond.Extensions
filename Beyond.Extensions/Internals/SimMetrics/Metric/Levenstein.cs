using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class Levenstein : AbstractStringMetric
{
    private const double DefaultMismatchScore = 0.0;
    private const double DefaultPerfectMatchScore = 1.0;
    private AbstractSubstitutionCost _dCostFunction = new SubCostRange0To1();
    private double _estimatedTimingConstant = 0.00018000000272877514;

    public override string LongDescriptionString => "Implements the basic Levenstein algorithm providing a similarity measure between two strings";

    public override string ShortDescriptionString => "Levenstein";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if (firstWord == null || secondWord == null)
        {
            return 0.0;
        }
        var unnormalisedSimilarity = GetUnnormalisedSimilarity(firstWord, secondWord);
        double length = firstWord.Length;
        if (length < secondWord.Length)
        {
            length = secondWord.Length;
        }
        if (length == 0.0)
        {
            return 1.0;
        }
        return 1.0 - unnormalisedSimilarity / length;
    }

    public override string GetSimilarityExplained(string firstWord, string secondWord)
    {
        throw new NotImplementedException();
    }

    public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
    {
        if (firstWord != null && secondWord != null)
        {
            double length = firstWord.Length;
            double num2 = secondWord.Length;
            return length * num2 * _estimatedTimingConstant;
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        if (firstWord == null || secondWord == null)
        {
            return 0.0;
        }
        var length = firstWord.Length;
        var index = secondWord.Length;
        if (length == 0)
        {
            return index;
        }
        if (index == 0)
        {
            return length;
        }
        double[][] numArray = new double[length + 1][];
        for (var i = 0; i < length + 1; i++)
        {
            numArray[i] = new double[index + 1];
        }
        for (var j = 0; j <= length; j++)
        {
            numArray[j][0] = j;
        }
        for (var k = 0; k <= index; k++)
        {
            numArray[0][k] = k;
        }
        for (var m = 1; m <= length; m++)
        {
            for (var n = 1; n <= index; n++)
            {
                var num8 = _dCostFunction.GetCost(firstWord, m - 1, secondWord, n - 1);
                numArray[m][n] = MathFunctions.MinOf3(numArray[m - 1][n] + 1.0, numArray[m][n - 1] + 1.0, numArray[m - 1][n - 1] + num8);
            }
        }
        return numArray[length][index];
    }
}