using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class JaccardSimilarity : AbstractStringMetric
{
    private const double defaultMismatchScore = 0.0;
    private double estimatedTimingConstant;
    private ITokeniser tokeniser;
    private TokeniserUtilities<string> tokenUtilities;

    public JaccardSimilarity() : this(new TokeniserWhitespace())
    {
    }

    public JaccardSimilarity(ITokeniser tokeniserToUse)
    {
        estimatedTimingConstant = 0.00014000000373926014;
        tokeniser = tokeniserToUse;
        tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the Jaccard Similarity algorithm providing a similarity measure between two strings";

    public override string ShortDescriptionString => "JaccardSimilarity";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            Collection<string> collection = tokenUtilities.CreateMergedSet(tokeniser.Tokenize(firstWord), tokeniser.Tokenize(secondWord));
            if (collection.Count > 0)
            {
                return (tokenUtilities.CommonSetTerms() / ((double)collection.Count));
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
        if ((firstWord != null) && (secondWord != null))
        {
            double count = tokeniser.Tokenize(firstWord).Count;
            double num2 = tokeniser.Tokenize(secondWord).Count;
            return ((count * num2) * estimatedTimingConstant);
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        return GetSimilarity(firstWord, secondWord);
    }
}