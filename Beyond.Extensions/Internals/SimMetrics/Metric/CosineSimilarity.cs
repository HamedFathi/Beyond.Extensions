using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class CosineSimilarity : AbstractStringMetric
{
    private double estimatedTimingConstant;
    private ITokeniser tokeniser;
    private TokeniserUtilities<string> tokenUtilities;

    public CosineSimilarity() : this(new TokeniserWhitespace())
    {
    }

    public CosineSimilarity(ITokeniser tokeniserToUse)
    {
        estimatedTimingConstant = 3.8337140040312079E-07;
        tokeniser = tokeniserToUse;
        tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the Cosine Similarity algorithm providing a similarity measure between two strings from the angular divergence within term based vector space";

    public override string ShortDescriptionString => "CosineSimilarity";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if (((firstWord != null) && (secondWord != null)) && (tokenUtilities.CreateMergedSet(tokeniser.Tokenize(firstWord), tokeniser.Tokenize(secondWord)).Count > 0))
        {
            return (tokenUtilities.CommonSetTerms() / (Math.Pow(tokenUtilities.FirstSetTokenCount, 0.5) * Math.Pow(tokenUtilities.SecondSetTokenCount, 0.5)));
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