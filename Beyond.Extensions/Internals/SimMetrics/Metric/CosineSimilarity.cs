﻿using Beyond.Extensions.Internals.SimMetrics.API;
using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.Metric;

internal sealed class CosineSimilarity : AbstractStringMetric
{
    private double _estimatedTimingConstant;
    private ITokeniser _tokeniser;
    private TokeniserUtilities<string> _tokenUtilities;

    public CosineSimilarity() : this(new TokeniserWhitespace())
    {
    }

    public CosineSimilarity(ITokeniser tokeniserToUse)
    {
        _estimatedTimingConstant = 3.8337140040312079E-07;
        _tokeniser = tokeniserToUse;
        _tokenUtilities = new TokeniserUtilities<string>();
    }

    public override string LongDescriptionString => "Implements the Cosine Similarity algorithm providing a similarity measure between two strings from the angular divergence within term based vector space";

    public override string ShortDescriptionString => "CosineSimilarity";

    public override double GetSimilarity(string firstWord, string secondWord)
    {
        if (firstWord != null && secondWord != null && _tokenUtilities.CreateMergedSet(_tokeniser.Tokenize(firstWord), _tokeniser.Tokenize(secondWord)).Count > 0)
        {
            return _tokenUtilities.CommonSetTerms() / (Math.Pow(_tokenUtilities.FirstSetTokenCount, 0.5) * Math.Pow(_tokenUtilities.SecondSetTokenCount, 0.5));
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
            return (length + num2) * ((length + num2) * _estimatedTimingConstant);
        }
        return 0.0;
    }

    public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
    {
        return GetSimilarity(firstWord, secondWord);
    }
}