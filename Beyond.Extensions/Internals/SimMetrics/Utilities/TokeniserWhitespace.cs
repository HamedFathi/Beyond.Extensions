using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal sealed class TokeniserWhitespace : ITokeniser
{
    private string delimiters = "\r\n\t \x00a0";
    private ITermHandler stopWordHandler = new DummyStopTermHandler();
    private TokeniserUtilities<string> tokenUtilities = new TokeniserUtilities<string>();

    public string Delimiters => delimiters;

    public string ShortDescriptionString => "TokeniserWhitespace";

    public ITermHandler StopWordHandler
    {
        get => stopWordHandler;
        set => stopWordHandler = value;
    }

    public Collection<string> Tokenize(string word)
    {
        Collection<string> collection = new Collection<string>();
        if (word != null)
        {
            int length;
            for (var i = 0; i < word.Length; i = length)
            {
                var c = word[i];
                if (char.IsWhiteSpace(c))
                {
                    i++;
                }
                length = word.Length;
                for (var j = 0; j < delimiters.Length; j++)
                {
                    var index = word.IndexOf(delimiters[j], i);
                    if ((index < length) && (index != -1))
                    {
                        length = index;
                    }
                }
                var termToTest = word.Substring(i, length - i);
                if (!stopWordHandler.IsWord(termToTest))
                {
                    collection.Add(termToTest);
                }
            }
        }
        return collection;
    }

    public Collection<string> TokenizeToSet(string word)
    {
        if (word != null)
        {
            return tokenUtilities.CreateSet(Tokenize(word));
        }
        return null;
    }
}