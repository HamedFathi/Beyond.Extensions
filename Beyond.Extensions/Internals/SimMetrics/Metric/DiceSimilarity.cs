using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class DiceSimilarity : AbstractStringMetric
{
    private double estimatedTimingConstant;
    private ITokeniser tokeniser;
    private TokeniserUtilities<string> tokenUtilities;

    public DiceSimilarity() : this(new TokeniserWhitespace())
    {
    }

    public DiceSimilarity(ITokeniser tokeniserToUse)
    {
        estimatedTimingConstant = 3.4457139008736704E-07;
        tokeniser = tokeniserToUse;
        tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the DiceSimilarity algorithm providing a similarity measure between two strings using the vector space of present terms";

    public override string ShortDescriptionString => "DiceSimilarity";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if (((firstWord != null) && (secondWord != null)) && (tokenUtilities.CreateMergedSet(tokeniser.Tokenize(firstWord), tokeniser.Tokenize(secondWord)).Count > 0))
        {
            return ((2.0 * tokenUtilities.CommonSetTerms()) / (tokenUtilities.FirstSetTokenCount + tokenUtilities.SecondSetTokenCount));
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
            double length = firstWord.Length;
            double num2 = secondWord.Length;
            return ((length + num2) * ((length + num2) * estimatedTimingConstant));
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        return GetSimilarity(firstWord, secondWord);
    }
}