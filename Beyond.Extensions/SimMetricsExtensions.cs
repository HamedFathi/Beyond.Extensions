// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using Beyond.Extensions.EnumerableExtended;
using Beyond.Extensions.Enums;
using Beyond.Extensions.Internals.SimMetrics.Metric;
using Beyond.Extensions.StringExtended;
using Beyond.Extensions.Types;

namespace Beyond.Extensions.SimMetricsExtended;

public static class SimMetricsExtensions
{
    public static IEnumerable<string> ContainsFuzzy(this string[] source, string search, double threshold = 0.7,
        SimMetricType simMetricType = SimMetricType.Levenstein, Func<string, string>? manipulator = null)
    {
        foreach (var item in source)
        {
            var data = manipulator == null ? item : manipulator(item);
            var status = data.ContainsFuzzy(search, threshold, simMetricType);
            if (status)
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<string> ContainsFuzzy(this IEnumerable<string> source, string search,
        double threshold = 0.7, SimMetricType simMetricType = SimMetricType.Levenstein,
        Func<string, string>? manipulator = null)
    {
        foreach (var item in source)
        {
            var data = manipulator == null ? item : manipulator(item);
            var status = data.ContainsFuzzy(search, threshold, simMetricType);
            if (status)
            {
                yield return item;
            }
        }
    }

    public static bool ContainsFuzzy(this string source, string search, double threshold = 0.7,
        SimMetricType simMetricType = SimMetricType.Levenstein, Func<string, string>? manipulator = null)
    {
        if (source.ContainsIgnoreCase(search)) return true;

        var words = source.Split(' ').Select(x => x.Trim()).ToList();
        if (manipulator != null)
        {
            words = words.Select(manipulator).ToList();
        }

        if (words.ContainsIgnoreCase(search)) return true;
        foreach (var word in words)
        {
            var score = search.ToLowerInvariant().GetSimilarity(word.ToLowerInvariant(), simMetricType);
            if (score >= threshold)
            {
                return true;
            }
        }

        return false;
    }

    public static IEnumerable<string> GetMaximumSimilarities(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        var word = list.OrderBy(x => x.Score).Last();
        var result = list.Where(x => x.Score == word.Score).Select(x => x.Item);

        return result;
    }

    public static IEnumerable<SimMetricResult> GetMaximumSimilaritiesInfo(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        var word = list.OrderBy(x => x.Score).Last();
        var result = list.Where(x => x.Score == word.Score);

        return result;
    }

    public static string GetMaximumSimilarity(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        var result = list.OrderBy(x => x.Score).Last().Item;
        return result;
    }

    public static SimMetricResult GetMaximumSimilarityInfo(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        var result = list.OrderBy(x => x.Score).Last();
        return result;
    }

    public static IEnumerable<string> GetMinimumSimilarities(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        var word = list.OrderBy(x => x.Score).First();
        var result = list.Where(x => x.Score == word.Score).Select(x => x.Item);

        return result;
    }

    public static IEnumerable<SimMetricResult> GetMinimumSimilaritiesInfo(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        var word = list.OrderBy(x => x.Score).First();
        var result = list.Where(x => x.Score == word.Score);

        return result;
    }

    public static string GetMinimumSimilarity(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        var result = list.OrderBy(x => x.Score).First().Item;
        return result;
    }

    public static SimMetricResult GetMinimumSimilarityInfo(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        var result = list.OrderBy(x => x.Score).First();
        return result;
    }

    public static IEnumerable<SimMetricResult> GetSimilarities(this string first, string[] second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        return list;
    }

    public static IEnumerable<SimMetricResult> GetSimilarities(this string first, IEnumerable<string> second,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        var list = new List<SimMetricResult>();
        foreach (var item in second)
        {
            var data = manipulator != null ? manipulator(item) : item;
            var rank = first.GetSimilarity(data, simMetricType);
            list.Add(new SimMetricResult { Item = item, Score = convertToPercentage.GetPercentageIf(rank) });
        }

        return list;
    }

    public static double GetSimilarity(this string firstWord, string secondWord,
        SimMetricType simMetricType = SimMetricType.Levenstein, bool convertToPercentage = false,
        Func<string, string>? manipulator = null)
    {
        if (manipulator != null)
        {
            secondWord = manipulator(secondWord);
        }

        switch (simMetricType)
        {
            case SimMetricType.Levenstein:
                var sim1 = new Levenstein();

                return convertToPercentage.GetPercentageIf(sim1.GetSimilarity(firstWord, secondWord));

            case SimMetricType.BlockDistance:
                var sim2 = new BlockDistance();
                return convertToPercentage.GetPercentageIf(sim2.GetSimilarity(firstWord, secondWord));

            case SimMetricType.ChapmanLengthDeviation:
                var sim3 = new ChapmanLengthDeviation();
                return convertToPercentage.GetPercentageIf(sim3.GetSimilarity(firstWord, secondWord));

            case SimMetricType.CosineSimilarity:
                var sim4 = new CosineSimilarity();
                return convertToPercentage.GetPercentageIf(sim4.GetSimilarity(firstWord, secondWord));

            case SimMetricType.DiceSimilarity:
                var sim5 = new DiceSimilarity();
                return convertToPercentage.GetPercentageIf(sim5.GetSimilarity(firstWord, secondWord));

            case SimMetricType.EuclideanDistance:
                var sim6 = new EuclideanDistance();
                return convertToPercentage.GetPercentageIf(sim6.GetSimilarity(firstWord, secondWord));

            case SimMetricType.JaccardSimilarity:
                var sim7 = new JaccardSimilarity();
                return convertToPercentage.GetPercentageIf(sim7.GetSimilarity(firstWord, secondWord));

            case SimMetricType.Jaro:
                var sim8 = new Jaro();
                return convertToPercentage.GetPercentageIf(sim8.GetSimilarity(firstWord, secondWord));

            case SimMetricType.JaroWinkler:
                var sim9 = new JaroWinkler();
                return convertToPercentage.GetPercentageIf(sim9.GetSimilarity(firstWord, secondWord));

            case SimMetricType.MatchingCoefficient:
                var sim10 = new MatchingCoefficient();
                return convertToPercentage.GetPercentageIf(sim10.GetSimilarity(firstWord, secondWord));

            case SimMetricType.MongeElkan:
                var sim11 = new MongeElkan();
                return convertToPercentage.GetPercentageIf(sim11.GetSimilarity(firstWord, secondWord));

            case SimMetricType.NeedlemanWunch:
                var sim12 = new NeedlemanWunch();
                return convertToPercentage.GetPercentageIf(sim12.GetSimilarity(firstWord, secondWord));

            case SimMetricType.OverlapCoefficient:
                var sim13 = new OverlapCoefficient();
                return convertToPercentage.GetPercentageIf(sim13.GetSimilarity(firstWord, secondWord));

            case SimMetricType.QGramsDistance:
                var sim14 = new QGramsDistance();
                return convertToPercentage.GetPercentageIf(sim14.GetSimilarity(firstWord, secondWord));

            case SimMetricType.SmithWaterman:
                var sim15 = new SmithWaterman();
                return convertToPercentage.GetPercentageIf(sim15.GetSimilarity(firstWord, secondWord));

            case SimMetricType.SmithWatermanGotoh:
                var sim16 = new SmithWatermanGotoh();
                return convertToPercentage.GetPercentageIf(sim16.GetSimilarity(firstWord, secondWord));

            case SimMetricType.SmithWatermanGotohWindowedAffine:
                var sim17 = new SmithWatermanGotohWindowedAffine();
                return convertToPercentage.GetPercentageIf(sim17.GetSimilarity(firstWord, secondWord));

            case SimMetricType.ChapmanMeanLength:
                var sim18 = new ChapmanMeanLength();
                return convertToPercentage.GetPercentageIf(sim18.GetSimilarity(firstWord, secondWord));

            default:
                var sim0 = new Levenstein();
                return convertToPercentage.GetPercentageIf(sim0.GetSimilarity(firstWord, secondWord));
        }
    }

    private static double GetPercentageIf(this bool convertToPercentage, double rank)
    {
        if (convertToPercentage)
        {
            return rank * 100;
        }

        return rank;
    }
}