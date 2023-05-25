using Beyond.Extensions.Internals.SimMetrics.Utilities;

namespace Beyond.Extensions.Internals.SimMetrics.API;

internal abstract class AbstractTokeniserQGramN : ITokeniser
{
    private const string defaultEndPadCharacter = "#";
    private const string defaultStartPadCharacter = "?";
    private int characterCombinationIndex;
    private int qGramLength;
    private ITermHandler stopWordHandler;
    private string suppliedWord;
    private TokeniserUtilities<string> tokenUtilities;

    public int CharacterCombinationIndex
    {
        get => characterCombinationIndex;
        set => characterCombinationIndex = value;
    }

    public string Delimiters => string.Empty;

    public int QGramLength
    {
        get => qGramLength;
        set => qGramLength = value;
    }

    public abstract string ShortDescriptionString { get; }

    public ITermHandler StopWordHandler
    {
        get => stopWordHandler;
        set => stopWordHandler = value;
    }

    public string SuppliedWord
    {
        get => suppliedWord;
        set => suppliedWord = value;
    }

    public TokeniserUtilities<string> TokenUtilities
    {
        get => tokenUtilities;
        set => tokenUtilities = value;
    }

    public abstract Collection<string> Tokenize(string word);

    public Collection<string> Tokenize(string word, bool extended, int tokenLength, int characterCombinationIndexValue)
    {
        int num3;
        if (string.IsNullOrEmpty(word))
        {
            return null;
        }
        SuppliedWord = word;
        Collection<string> collection = new Collection<string>();
        var length = word.Length;
        var count = 0;
        if (tokenLength > 0)
        {
            count = tokenLength - 1;
        }
        var builder = new StringBuilder(length + (2 * count));
        if (extended)
        {
            builder.Insert(0, "?", count);
        }
        builder.Append(word);
        if (extended)
        {
            builder.Insert(builder.Length, "#", count);
        }
        var str = builder.ToString();
        if (extended)
        {
            num3 = length + count;
        }
        else
        {
            num3 = (length - tokenLength) + 1;
        }
        for (var i = 0; i < num3; i++)
        {
            var termToTest = str.Substring(i, tokenLength);
            if (!stopWordHandler.IsWord(termToTest))
            {
                collection.Add(termToTest);
            }
        }
        if (characterCombinationIndexValue != 0)
        {
            str = builder.ToString();
            num3--;
            for (var j = 0; j < num3; j++)
            {
                var str3 = str.Substring(j, count) + str.Substring(j + tokenLength, 1);
                if (!stopWordHandler.IsWord(str3) && !collection.Contains(str3))
                {
                    collection.Add(str3);
                }
            }
        }
        return collection;
    }

    public Collection<string> TokenizeToSet(string word)
    {
        if (!string.IsNullOrEmpty(word))
        {
            SuppliedWord = word;
            return TokenUtilities.CreateSet(Tokenize(word));
        }
        return null;
    }
}