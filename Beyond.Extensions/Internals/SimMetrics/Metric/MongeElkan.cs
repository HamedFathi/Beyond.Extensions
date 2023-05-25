using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal class MongeElkan : AbstractStringMetric
{
    internal ITokeniser tokeniser;
    private const double defaultMismatchScore = 0.0;
    private double estimatedTimingConstant;
    private AbstractStringMetric internalStringMetric;

    public MongeElkan() : this(new TokeniserWhitespace())
    {
    }

    public MongeElkan(AbstractStringMetric metricToUse)
    {
        estimatedTimingConstant = 0.034400001168251038;
        tokeniser = new TokeniserWhitespace();
        internalStringMetric = metricToUse;
    }

    public MongeElkan(ITokeniser tokeniserToUse)
    {
        estimatedTimingConstant = 0.034400001168251038;
        tokeniser = tokeniserToUse;
        internalStringMetric = new SmithWatermanGotoh();
    }

    public MongeElkan(ITokeniser tokeniserToUse, AbstractStringMetric metricToUse)
    {
        estimatedTimingConstant = 0.034400001168251038;
        tokeniser = tokeniserToUse;
        internalStringMetric = metricToUse;
    }

    public override string LongDescriptionString => "Implements the Monge Elkan algorithm providing an matching style similarity measure between two strings";

    public override string ShortDescriptionString => "MongeElkan";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if ((firstWord == null) || (secondWord == null))
        {
            return 0.0;
        }
        Collection<string> collection = tokeniser.Tokenize(firstWord);
        Collection<string> collection2 = tokeniser.Tokenize(secondWord);
        var num = 0.0;
        for (var i = 0; i < collection.Count; i++)
        {
            var str = collection[i];
            var num3 = 0.0;
            for (var j = 0; j < collection2.Count; j++)
            {
                var str2 = collection2[j];
                var similarity = internalStringMetric.GetSimilarity(str, str2);
                if (similarity > num3)
                {
                    num3 = similarity;
                }
            }
            num += num3;
        }
        return (num / collection.Count);
    }

    public override string GetSimilarityExplained(string firstWord, string secondWord)
    {
        throw new NotImplementedException();
    }

    public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            double count = tokeniser.Tokenize(firstWord).Count;
            double num2 = tokeniser.Tokenize(secondWord).Count;
            return ((((count + num2) * count) + ((count + num2) * num2)) * estimatedTimingConstant);
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        return GetSimilarity(firstWord, secondWord);
    }
}