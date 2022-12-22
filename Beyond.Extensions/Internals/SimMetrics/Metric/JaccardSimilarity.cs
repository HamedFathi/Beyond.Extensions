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
        this.estimatedTimingConstant = 0.00014000000373926014;
        this.tokeniser = tokeniserToUse;
        this.tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the Jaccard Similarity algorithm providing a similarity measure between two strings";

    public override string ShortDescriptionString => "JaccardSimilarity";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            Collection<string> collection = this.tokenUtilities.CreateMergedSet(this.tokeniser.Tokenize(firstWord), this.tokeniser.Tokenize(secondWord));
            if (collection.Count > 0)
            {
                return (((double)this.tokenUtilities.CommonSetTerms()) / ((double)collection.Count));
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
            double count = this.tokeniser.Tokenize(firstWord).Count;
            double num2 = this.tokeniser.Tokenize(secondWord).Count;
            return ((count * num2) * this.estimatedTimingConstant);
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        return this.GetSimilarity(firstWord, secondWord);
    }
}