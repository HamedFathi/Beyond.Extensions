namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal class TokeniserUtilities<T>
{
    private Collection<T> _allTokens;
    private int _firstSetTokenCount;
    private int _firstTokenCount;
    private int _secondSetTokenCount;
    private int _secondTokenCount;
    private Collection<T> _tokenSet;

    public TokeniserUtilities()
    {
        _allTokens = new Collection<T>();
        _tokenSet = new Collection<T>();
    }

    public int FirstSetTokenCount => _firstSetTokenCount;

    public int FirstTokenCount => _firstTokenCount;

    public Collection<T> MergedTokens => _allTokens;

    public int SecondSetTokenCount => _secondSetTokenCount;

    public int SecondTokenCount => _secondTokenCount;

    public Collection<T> TokenSet => _tokenSet;

    public int CommonSetTerms()
    {
        return FirstSetTokenCount + SecondSetTokenCount - _tokenSet.Count;
    }

    public int CommonTerms()
    {
        return FirstTokenCount + SecondTokenCount - _allTokens.Count;
    }

    public Collection<T> CreateMergedList(Collection<T> firstTokens, Collection<T> secondTokens)
    {
        _allTokens.Clear();
        _firstTokenCount = firstTokens.Count;
        _secondTokenCount = secondTokens.Count;
        MergeLists(firstTokens);
        MergeLists(secondTokens);
        return _allTokens;
    }

    public Collection<T> CreateMergedSet(Collection<T> firstTokens, Collection<T> secondTokens)
    {
        _tokenSet.Clear();
        _firstSetTokenCount = CalculateUniqueTokensCount(firstTokens);
        _secondSetTokenCount = CalculateUniqueTokensCount(secondTokens);
        MergeIntoSet(firstTokens);
        MergeIntoSet(secondTokens);
        return _tokenSet;
    }

    public Collection<T> CreateSet(Collection<T> tokenList)
    {
        _tokenSet.Clear();
        AddUniqueTokens(tokenList);
        _firstTokenCount = _tokenSet.Count;
        _secondTokenCount = 0;
        return _tokenSet;
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
            _allTokens.Add(local);
        }
    }

    private void AddUniqueTokens(Collection<T> tokenList)
    {
        foreach (var local in tokenList)
        {
            if (!_tokenSet.Contains(local))
            {
                _tokenSet.Add(local);
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