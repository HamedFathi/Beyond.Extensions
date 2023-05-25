using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class JaroWinkler : AbstractStringMetric
{
    private const int MinPrefixTestLength = 4;
    private const double PrefixAdustmentScale = 0.10000000149011612;
    private double _estimatedTimingConstant = 4.3420001020422205E-05;
    private AbstractStringMetric _jaroStringMetric = new Jaro();
    public override string LongDescriptionString => "Implements the Jaro-Winkler algorithm providing a similarity measure between two strings allowing character transpositions to a degree adjusting the weighting for common prefixes";

    public override string ShortDescriptionString => "JaroWinkler";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if (firstWord != null && secondWord != null)
        {
            var similarity = _jaroStringMetric.GetSimilarity(firstWord, secondWord);
            var prefixLength = GetPrefixLength(firstWord, secondWord);
            return similarity + prefixLength * 0.10000000149011612 * (1.0 - similarity);
        }
        return 0.0;
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
        return GetSimilarity(firstWord, secondWord);
    }

    private static int GetPrefixLength(string firstWord, string secondWord)
    {
        if (firstWord == null || secondWord == null)
        {
            return 4;
        }
        var num = MathFunctions.MinOf3(4, firstWord.Length, secondWord.Length);
        for (var i = 0; i < num; i++)
        {
            if (firstWord[i] != secondWord[i])
            {
                return i;
            }
        }
        return num;
    }
}