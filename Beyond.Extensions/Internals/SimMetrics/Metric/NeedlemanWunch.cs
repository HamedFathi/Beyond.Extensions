using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class NeedlemanWunch : AbstractStringMetric
{
    private const double DefaultGapCost = 2.0;
    private const double DefaultMismatchScore = 0.0;
    private const double DefaultPerfectMatchScore = 1.0;
    private AbstractSubstitutionCost _dCostFunction;
    private double _estimatedTimingConstant;
    private double _gapCost;

    public NeedlemanWunch() : this(2.0, new SubCostRange0To1())
    {
    }

    public NeedlemanWunch(AbstractSubstitutionCost costFunction) : this(2.0, costFunction)
    {
    }

    public NeedlemanWunch(double costG) : this(costG, new SubCostRange0To1())
    {
    }

    public NeedlemanWunch(double costG, AbstractSubstitutionCost costFunction)
    {
        _estimatedTimingConstant = 0.00018420000560581684;
        _gapCost = costG;
        _dCostFunction = costFunction;
    }

    public AbstractSubstitutionCost DCostFunction
    {
        get => _dCostFunction;
        set => _dCostFunction = value;
    }

    public double GapCost
    {
        get => _gapCost;
        set => _gapCost = value;
    }

    public override string LongDescriptionString => "Implements the Needleman-Wunch algorithm providing an edit distance based similarity measure between two strings";

    public override string ShortDescriptionString => "NeedlemanWunch";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if (firstWord == null || secondWord == null)
        {
            return 0.0;
        }
        var unnormalisedSimilarity = GetUnnormalisedSimilarity(firstWord, secondWord);
        double num2 = Math.Max(firstWord.Length, secondWord.Length);
        var num3 = num2;
        if (_dCostFunction.MaxCost > _gapCost)
        {
            num2 *= _dCostFunction.MaxCost;
        }
        else
        {
            num2 *= _gapCost;
        }
        if (_dCostFunction.MinCost < _gapCost)
        {
            num3 *= _dCostFunction.MinCost;
        }
        else
        {
            num3 *= _gapCost;
        }
        if (num3 < 0.0)
        {
            num2 -= num3;
            unnormalisedSimilarity -= num3;
        }
        if (num2 == 0.0)
        {
            return 1.0;
        }
        return 1.0 - unnormalisedSimilarity / num2;
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
                numArray[m][n] = MathFunctions.MinOf3(numArray[m - 1][n] + _gapCost, numArray[m][n - 1] + _gapCost, numArray[m - 1][n - 1] + num8);
            }
        }
        return numArray[length][index];
    }
}