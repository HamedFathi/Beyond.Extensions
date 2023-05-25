using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class BlockDistance : AbstractStringMetric
{
    private double estimatedTimingConstant;
    private ITokeniser tokeniser;
    private TokeniserUtilities<string> tokenUtilities;

    public BlockDistance() : this(new TokeniserWhitespace())
    {
    }

    public BlockDistance(ITokeniser tokeniserToUse)
    {
        estimatedTimingConstant = 6.4457140979357064E-05;
        tokeniser = tokeniserToUse;
        tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the Block distance algorithm whereby vector space block distance is used to determine a similarity";

    public override string ShortDescriptionString => "BlockDistance";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        Collection<string> firstTokens = tokeniser.Tokenize(firstWord);
        Collection<string> secondTokens = tokeniser.Tokenize(secondWord);
        var num = firstTokens.Count + secondTokens.Count;
        var actualSimilarity = GetActualSimilarity(firstTokens, secondTokens);
        return ((num - actualSimilarity) / num);
    }

    public override string GetSimilarityExplained(string firstWord, string secondWord)
    {
        throw new NotImplementedException();
    }

    public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
    {
        double count = tokeniser.Tokenize(firstWord).Count;
        double num2 = tokeniser.Tokenize(secondWord).Count;
        return ((((count + num2) * count) + ((count + num2) * num2)) * estimatedTimingConstant);
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        Collection<string> firstTokens = tokeniser.Tokenize(firstWord);
        Collection<string> secondTokens = tokeniser.Tokenize(secondWord);
        return GetActualSimilarity(firstTokens, secondTokens);
    }

    private double GetActualSimilarity(Collection<string> firstTokens, Collection<string> secondTokens)
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
            if (num2 > num3)
            {
                num += num2 - num3;
            }
            else
            {
                num += num3 - num2;
            }
        }
        return num;
    }
}