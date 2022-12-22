namespace Beyond.Extensions.Internals.SimMetrics.API;

internal interface IStringMetric
{
    string LongDescriptionString { get; }

    string ShortDescriptionString { get; }

    double GetSimilarity(string firstWord, string secondWord);

    string GetSimilarityExplained(string firstWord, string secondWord);

    long GetSimilarityTimingActual(string firstWord, string secondWord);

    double GetSimilarityTimingEstimated(string firstWord, string secondWord);

    double GetUnnormalisedSimilarity(string firstWord, string secondWord);
}