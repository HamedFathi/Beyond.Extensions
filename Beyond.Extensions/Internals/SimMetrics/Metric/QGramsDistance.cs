using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class QGramsDistance : AbstractStringMetric
{
    private const double DefaultMismatchScore = 0.0;
    private double _estimatedTimingConstant;
    private ITokeniser _tokeniser;
    private TokeniserUtilities<string> _tokenUtilities;

    public QGramsDistance() : this(new TokeniserQGram3Extended())
    {
    }

    public QGramsDistance(ITokeniser tokeniserToUse)
    {
        _estimatedTimingConstant = 0.0001340000017080456;
        _tokeniser = tokeniserToUse;
        _tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the Q Grams Distance algorithm providing a similarity measure between two strings using the qGram approach check matching qGrams/possible matching qGrams";

    public override string ShortDescriptionString => "QGramsDistance";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if (firstWord != null && secondWord != null)
        {
            var unnormalisedSimilarity = GetUnnormalisedSimilarity(firstWord, secondWord);
            var num2 = _tokenUtilities.FirstTokenCount + _tokenUtilities.SecondTokenCount;
            if (num2 != 0)
            {
                return (num2 - unnormalisedSimilarity) / num2;
            }
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
        Collection<string> firstTokens = _tokeniser.Tokenize(firstWord);
        Collection<string> secondTokens = _tokeniser.Tokenize(secondWord);
        _tokenUtilities.CreateMergedList(firstTokens, secondTokens);
        return GetActualSimilarity(firstTokens, secondTokens);
    }

    private double GetActualSimilarity(Collection<string> firstTokens, Collection<string> secondTokens)
    {
        Collection<string> collection = _tokenUtilities.CreateMergedSet(firstTokens, secondTokens);
        var num = 0;
        foreach (var str in collection)
        {
            var num2 = 0;
            for (var i = 0; i < firstTokens.Count; i++)
            {
                if (firstTokens[i].Equals(str))
                {
                    num2++;
                }
            }
            var num4 = 0;
            for (var j = 0; j < secondTokens.Count; j++)
            {
                if (secondTokens[j].Equals(str))
                {
                    num4++;
                }
            }
            if (num2 > num4)
            {
                num += num2 - num4;
            }
            else
            {
                num += num4 - num2;
            }
        }
        return num;
    }
}