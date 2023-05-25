namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal class TokeniserUtilities<T>
{
    private Collection<T> allTokens;
    private int firstSetTokenCount;
    private int firstTokenCount;
    private int secondSetTokenCount;
    private int secondTokenCount;
    private Collection<T> tokenSet;

    public TokeniserUtilities()
    {
        allTokens = new Collection<T>();
        tokenSet = new Collection<T>();
    }

    public int FirstSetTokenCount => firstSetTokenCount;

    public int FirstTokenCount => firstTokenCount;

    public Collection<T> MergedTokens => allTokens;

    public int SecondSetTokenCount => secondSetTokenCount;

    public int SecondTokenCount => secondTokenCount;

    public Collection<T> TokenSet => tokenSet;

    public int CommonSetTerms()
    {
        return ((FirstSetTokenCount + SecondSetTokenCount) - tokenSet.Count);
    }

    public int CommonTerms()
    {
        return ((FirstTokenCount + SecondTokenCount) - allTokens.Count);
    }

    public Collection<T> CreateMergedList(Collection<T> firstTokens, Collection<T> secondTokens)
    {
        allTokens.Clear();
        firstTokenCount = firstTokens.Count;
        secondTokenCount = secondTokens.Count;
        MergeLists(firstTokens);
        MergeLists(secondTokens);
        return allTokens;
    }

    public Collection<T> CreateMergedSet(Collection<T> firstTokens, Collection<T> secondTokens)
    {
        tokenSet.Clear();
        firstSetTokenCount = CalculateUniqueTokensCount(firstTokens);
        secondSetTokenCount = CalculateUniqueTokensCount(secondTokens);
        MergeIntoSet(firstTokens);
        MergeIntoSet(secondTokens);
        return tokenSet;
    }

    public Collection<T> CreateSet(Collection<T> tokenList)
    {
        tokenSet.Clear();
        AddUniqueTokens(tokenList);
        firstTokenCount = tokenSet.Count;
        secondTokenCount = 0;
        return tokenSet;
    }

    public void MergeIntoSet(Collection<T> firstTokens)
    {
        AddUniqueTokens(firstTokens);
    }

    public void MergeLists(Collection<T> firstTokens)
    {
        AddTokens(firstTokens);
    }

    private void AddTokens(Collection<T> tokenList)
    {
        foreach (var local in tokenList)
        {
            allTokens.Add(local);
        }
    }

    private void AddUniqueTokens(Collection<T> tokenList)
    {
        foreach (var local in tokenList)
        {
            if (!tokenSet.Contains(local))
            {
                tokenSet.Add(local);
            }
        }
    }

    private int CalculateUniqueTokensCount(Collection<T> tokenList)
    {
        Collection<T> collection = new Collection<T>();
        foreach (var local in tokenList)
        {
            if (!collection.Contains(local))
            {
                collection.Add(local);
            }
        }
        return collection.Count;
    }
}