using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal class MongeElkan : AbstractStringMetric
{
    internal ITokeniser Tokeniser;
    private const double DefaultMismatchScore = 0.0;
    private double _estimatedTimingConstant;
    private AbstractStringMetric _internalStringMetric;

    public MongeElkan() : this(new TokeniserWhitespace())
    {
    }

    public MongeElkan(AbstractStringMetric metricToUse)
    {
        _estimatedTimingConstant = 0.034400001168251038;
        Tokeniser = new TokeniserWhitespace();
        _internalStringMetric = metricToUse;
    }

    public MongeElkan(ITokeniser tokeniserToUse)
    {
        _estimatedTimingConstant = 0.034400001168251038;
        Tokeniser = tokeniserToUse;
        _internalStringMetric = new SmithWatermanGotoh();
    }

    public MongeElkan(ITokeniser tokeniserToUse, AbstractStringMetric metricToUse)
    {
        _estimatedTimingConstant = 0.034400001168251038;
        Tokeniser = tokeniserToUse;
        _internalStringMetric = metricToUse;
    }

    public override string LongDescriptionString => "Implements the Monge Elkan algorithm providing an matching style similarity measure between two strings";

    public override string ShortDescriptionString => "MongeElkan";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if (firstWord == null || secondWord == null)
        {
            return 0.0;
        }
        Collection<string> collection = Tokeniser.Tokenize(firstWord);
        Collection<string> collection2 = Tokeniser.Tokenize(secondWord);
        var num = 0.0;
        for (var i = 0; i < collection.Count; i++)
        {
            var str = collection[i];
            var num3 = 0.0;
            for (var j = 0; j < collection2.Count; j++)
            {
                var str2 = collection2[j];
                var similarity = _internalStringMetric.GetSimilarity(str, str2);
                if (similarity > num3)
                {
                    num3 = similarity;
                }
            }
            num += num3;
        }
        return num / collection.Count;
    }

    public override string GetSimilarityExplained(string firstWord, string secondWord)
    {
        throw new NotImplementedException();
    }

    public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
    {
        if (firstWord != null && secondWord != null)
        {
            double count = Tokeniser.Tokenize(firstWord).Count;
            double num2 = Tokeniser.Tokenize(secondWord).Count;
            return ((count + num2) * count + (count + num2) * num2) * _estimatedTimingConstant;
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        return GetSimilarity(firstWord, secondWord);
    }
}