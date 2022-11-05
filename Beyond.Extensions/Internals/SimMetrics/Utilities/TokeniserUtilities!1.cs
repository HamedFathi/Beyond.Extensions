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
        this.allTokens = new Collection<T>();
        this.tokenSet = new Collection<T>();
    }

    private void AddTokens(Collection<T> tokenList)
    {
        foreach (T local in tokenList)
        {
            this.allTokens.Add(local);
        }
    }

    private void AddUniqueTokens(Collection<T> tokenList)
    {
        foreach (T local in tokenList)
        {
            if (!this.tokenSet.Contains(local))
            {
                this.tokenSet.Add(local);
            }
        }
    }

    private int CalculateUniqueTokensCount(Collection<T> tokenList)
    {
        Collection<T> collection = new Collection<T>();
        foreach (T local in tokenList)
        {
            if (!collection.Contains(local))
            {
                collection.Add(local);
            }
        }
        return collection.Count;
    }

    public int CommonSetTerms()
    {
        return ((this.FirstSetTokenCount + this.SecondSetTokenCount) - this.tokenSet.Count);
    }

    public int CommonTerms()
    {
        return ((this.FirstTokenCount + this.SecondTokenCount) - this.allTokens.Count);
    }

    public Collection<T> CreateMergedList(Collection<T> firstTokens, Collection<T> secondTokens)
    {
        this.allTokens.Clear();
        this.firstTokenCount = firstTokens.Count;
        this.secondTokenCount = secondTokens.Count;
        this.MergeLists(firstTokens);
        this.MergeLists(secondTokens);
        return this.allTokens;
    }

    public Collection<T> CreateMergedSet(Collection<T> firstTokens, Collection<T> secondTokens)
    {
        this.tokenSet.Clear();
        this.firstSetTokenCount = this.CalculateUniqueTokensCount(firstTokens);
        this.secondSetTokenCount = this.CalculateUniqueTokensCount(secondTokens);
        this.MergeIntoSet(firstTokens);
        this.MergeIntoSet(secondTokens);
        return this.tokenSet;
    }

    public Collection<T> CreateSet(Collection<T> tokenList)
    {
        this.tokenSet.Clear();
        this.AddUniqueTokens(tokenList);
        this.firstTokenCount = this.tokenSet.Count;
        this.secondTokenCount = 0;
        return this.tokenSet;
    }

    public void MergeIntoSet(Collection<T> firstTokens)
    {
        this.AddUniqueTokens(firstTokens);
    }

    public void MergeLists(Collection<T> firstTokens)
    {
        this.AddTokens(firstTokens);
    }

    public int FirstSetTokenCount => this.firstSetTokenCount;

    public int FirstTokenCount => this.firstTokenCount;

    public Collection<T> MergedTokens => this.allTokens;

    public int SecondSetTokenCount => this.secondSetTokenCount;

    public int SecondTokenCount => this.secondTokenCount;

    public Collection<T> TokenSet => this.tokenSet;
}