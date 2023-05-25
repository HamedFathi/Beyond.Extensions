using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal sealed class TokeniserWhitespace : ITokeniser
{
    private string _delimiters = "\r\n\t \x00a0";
    private ITermHandler _stopWordHandler = new DummyStopTermHandler();
    private TokeniserUtilities<string> _tokenUtilities = new TokeniserUtilities<string>();

    public string Delimiters => _delimiters;

    public string ShortDescriptionString => "TokeniserWhitespace";

    public ITermHandler StopWordHandler
    {
        get => _stopWordHandler;
        set => _stopWordHandler = value;
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
                for (var j = 0; j < _delimiters.Length; j++)
                {
                    var index = word.IndexOf(_delimiters[j], i);
                    if (index < length && index != -1)
                    {
                        length = index;
                    }
                }
                var termToTest = word.Substring(i, length - i);
                if (!_stopWordHandler.IsWord(termToTest))
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
            return _tokenUtilities.CreateSet(Tokenize(word));
        }
        return null;
    }
}