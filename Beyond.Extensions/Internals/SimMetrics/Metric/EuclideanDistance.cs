using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class EuclideanDistance : AbstractStringMetric
{
    private const double defaultMismatchScore = 0.0;
    private double estimatedTimingConstant;
    private ITokeniser tokeniser;
    private TokeniserUtilities<string> tokenUtilities;

    public EuclideanDistance() : this(new TokeniserWhitespace())
    {
    }

    public EuclideanDistance(ITokeniser tokeniserToUse)
    {
        estimatedTimingConstant = 7.4457137088757008E-05;
        tokeniser = tokeniserToUse;
        tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the Euclidean Distancey algorithm providing a similarity measure between two stringsusing the vector space of combined terms as the dimensions";

    public override string ShortDescriptionString => "EuclideanDistance";

    public double GetEuclidDistance(string firstWord, string secondWord)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            Collection<string> firstTokens = tokeniser.Tokenize(firstWord);
            Collection<string> secondTokens = tokeniser.Tokenize(secondWord);
            return GetActualDistance(firstTokens, secondTokens);
        }
        return 0.0;
    }

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            var unnormalisedSimilarity = GetUnnormalisedSimilarity(firstWord, secondWord);
            var num2 = Math.Sqrt(tokenUtilities.FirstTokenCount + tokenUtilities.SecondTokenCount);
            return ((num2 - unnormalisedSimilarity) / num2);
        }
        return 0.0;
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
        return GetEuclidDistance(firstWord, secondWord);
    }

    private double GetActualDistance(Collection<string> firstTokens, Collection<string> secondTokens)
    {
        Collection<string> collection = tokenUtilities.CreateMergedList(firstTokens, secondTokens);
        var num = 0;
        foreach (var str in collection)
        {
            var num2 = 0;
            var num3 = 0;
            if (firstTokens.Contains(str))
            {
                num2++;
            }
            if (secondTokens.Contains(str))
            {
                num3++;
            }
            num += (num2 - num3) * (num2 - num3);
        }
        return Math.Sqrt(num);
    }
}