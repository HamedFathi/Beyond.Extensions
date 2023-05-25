using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class MatchingCoefficient : AbstractStringMetric
{
    private const double defaultMismatchScore = 0.0;
    private double estimatedTimingConstant;
    private ITokeniser tokeniser;
    private TokeniserUtilities<string> tokenUtilities;

    public MatchingCoefficient() : this(new TokeniserWhitespace())
    {
    }

    public MatchingCoefficient(ITokeniser tokeniserToUse)
    {
        estimatedTimingConstant = 0.00019999999494757503;
        tokeniser = tokeniserToUse;
        tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the Matching Coefficient algorithm providing a similarity measure between two strings";

    public override string ShortDescriptionString => "MatchingCoefficient";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            var unnormalisedSimilarity = GetUnnormalisedSimilarity(firstWord, secondWord);
            var num2 = Math.Max(tokenUtilities.FirstTokenCount, tokenUtilities.SecondTokenCount);
            return (unnormalisedSimilarity / num2);
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
            return ((num2 * count) * estimatedTimingConstant);
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        Collection<string> firstTokens = tokeniser.Tokenize(firstWord);
        Collection<string> secondTokens = tokeniser.Tokenize(secondWord);
        return GetActualSimilarity(firstTokens, secondTokens);
    }

    private double GetActualSimilarity(Collection<string> firstTokens, Collection<string> secondTokens)
    {
        tokenUtilities.CreateMergedList(firstTokens, secondTokens);
        var num = 0;
        foreach (var str in firstTokens)
        {
            if (secondTokens.Contains(str))
            {
                num++;
            }
        }
        return num;
    }
}